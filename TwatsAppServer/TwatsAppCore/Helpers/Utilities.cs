using System;
using System.Security.Cryptography;
using System.Text;

namespace TwatsAppCore.Helpers
{
    static class Utils
    {
        public static string GenerateSHA256String(string inputString)
        {
            byte[] data = Encoding.UTF8.GetBytes(inputString);
            using (SHA256 shaM = new SHA256Managed())
            {
                byte[] result = shaM.ComputeHash(data);
                return Convert.ToBase64String(result);
            }
        }

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}
