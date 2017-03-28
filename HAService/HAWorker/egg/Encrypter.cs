using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HAWorker.egg
{

    /// <summary>
    /// Base64加解密
    /// </summary>
    public class Encrypter_Base64
    {
        /// <summary>
        /// 对字符串进行Base64加密，得到加密后的字符串
        /// </summary>
        /// <param name="str">加密前字符串</param>
        /// <returns>加密后字符串</returns>
        public static string String2Base64(string str)
        {
            byte[] bytedata = System.Text.Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytedata, 0, bytedata.Length);
        }

        /// <summary>
        /// Base64字符串解密，得到解密后的字符串
        /// </summary>
        /// <param name="base64">解密前字符串</param>
        /// <returns>解密后字符串</returns>
        public static string Base642String(string base64)
        {
            byte[] bytedata = Convert.FromBase64String(base64);
            return System.Text.ASCIIEncoding.UTF8.GetString(bytedata);
        }
    }

    /// <summary>
    /// RC4加解密
    /// </summary>
    public class Encrypter_RC4
    {
        /// <summary>
        /// rc4加密
        /// </summary>
        /// <param name="content">要加密的内容</param>
        /// <param name="key">密码</param>
        /// <returns>返回已加密二进制串，使用BitConverter.ToString()可转换成字符串查看</returns>
        private static byte[] Encrypt(string content, string key)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(content);
            return Encrypt_RC4(System.Text.Encoding.UTF8.GetBytes(key), data);
        }

        /// <summary>
        /// 完成加密
        /// </summary>
        /// <param name="key">密码</param>
        /// <param name="data">加密前</param>
        /// <returns>加密后的字节数组</returns>
        private static byte[] Encrypt_RC4(byte[] key, byte[] data)
        {
            return EncryptOutput(key, data).ToArray();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="key">密码</param>
        /// <param name="data">解密前</param>
        /// <returns>解密后的数据数组</returns>
        private static byte[] Decrypt_RC4(byte[] key, byte[] data)
        {
            return EncryptOutput(key, data).ToArray();
        }

        /// <summary>
        /// 不明
        /// </summary>
        /// <param name="key">密码</param>
        /// <returns>不明。。</returns>
        private static byte[] EncryptInitalize(byte[] key)
        {
            byte[] s = Enumerable.Range(0, 256)
                .Select(i => (byte)i)
                .ToArray();

            for (int i = 0, j = 0; i < 256; i++)
            {
                j = (j + key[i % key.Length] + s[i]) & 255;

                Swap(s, i, j);
            }

            return s;
        }

        /// <summary>
        /// 加解密
        /// </summary>
        /// <param name="key">密码</param>
        /// <param name="data">数据</param>
        /// <returns>对应的解加密数据</returns>
        private static IEnumerable<byte> EncryptOutput(byte[] key, IEnumerable<byte> data)
        {
            byte[] s = EncryptInitalize(key);

            int i = 0;
            int j = 0;

            return data.Select((b) =>
            {
                i = (i + 1) & 255;
                j = (j + s[i]) & 255;

                Swap(s, i, j);

                return (byte)(b ^ s[(s[i] + s[j]) & 255]);
            });
        }

        /// <summary>
        /// 交换同一数组byte
        /// </summary>
        /// <param name="s">数组</param>
        /// <param name="i">index1</param>
        /// <param name="j">index2</param>
        private static void Swap(byte[] s, int i, int j)
        {
            byte c = s[i];

            s[i] = s[j];
            s[j] = c;
        }
    }

    /// <summary>
    /// AES加解密
    /// </summary>
    public class Encrypter_AES
    {
        /// <summary>
        /// 默认的key，即第一次发送时候的key， U2FsdGVkX1+4arjl
        /// </summary>
        //private static string defaultKey = Encrypter_Base64.Base642String("VTJGc2RHVmtYMSs0YXJqbA==");


        /// <summary>
        /// 对字符串进行AES加密
        /// </summary>
        /// <param name="content">需要加密的字符串</param>
        /// <param name="isSpecifiedKey">是否使用服务器指定密码</param>
        /// <returns>返回已加密二进制串，使用BitConverter.ToString()可转换成字符串查看</returns>
        public static byte[] EncryptString(string content, string key)
        {
            byte[] dataArray = System.Text.Encoding.UTF8.GetBytes(key);
            return EncryptString(content, dataArray, dataArray);
        }

        /// <summary>
        /// 对二进制进行AES加密
        /// </summary>
        /// <param name="originData">需要加密的二进制数据</param>
        /// <param name="isSpecifiedKey">是否使用服务器指定密码</param>
        /// <returns></returns>
        public static byte[] EncryptBinary(byte[] originData, string key)
        {
            byte[] dataArray = System.Text.Encoding.UTF8.GetBytes(key);
            return EncryptBinary(originData, dataArray, dataArray);
        }

        /// <summary>
        /// AES解密成字符串
        /// </summary>
        /// <param name="cipherText">AES加密后的二进制数据</param>
        /// <returns>解密后的字符串</returns>
        public static string DecryptToString(byte[] cipherText, string key)
        {
            byte[] dataArray = System.Text.Encoding.UTF8.GetBytes(key);
            return DecryptToString(cipherText, dataArray, dataArray);
        }

        /// <summary>
        /// string加密成byte[]
        /// </summary>
        /// <param name="plainText">初始</param>
        /// <param name="Key">key</param>
        /// <param name="IV">iv</param>
        /// <returns>加密后</returns>
        private static byte[] EncryptString(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        private static byte[] EncryptBinary(byte[] originData, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (BinaryWriter swEncrypt = new BinaryWriter(csEncrypt))
                        {
                            swEncrypt.Write(originData);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;

        }

        private static string DecryptToString(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }

        private static byte[] DecryptToBinary(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(cipherText), output = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (BinaryReader srDecrypt = new BinaryReader(csDecrypt))
                        {
                            int len = 0;
                            byte[] result = null;
                            do
                            {
                                byte[] temp = srDecrypt.ReadBytes(1024);
                                len = temp.Length;
                                if (result == null)
                                    result = temp;
                                else
                                    result.Append(temp);
                                    //{
                                    //    List<byte> source = result.ToList<byte>();
                                    //    source.AddRange(temp.ToList<byte>());
                                    //    result = source.ToArray<byte>();
                                    //}

                                } while (len == 1024);
                            return result;
                        }

                    }
                }

            }
        }
    }
}
