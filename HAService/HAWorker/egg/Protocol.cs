using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAWorker.egg
{
    public static class Protocol
    {
        public static byte[] Encode(string key,byte[] bt, ushort  protocolVersion,bool isSpecifiedKey) {
            byte[] body = Encrypter_AES.EncryptBinary(bt, key);//AES密文
            byte[] tempdata = BitConverter.GetBytes(isSpecifiedKey);//是否使用服务器指定密码
            body = body.Prepend(tempdata);
            tempdata = BitConverter.GetBytes(protocolVersion);//协议版本号
            body = body.Prepend(tempdata);
            tempdata = BitConverter.GetBytes((UInt32)(body.Length));//包体长度
            body = body.Prepend(tempdata);
            return body;
        }

        public static string Decode(string key, byte[] bt) {
            byte[] dbt =new byte[bt.Length - 3];
            Buffer.BlockCopy(bt, 3, dbt, 0, dbt.Length);
            
            string ret = Encrypter_AES.DecryptToString(dbt, key);

            return ret;
        }
    }

        /// <summary>
        /// byte[]的拓展类
        /// </summary>
        public static class ByteArrayExtention
        {
            /// <summary>
            /// 在当前的byte[]上追加，当前byte[]不会改变，返回新的byte[]
            /// </summary>
            /// <param name="array">源，不能为空</param>
            /// <param name="tail">要追加的byte[]，不能为空</param>
            /// <returns>追加后的byte[]</returns>
            public static byte[] Append(this byte[] array, byte[] tail)
            {
                List<byte> source = array.ToList<byte>();
                source.AddRange(tail.ToList<byte>());
                return source.ToArray<byte>();
            }

            /// <summary>
            /// 在当前的byte[]之前插入，当前byte[]不会改变，返回新的byte[]
            /// </summary>
            /// <param name="array">源，不能为空</param>
            /// <param name="head">在源之前插入的byte[]</param>
            /// <returns>插入后的byte[]</returns>
            public static byte[] Prepend(this byte[] array, byte[] head)
            {
                return head.Append(array);
            }
        }
}
