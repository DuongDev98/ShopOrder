using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ShopOrder.Utils
{
    public class PasswordUtils
    {
        public static string EncrytPass(string password)
        {
            return GetMd5(GetMd5(password + "1998") + "1998");
        }

        private static string GetMd5(string password)
        {
            string rs = "";
            using (var md5Hash = MD5.Create())
            {
                // Byte array representation of source string
                var sourceBytes = Encoding.UTF8.GetBytes(password);

                // Generate hash value(Byte Array) for input data
                var hashBytes = md5Hash.ComputeHash(sourceBytes);

                // Convert hash byte array to string
                rs = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }
            return rs;
        }
    }
}