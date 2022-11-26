using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace TuVanKhamBenh.Models
{
    public class Common
    {
        public static String MD5Hash(String source)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            // compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(source));
            // get hash result after compute it
            byte[] result = md5.Hash;
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                // change it into 2 hexadecimal digits
                // for each byte
                text.Append(result[i].ToString("x2"));
            }
            return text.ToString();
        }
    }
}