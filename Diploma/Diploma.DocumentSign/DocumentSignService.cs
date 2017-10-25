using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.Core.Repositories;
using Diploma.Core.Repositories.Abstracts.Base;
using Diploma.EmailSender.Abstracts;
using Diploma.EmailSender.Models;

namespace Diploma.DocumentSign
{
    public class DocumentSignService
    {
        private readonly IEmailNotificator _emailNotificator;
        private readonly BaseRepository<SignatureWarrant> _signatureWarrantRepository;
        private readonly BaseRepository<IncomingSignatureRequest> _signatureRequestRepository;

        public DocumentSignService(IEmailNotificator emailNotificator, BaseRepository<SignatureWarrant> signatureWarrantRepository, BaseRepository<IncomingSignatureRequest> signatureRequestRepository)
        {
            _emailNotificator = emailNotificator;
            _signatureWarrantRepository = signatureWarrantRepository;
            _signatureRequestRepository = signatureRequestRepository;
        }

        public async Task<bool> SignDocument(Document document, ApplicationUser user)
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

                return false;
            }

            UserKeys userKeys;
            string signDetails = string.Empty;

            if (signatureRequest.ClonnedUsingWarrant)
            {
                var userWarrants = _signatureWarrantRepository.FindBy(x => x.ToUser == user.Email).ToList();
                var validWarrant = userWarrants
                    .OrderBy(x => x.Expired)
                    .LastOrDefault(x => x.Expired < DateTime.Now);

                if (userWarrants.Any() && validWarrant == null)
                {
                    await _emailNotificator.SendErrorReportToAdmin(new EventReportModel
                    {
                        Subject = "Document sign",
                        Body = "Your all warrants were expired.",
                        ToUser = user.Email                        
                    });

                    return false;
                }
                else
                {
                    var originalRequest = _signatureRequestRepository
                        .FindBy(x => !x.ClonnedUsingWarrant && x.DocumentId == document.Id)
                        .Single();

                    userKeys = originalRequest.ApplicationUser.UserKeys;

                    signDetails = "(using signature warrant)";
                }
            }
            else
            {
                userKeys = user.UserKeys;
            }

            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.ImportParameters(ConvetToRsaParameters(userKeys));
                    
                    var hash = SHA1.Create().ComputeHash(document.Content);
                    
                    var signedBytes = rsa.SignHash(hash, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);

                    document.SignedByUser = $"{user.Email}{signDetails}";
                    document.Signature = Convert.ToBase64String(signedBytes);

                    var newContent = document.Content.ToList();
                    newContent.AddRange(signedBytes);
                    document.Content = newContent.ToArray();

                    var signatureRequests = _signatureRequestRepository
                        .FindBy(x => x.DocumentId == document.Id).ToList();

                    foreach (var signRequest in signatureRequests)
                    {
                        await _signatureRequestRepository.Remove(signRequest.Id.ToString());
                    }

                    await _emailNotificator.SendEventNotificationToUser(new EventReportModel
                    {
                        Subject = "Document sign",
                        Body = $"Document {document.DocumentName} with version was signed.",
                        ToUser = signatureRequest.UserRequester
                    });
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
            bool success = false;
            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.ImportParameters(publicKey);

                    SHA1 sha1 = SHA1.Create();                    

                    byte[] hash = sha1.ComputeHash(document.Content);

                    byte[] signedBytes = Convert.FromBase64String(document.Signature);

                    success = rsa.VerifyHash(hash, signedBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                }
                catch (CryptographicException e)
                {
                    
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }

            return success;
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
    }
}
