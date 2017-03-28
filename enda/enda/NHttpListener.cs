using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace enda
{
    public class NHttpListener : AsyncHttpListener
    {

        public NHttpListener(int port):base(port) {

        }
        public override void ProcessRequest(HttpListenerContext httpContext)
        {
            var req = httpContext.Request;
            var resp = httpContext.Response;
            try
            {
                var stream = req.InputStream;
                if (stream != null)
                {
                    var reader = new StreamReader(stream);
                    var strJson = reader.ReadToEnd();
                    //打日志的可以去掉 
                    Console.WriteLine("receive:" + strJson);
                    var objJson = JsonSerializer.JString2Object<Entity>(strJson);
                    var respBody = Business.GetData(objJson);
                    //打日志的可以去掉 
                    Console.WriteLine("send:" + respBody);
                    var bys = Encoding.UTF8.GetBytes(respBody);
                    resp.OutputStream.Write(bys, 0, bys.Length);
                }
                else
                {
                    NLog.Info("bad request");
                }
                resp.OutputStream.Flush();
            }
            catch (Exception ex)
            {
                NLog.Info("accer error " + ex.Message);
            }
            finally {
                resp.Close();
            }
         
            
        }
    }
}
