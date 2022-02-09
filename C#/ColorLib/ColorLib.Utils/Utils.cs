using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ColorLib.Utils
{
    public class Utils
    {
        /// <summary>
        /// 去掉字符串中的数字
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string RemoveNumber(string key)
        {
            return Regex.Replace(key, @"\d", "");
        }
        public static string GetString(string a, string b, string? c = null)
        {
            int x = a.IndexOf(b) + b.Length;
            int y;
            if (c != null)
            {
                y = a.IndexOf(c, x);
                if (a[y - 1] == '"')
                {
                    y = a.IndexOf(c, y + 1);
                }
                if (y - x <= 0)
                    return a;
                else
                    return a[x..y];
            }
            else
                return a[x..];
        }
        public static string GetSHA1(string data)
        {
            return BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "");
        }
        public static string NewUUID()
        {
            return Guid.NewGuid().ToString().ToLower().Replace("-", "");
        }
        public static string BytesToHexString(byte[] src)
        {
            StringBuilder stringBuilder = new();
            if (src == null || src.Length <= 0)
            {
                return "";
            }
            for (int i = 0; i < src.Length; i++)
            {
                int v = src[i] & 0xFF;
                string hv = string.Format("{0:X2}", v);
                stringBuilder.Append(hv);
            }
            return stringBuilder.ToString();
        }
        public static byte[] HexStringToByte(string hex)
        {
            int len = hex.Length / 2;
            byte[] result = new byte[len];
            char[] achar = hex.ToCharArray();
            for (int i = 0; i < len; i++)
            {
                int pos = i * 2;
                result[i] = (byte)(ToByte(achar[pos]) << 4 | ToByte(achar[pos + 1]));
            }
            return result;
        }

        private const string bytelist = "0123456789ABCDEF";

        private static byte ToByte(char c)
        {
            byte b = (byte)bytelist.IndexOf(c);
            return b;
        }
        private static Regex regex = new Regex(@"^(((13[0-9]{1})|(15[0-35-9]{1})|(17[0-9]{1})|(18[0-9]{1}))+\d{8})$");
        public static bool PhoneCheck(long phone)
        {
            return regex.IsMatch(phone.ToString());
        }
    }
}