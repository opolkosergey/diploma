using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Core.Repositories.Abstracts.Base;
using Diploma.EmailSender.Abstracts;
using Diploma.EmailSender.Models;

namespace Diploma.Core.Services
{
    public class DocumentSignService
    {        
        private readonly IEmailNotificator _emailNotificator;
        private readonly BaseRepository<SignatureWarrant> _signatureWarrantRepository;
        private readonly BaseRepository<IncomingSignatureRequest> _signatureRequestRepository;

        public DocumentSignService(
            IEmailNotificator emailNotificator,
            BaseRepository<SignatureWarrant> signatureWarrantRepository,
            BaseRepository<IncomingSignatureRequest> signatureRequestRepository)
        {
            _emailNotificator = emailNotificator;
            _signatureWarrantRepository = signatureWarrantRepository;
            _signatureRequestRepository = signatureRequestRepository;
        }

        public async Task<bool> SignDocument(Document document, ApplicationUser user)
        {
            var signatureRequest = await GetSignatureRequestAsync(document, user);
            if (signatureRequest == null)
            {
                return false;
            }

            var preparatorySignatureData = await GetPreparatorySignatureDataAsync(signatureRequest, document, user);
            if (preparatorySignatureData == null)
            {
                return false;
            }

            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    SignDocument(document, user, preparatorySignatureData, rsa);

                    var signatureRequests = _signatureRequestRepository.FindBy(x => x.DocumentId == document.Id).ToList();
                    signatureRequests.ForEach(async x => await _signatureRequestRepository.Remove(x.Id.ToString()));

                    await NotifyUserSuccessfully(document, signatureRequest);
                }
                catch (Exception e)
                {
                    await _emailNotificator.SendErrorReportToAdmin(new ReportModel
                    {
                        Subject = "Document sign exception",
                        Body = e.ToString()
                    });

                    return false;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }

            return true;
        }

        public bool VerifySignature(Document document, RSAParameters publicKey)
        {
            var success = false;
            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.ImportParameters(publicKey);
                    using (var sha1 = SHA1.Create())
                    {
                        var hash = sha1.ComputeHash(document.Content);
                        var signedBytes = Convert.FromBase64String(document.Signature);
                        success = rsa.VerifyHash(hash, signedBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                    }
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }

            return success;
        }

        private async Task NotifyUserSuccessfully(Document document, IncomingSignatureRequest signatureRequest)
        {
            await _emailNotificator.SendEventNotificationToUser(new EventReportModel
            {
                Subject = "Document sign",
                Body = $"Document {document.DocumentName} with version {document.Version} was signed.",
                ToUser = signatureRequest.UserRequester
            });
        }

        private void SignDocument(Document document, ApplicationUser user, PreparatorySignatureData preparatorySignatureData, RSACryptoServiceProvider rsa)
        {
            rsa.ImportParameters(ConvetToRsaParameters(preparatorySignatureData.UserKeys));

            using (var sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(document.Content);

                var signedBytes = rsa.SignHash(hash, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);

                document.SignedByUser = $"{user.Email}{preparatorySignatureData.SignDetails}";
                document.Signature = Convert.ToBase64String(signedBytes);
                var newContent = document.Content.ToList();
                newContent.AddRange(signedBytes);
                document.Content = newContent.ToArray();
            }
        }

        private async Task<PreparatorySignatureData> GetPreparatorySignatureDataAsync(IncomingSignatureRequest signatureRequest, Document document, ApplicationUser user)
        {
            var result = new PreparatorySignatureData();

            if (signatureRequest.ClonnedUsingWarrant)
            {
                var validWarrant = _signatureWarrantRepository
                    .FindBy(x => x.ToUser == user.Email)
                    .OrderBy(x => x.Expired)
                    .LastOrDefault(x => x.Expired < DateTime.Now);

                if (validWarrant == null)
                {
                    await _emailNotificator.SendErrorReportToAdmin(new EventReportModel
                    {
                        Subject = "Document sign",
                        Body = "Your all warrants were expired.",
                        ToUser = user.Email
                    });

                    return null;
                }

                var originalRequest = _signatureRequestRepository
                    .FindBy(x => !x.ClonnedUsingWarrant && x.DocumentId == document.Id)
                    .Single();

                result.UserKeys = originalRequest.ApplicationUser.UserKeys;
                result.SignDetails = "(using signature warrant)";
            }
            else
            {
                result.UserKeys = user.UserKeys;
            }

            return result;
        }

        private async Task<IncomingSignatureRequest> GetSignatureRequestAsync(Document document, ApplicationUser user)
        {
            var signatureRequest = _signatureRequestRepository
                .FindBy(x => x.ApplicationUserId == user.Id && x.DocumentId == document.Id)
                .FirstOrDefault();

            if (signatureRequest == null)
            {
                await _emailNotificator.SendErrorReportToAdmin(new ReportModel
                {
                    Subject = "Document sign",
                    Body = $"User {user.Email} tried to sign document with id {document.Id} without request."
                });
            }

            return signatureRequest;
        }

        private RSAParameters ConvetToRsaParameters(UserKeys userKeys)
        {
            return new RSAParameters
            {
                D = userKeys.D,
                DP = userKeys.DP,
                DQ = userKeys.DQ,
                P = userKeys.P,
                Q = userKeys.Q,
                InverseQ = userKeys.InverseQ,
                Exponent = userKeys.Exponent,
                Modulus = userKeys.Modulus
            };
        }

        private class PreparatorySignatureData
        {
            public UserKeys UserKeys { get; set; }

            public string SignDetails { get; set; }
        }
    }
}
