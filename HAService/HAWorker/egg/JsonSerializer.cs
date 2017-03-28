using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;

namespace HAWorker.egg
{
    /// <summary>
    /// json序列化工具
    /// </summary>
    public class JsonSerializer
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="jsonString">json字符串</param>
        /// <returns>T类型对象</returns>
        public static T JString2Object<T>(string jsonString)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">需要序列化的对象</param>
        /// <returns>json字符串</returns>
        public static string Object2JString(object obj)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(obj.GetType()).WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="jsonString">json字符串</param>
        /// <returns>key value对应字典</returns>
        public static Dictionary<string, object> JStringToDictionary(string jsonString)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<Dictionary<string, object>>(jsonString);
        }
    }
}
