using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Diploma.Core.Models;
using Diploma.EmailSender;
using Diploma.Repositories;
using Diploma.Services;
using Microsoft.AspNetCore.Identity;
using Diploma.EmailSender.Abstracts;
using Diploma.EmailSender.Models;

namespace Diploma.DocumentSign
{
    public class DocumentSignService
    {
        private readonly IEmailNotificator _emailNotificator = new EmailNotificator();
        private readonly SignatureWarrantRepository _signatureWarrantRepository = new SignatureWarrantRepository();
        private readonly SignatureRequestRepository _signatureRequestRepository = new SignatureRequestRepository();

        public async Task<bool> SignDocument(Document document, ApplicationUser user)
        {
            var signatureRequest = _signatureRequestRepository.GetAll().FirstOrDefault(x => x.ApplicationUserId == user.Id);
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
                var userWarrants = _signatureWarrantRepository.GetUserSignatureWarrants(user.Email);
                var validWarrant = userWarrants
                    .OrderBy(x => x.Expired)
                    .LastOrDefault(x => x.Expired < DateTime.Now);

                if (userWarrants.Any() && validWarrant == null)
                {
                    await _emailNotificator.SendErrorReportToAdmin(new EventReportModel
                    {
                        Subject = "Document sign",
                        Body = "Your all warrants were expire.",
                        ToUser = user.Email                        
                    });

                    return false;
                }
                else
                {
                    var originalRequest = _signatureRequestRepository.GetAll().Single(x => !x.ClonnedUsingWarrant && x.DocumentId == document.Id);

                    userKeys = originalRequest.ApplicationUser.UserKeys;

                    signDetails = "(using signature warrant)";
                }
            }
            else
            {
                userKeys = user.UserKeys;
            }

            byte[] signedBytes;
            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.ImportParameters(ConvetToRsaParameters(userKeys));

                    var sha1 = SHA1.Create();
                    
                    byte[] hash = sha1.ComputeHash(document.Content);
                    
                    signedBytes = rsa.SignHash(hash, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);

                    document.SignedByUser = $"{user.Email}{signDetails}";
                    document.Signature = Convert.ToBase64String(signedBytes);

                    var newContent = document.Content.ToList();
                    newContent.AddRange(signedBytes);
                    document.Content = newContent.ToArray();

                    var signatureRequests = _signatureRequestRepository.GetAll()
                        .Where(x => x.DocumentId == document.Id).ToList();

                    foreach (var signRequest in signatureRequests)
                    {
                        await _signatureRequestRepository.DeleteSignatureRequest(signRequest.Id);
                    }

                    await _emailNotificator.SendEventNotificationToUser(new EventReportModel
                    {
                        Subject = "Document sign",
                        Body = $"Document {document.DocumentName} with version was signed.",
                        ToUser = signatureRequest.UserRequester
                    });
                }
                catch (CryptographicException e)
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
