using System;
using System.Security.Cryptography;
using System.Text;

namespace WebChromiumCcsipro.Domain.Extensions
{
    public static class CryptoExtension
    {
        public static string GenerateSalt()
        {
            var bytes = new byte[64];
            var csprng = new RNGCryptoServiceProvider();
            csprng.GetBytes(bytes);
            var salt64 = Convert.ToBase64String(bytes);
            return salt64;
        }

        public static string HashPassword(string password, string salt)
        {
            var text = salt + password;
            var bytes = Encoding.UTF8.GetBytes(text);
            using (var sha = new SHA512CryptoServiceProvider())
            {
                var hash = sha.ComputeHash(bytes);
                var hash64 = Convert.ToBase64String(hash);
                return hash64;
            }
        }

        public static bool VerifyPassword(string password, string salt, string hashedPassword)
        {
            var newHash = HashPassword(password, salt);
            var result = newHash.Equals(hashedPassword, StringComparison.Ordinal);
            return result;
        }
    }
}
