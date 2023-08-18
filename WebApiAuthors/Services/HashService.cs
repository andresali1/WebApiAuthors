using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using WebApiAuthors.DTOs;

namespace WebApiAuthors.Services
{
    public class HashService
    {
        /// <summary>
        /// Method to create a Random Salt
        /// </summary>
        /// <param name="plainText">PLain text to hash</param>
        /// <returns></returns>
        public HashResult Hash(string plainText)
        {
            var salt = new byte[16];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            return Hash(plainText, salt);
        }

        /// <summary>
        /// Method to create a Random Hash by given salt
        /// </summary>
        /// <param name="plainText">plain text to hash</param>
        /// <param name="salt">Generated Salt</param>
        /// <returns></returns>
        public HashResult Hash(string plainText, byte[] salt)
        {
            var derivatedKey = KeyDerivation.Pbkdf2(
                password: plainText,
                salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 32
            );

            var hash = Convert.ToBase64String(derivatedKey);

            return new HashResult()
            {
                Hash = hash,
                Salt = salt
            };
        }
    }
}
