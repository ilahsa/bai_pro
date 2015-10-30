using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;

namespace IICCommonlibrary_NUnitTest
{
    public class HttpListenerServer
    {
        private HttpListener _listener;

        public HttpListenerServer(string url)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(url);
        }

        public void Start()
        {
            _listener.Start();
            _listener.BeginGetContext(new AsyncCallback(ListenerCallback), this);

        }

        private static void ListenerCallback(IAsyncResult iResult)
        {
            try
            {
                HttpListenerServer httpServer = (HttpListenerServer)iResult.AsyncState;
                HttpListenerContext httpContext = httpServer._listener.EndGetContext(iResult);
                Business(httpContext);
                httpServer._listener.BeginGetContext(new AsyncCallback(ListenerCallback), httpServer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static  void Business(HttpListenerContext httpContext)
        {
            HttpListenerRequest req = httpContext.Request;
            string str = "222222222222222";//req.QueryString.Get("test");
            HttpListenerResponse resp = httpContext.Response;
            byte[] bys = Encoding.UTF8.GetBytes(str);
            resp.OutputStream.Write(bys, 0, bys.Length);
            resp.Close();
        }
    }
}
