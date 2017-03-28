using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace enda_test
{
    public static class HttpUtils
    {
        /// <summary>
        /// Serialize an array of Key-Value pairs into a URL encoded query string
        /// </summary>
        /// <param name="Parameters">The key-value pair array</param>
        /// <returns>The URL encoded query string</returns>
        public static string SerializeQueryString(object Parameters)
        {
            string querystring = "";
            int i = 0;
            try
            {
                foreach (var property in Parameters.GetType().GetProperties())
                {
                    querystring += property.Name + "=" + System.Uri.EscapeDataString(property.GetValue(Parameters, null).ToString());
                    if (++i < Parameters.GetType().GetProperties().Length)
                    {
                        querystring += "&";
                    }
                }

            }
            catch (NullReferenceException e)
            {
                throw new ArgumentNullException("Paramters cannot be a null object", e);
            }

            return querystring;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method">POST or GET</param>
        /// <param name="url"></param>
        /// <param name="contentType">application/json or ...</param>
        /// <param name="headers"></param>
        /// <param name="reqBytes"></param>
        /// <param name="respHeaders"></param>
        /// <param name="respBytes"></param>
        public static void HttpRequest(string method, string url, string contentType, IEnumerable<KeyValuePair<string, string>> headers,
            byte[] reqBytes, out List<KeyValuePair<string, string>> respHeaders, out byte[] respBytes)
        {

            if ("POST,GET".IndexOf(method) < 0)
            {
                throw new Exception("method not support!");
            }
            respHeaders = new List<KeyValuePair<string, string>>();

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.Method = method;
            request.Timeout = 1000 * 30;
            if (contentType != null)
            {
                request.ContentType = contentType;
            }

            if (headers != null)
            {
                foreach (var h in headers)
                {
                    request.Headers.Add(h.Key, h.Value);
                }
            }

            if (method == "POST" && reqBytes != null)
            {

                using (var reqStream = request.GetRequestStream())
                {
                    reqStream.Write(reqBytes, 0, reqBytes.Length);
                    reqStream.Flush();
                }

            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            foreach (var key in response.Headers.AllKeys)
            {
                var value = response.Headers.Get(key);
                respHeaders.Add(new KeyValuePair<string, string>(key, value));
            }
            List<ArraySegment<byte>> lst = new List<ArraySegment<byte>>();

            var buf = new byte[1024];
            var len = 0;
            var totalLen = 0;
            using (var respStream = response.GetResponseStream())
            {
                while ((len = respStream.Read(buf, 0, buf.Length)) != 0)
                {
                    totalLen += len;
                    var tmpBuf = new byte[len];
                    Buffer.BlockCopy(buf, 0, tmpBuf, 0, len);
                    var seg = new ArraySegment<byte>(tmpBuf);
                    lst.Add(seg);
                }
            }
            respBytes = new byte[totalLen];
            var offset = 0;
            foreach (var seg in lst)
            {
                Buffer.BlockCopy(seg.Array, 0, respBytes, offset, seg.Array.Length);
                offset += seg.Array.Length;
            }
        }
    }
}
