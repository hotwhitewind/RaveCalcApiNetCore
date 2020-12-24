using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GenerateHash
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter new Password:");
            string readPassword = Console.ReadLine();
            string base64Salt = "";
            string base64Password = CreatePasswordHash(readPassword, out base64Salt);
            string SHA384Password = ComputeHash(readPassword);
            using (var f = new FileStream("outHash.txt", FileMode.Create, FileAccess.Write))
            {
                using(var t = new StreamWriter(f))
                {
                    t.WriteLine("Salt:");
                    t.WriteLine(base64Salt);
                    t.WriteLine("Password:");
                    t.WriteLine(base64Password);
                    t.WriteLine("SHA384Password:");
                    t.WriteLine(SHA384Password);
                }
            }
            Console.WriteLine("Password success writed!");
        }

        private static string ComputeHash(string password)
        {
            SHA384CryptoServiceProvider hashAlgorithm = new SHA384CryptoServiceProvider();
            return Convert.ToBase64String(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        private static string CreatePasswordHash(string password, out string Salt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                var passwordSalt = hmac.Key;
                var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                string base64StringPassword = Convert.ToBase64String(passwordHash);
                Salt = Convert.ToBase64String(passwordSalt);
                return base64StringPassword;
            }
        }
    }
}
