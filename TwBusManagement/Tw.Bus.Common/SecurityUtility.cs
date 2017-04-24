using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Tw.Bus.Common
{
    public class SecurityUtility
    {
        /// <summary>
        /// 使用MD5算法哈希
        /// </summary>
        /// <param name="plainMessage"></param>
        /// <returns></returns>
        public static string GetMd5Hash(string plainMessage)
        {
            MD5 md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(plainMessage));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
