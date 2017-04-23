using System.Security.Cryptography;
using Diploma.Core.Models;

namespace Diploma.Helpers
{
    public static class ModelsConverter
    {
        public static UserKeys ConvetToUserKeys(this RSAParameters parameters)
        {
            return new UserKeys
            {
                D = parameters.D,
                DP = parameters.DP,
                DQ = parameters.DQ,
                P = parameters.P,
                Q = parameters.Q,
                InverseQ = parameters.InverseQ,
                Exponent = parameters.Exponent,
                Modulus = parameters.Modulus
            };
        }

        public static RSAParameters ConvetToRsaParameters(this UserKeys userKeys)
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
