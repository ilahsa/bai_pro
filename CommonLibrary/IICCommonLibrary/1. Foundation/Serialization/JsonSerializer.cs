using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Imps.Services.CommonV4
{
    public class JsonSerializer
    {
        public static byte[] Serialize<T>(T obj)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();

            string szJson = jsonSerializer.Serialize(obj);

            return Encoding.UTF8.GetBytes(szJson);

        }

        public static T Deserialize<T>(string jsonObj)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();

            T dsObj = jsonSerializer.Deserialize<T>(jsonObj);
            return dsObj;
        }
    }
}
