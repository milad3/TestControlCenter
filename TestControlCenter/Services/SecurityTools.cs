using System;
using System.Security.Cryptography;
using System.Text;
using TestControlCenter.Properties;
using TestControlCenter.Tools;

namespace TestControlCenter.Services
{
    public class SecurityTools
    {
        private static readonly Random random = new Random();

        public static string GetDatabasePassword()
        {
            var pass = Settings.Default.DatabasePassword;
            if (string.IsNullOrEmpty(pass))
            {
                pass = GenerateAndSavePassword();
            }
            else
            {
                pass = StringProtector.Unprotect(Settings.Default.DatabasePassword);
            }

            return CreateMD5(pass);
        }

        private static string GenerateAndSavePassword()
        {
            var data = GenerateString(8);

            Settings.Default.DatabasePassword = StringProtector.Protect(data);
            Settings.Default.Save();
            Settings.Default.Reload();

            return data;
        }

        public static string GenerateString(int length)
        {
            var characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }

            return result.ToString();
        }

        public static string CreateMD5(string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
