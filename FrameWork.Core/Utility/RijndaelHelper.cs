using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FrameWork.Core.Utility.Security
{
    /// <summary>
    /// Rijndael对称加密
    /// </summary>
    public static class RijndaelHelper
    {
        private static readonly SymmetricAlgorithm RijndaelService;
        private static readonly byte[] EncryptKey;
        private static readonly byte[] EncryptIv;

        static RijndaelHelper()
        {
            RijndaelService = Rijndael.Create();
            EncryptKey = Encoding.UTF8.GetBytes("TLsY@xICpJ!cLv#fkmKw!LRi74cHP5o$");
            EncryptIv = Encoding.UTF8.GetBytes("eGkqwISA9dzW1&8V");
        }

        /// <summary>
        /// 加密
        /// </summary>
        public static string Encrypt(this string data)
        {
            try
            {
                byte[] buffer;
                var encryptor = RijndaelService.CreateEncryptor(EncryptKey, EncryptIv);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        var writer = new StreamWriter(cs);
                        writer.Write(data);
                        writer.Flush();
                    }
                    buffer = ms.ToArray();
                }

                return Convert.ToBase64String(buffer);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("加密数据时出错", ex);
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static string Decrypt(this string data)
        {
            try
            {
                string result;
                var decryptor = RijndaelService.CreateDecryptor(EncryptKey, EncryptIv);

                using (var ms = new MemoryStream(Convert.FromBase64String(data)))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        var reader = new StreamReader(cs);
                        result = reader.ReadLine();
                        reader.Close();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("解密数据时出错", ex);
            }
        }
    }
}