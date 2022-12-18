using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Infrastructuur.extensions
{
    public static class HashPassword
    {
        public static string HashToPassword(this string password)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            var passwordData = Encoding.ASCII.GetBytes(password);
            var hash = sha1.ComputeHash(passwordData);
            return Convert.ToBase64String(hash);
        }
    }
}
