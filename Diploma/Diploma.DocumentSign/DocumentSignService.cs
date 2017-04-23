using System;
using System.Security.Cryptography;
using Diploma.Core.Models;

namespace Diploma.DocumentSign
{
    public class DocumentSignService
    {
        public bool SignData(Document document, ApplicationUser user)
        {            
            byte[] signedBytes;
            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.ImportParameters(ConvetToRsaParameters(user.UserKeys));

                    SHA1 sha1 = SHA1.Create();
                    
                    byte[] hash = sha1.ComputeHash(document.Content);
                    
                    signedBytes = rsa.SignHash(hash, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);

                    document.SignedByUser = user.Email;
                    document.Signature = Convert.ToBase64String(signedBytes);
                }
                catch (CryptographicException e)
                {
                    return false;
                }
                finally
                {                    
                    rsa.PersistKeyInCsp = false;
                }
            }
            
            return true;
        }

        public bool VerifyData(Document document, RSAParameters publicKey)
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
