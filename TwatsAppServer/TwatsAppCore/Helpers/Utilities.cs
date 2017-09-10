using System;
using System.Security.Cryptography;
using System.Text;

namespace TwatsAppCore.Helpers
{
    /// <summary>
    /// some util functions I didnt know where to put
    /// </summary>
    static class Utils
    {
        /// <summary>
        /// Generates a SHA256 encoded string , do not store raw passwords in data base!
        /// </summary>
        /// <param name="inputString">the string to encode</param>
        /// <returns>encoded string</returns>
        public static string GenerateSHA256String(string inputString)
        {
            byte[] data = Encoding.UTF8.GetBytes(inputString);
            using (SHA256 shaM = new SHA256Managed())
            {
                byte[] result = shaM.ComputeHash(data);
                return Convert.ToBase64String(result);
            }
        }
    }
}
