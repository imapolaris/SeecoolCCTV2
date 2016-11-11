using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserPlugin
{
    public class LocalEncryptor
    {
        public static LocalEncryptor Instance { get; private set; }

        static LocalEncryptor()
        {
            Instance = new LocalEncryptor();
        }

        private byte[] _pwdKeys = new byte[] { 0xAD, 0xB5, 0xD3, 0xEC };
        private LocalEncryptor()
        {
        }

        public string Encrypt(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            for (int i = 0; i < _pwdKeys.Length; i++)
            {
                input = doEncrypt(input, _pwdKeys[i]);
            }
            return input;
        }

        private string doEncrypt(string input, byte eytKey)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ eytKey);
            }
            return Convert.ToBase64String(bytes);
        }

        public string Decrypt(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            for (int i = _pwdKeys.Length - 1; i >= 0; i--)
            {
                input = doDecrypt(input, _pwdKeys[i]);
            }
            return input;
        }

        private string doDecrypt(string input, byte dytKey)
        {
            byte[] bytes = Convert.FromBase64String(input);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ dytKey);
            }
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
