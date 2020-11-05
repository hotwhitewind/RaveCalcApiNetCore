using System;
using System.IO;

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
            using(var f = new FileStream("outHash.txt", FileMode.Create, FileAccess.Write))
            {
                using(var t = new StreamWriter(f))
                {
                    t.WriteLine("Salt:");
                    t.WriteLine(base64Salt);
                    t.WriteLine("Password:");
                    t.WriteLine(base64Password);
                }
            }
            Console.WriteLine("Password success writed!");
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
