using MyFace.Models.Request;
using MyFace.Models.Database;
using System.Security.Cryptography;
using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MyFace.Helpers
{
    public class PasswordHelper
    {
        public PasswordHelperProcessor GetHashedPassword(string password)
        {
            
            // string password = user.Password;

            byte[] salt = new byte[128 / 8];

            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            string saltString = Convert.ToBase64String(salt);

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            
            PasswordHelperProcessor passwordProcessor = new PasswordHelperProcessor(hashed, saltString);
            
            return passwordProcessor;

        }
    }
}