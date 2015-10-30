using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;

namespace Imps.Services.CommonV4
{
    public class JsonRpcHttpClientTransaction : IDisposable
    {

        private static ITracing _tracing = TracingManager.GetTracing("JsonRpc");

        private Action<JsonRpcResponse> _callback;
        private RegisteredWaitHandle _registeredHandle = null;

        private WebRequest _webRequest = null;
        private WebResponse _webResponse = null;
        private ManualResetEvent _waitHandle;
        private string _sericeUri;
        private int _timeOut = 1000*60;



        public JsonRpcHttpClientTransaction()
        {
        }

        public void Dispose()
        {
            if (_webRequest != null)
            {
                try
                {
                    _webRequest.Abort();
                }
                catch (Exception ex)
                {
                    SystemLog.Unexcepted(ex);
                }
            }
            if (_waitHandle != null)
            {
                _waitHandle.Close();
            }
        }
        public void SendRequest(JsonRpcRequest rpcRequest, Action<JsonRpcResponse> callback)
        {
            SendRequest(rpcRequest, _timeOut, callback);
        }
        public void SendRequest(JsonRpcRequest rpcRequest, int timeout, Action<JsonRpcResponse> callback)
        {
           // Console.WriteLine("发送请求11111111111111"+DateTime.Now.ToShortDateString());
            TracingManager.Info(
                delegate()
                {
                    string module = null;
                    string action = null;
                    if (rpcRequest.Header != null)
                    {
                        module = rpcRequest.Header["UU-REQUEST-MODULE"] == null ? "" : rpcRequest.Header["UU-REQUEST-MODULE"];
                        action = rpcRequest.Header["UU-REQUEST-ACTION"] == null ? "" : rpcRequest.Header["UU-REQUEST-ACTION"];
                    }
                    _tracing.Info(string.Format("jsonrpc request:uri={0} module={1} action={2}\r\nrequestbody:{3}",
                        rpcRequest.ServiceUri, module, action, rpcRequest.ReqBody));
                }
                );
            _sericeUri = rpcRequest.ServiceUri;
            _callback = callback;

            _webRequest = HttpWebRequest.Create(new Uri(_sericeUri));
            _webRequest.Method = "POST";
            _webRequest.Proxy = null;
            _webRequest.ContentType = "application/json";
            _webRequest.Headers.Add(HttpRequestHeader.From, rpcRequest.FromComputer);
            _webRequest.Headers.Add(HttpRequestHeader.Pragma, rpcRequest.FromService);

            if (rpcRequest.Header != null && rpcRequest.Header.Count > 0)
            {
                foreach (string key in rpcRequest.Header.AllKeys)
                {
                    _webRequest.Headers.Add(key, rpcRequest.Header[key]);
                }
            }

            byte[] buffer = null;
            if (rpcRequest.ReqBody == null)
            {
                _webRequest.ContentLength = 0;
            }
            else
            {
                buffer = Encoding.UTF8.GetBytes(rpcRequest.ReqBody);//Request.BodyBuffer.GetByteArray();
                _webRequest.ContentLength = buffer.Length;
            }

            timeout = timeout > 0 ? timeout : _timeOut;

            if (timeout > 0)
            {
                _waitHandle = new ManualResetEvent(false);
                _registeredHandle = ThreadPool.RegisterWaitForSingleObject(_waitHandle, new WaitOrTimerCallback(TimeoutCallback), this, timeout, true);
            }
            if (_webRequest.ContentLength == 0)
            {
                _webRequest.BeginGetResponse(new AsyncCallback(ResponseCallback), this);
            }
            else
            {
                _webRequest.BeginGetRequestStream(
                    delegate(IAsyncResult asyncResult)
                    {
                        JsonRpcHttpClientTransaction trans = (JsonRpcHttpClientTransaction)asyncResult.AsyncState;
                        try
                        {
                            WebRequest webReq = trans._webRequest;

                            Stream stream = webReq.EndGetRequestStream(asyncResult);
                            stream.Write(buffer, 0, buffer.Length);
                            stream.Close();
                            webReq.BeginGetResponse(new AsyncCallback(ResponseCallback), this);
                        }
                        catch (Exception ex)
                        {
                            var rpcResonse = new JsonRpcResponse(JsonRpcErrorCode.SendFailed, null, new JsonRpcException(_sericeUri, "send failed", ex), 0);
                            trans.OnCallback(rpcResonse);
                        }
                    },
                    this
                );
            }
        }


        //public bool Wait()
        //{
        //    if (_waitHandle != null)
        //        return _waitHandle.WaitOne(_timeOut + 10000);

        //    return false;
        //}

        private static void ResponseCallback(IAsyncResult asyncResult)
        {
            //Console.WriteLine("接收应答222222222222222" + DateTime.Now.ToShortDateString());
            JsonRpcHttpClientTransaction trans = (JsonRpcHttpClientTransaction)asyncResult.AsyncState;
            JsonRpcResponse response = null;
            WebResponse webResponse = null;
            try
            {
                webResponse = trans._webRequest.EndGetResponse(asyncResult);
                trans._webResponse = webResponse;

                string warn = webResponse.Headers.Get("UU-RESPONSE-RC");
                string lengthStr = webResponse.Headers.Get("UU-CONTENT-LENGTH");
                int length = 0;
                int.TryParse(lengthStr + "", out length);
                int orginalErrorCode = 0;
                int.TryParse(warn, out orginalErrorCode);
                string respBody = null;
                if (length > 0)
                {
                    Stream stream = webResponse.GetResponseStream();
                    StreamReader readerStream = new StreamReader(stream);
                    respBody = readerStream.ReadToEnd();

                    readerStream.Close();
                }

                if (!string.IsNullOrEmpty(warn) && warn != "0")
                {
                    JsonRpcErrorCode errCode = JsonRpcErrorCode.Unknown;
                    Enum.TryParse<JsonRpcErrorCode>(warn, true, out errCode);

                    response = new JsonRpcResponse(errCode, respBody, new JsonRpcException(trans._sericeUri, respBody, new Exception("rc != 0")), orginalErrorCode);

                }
                else
                {
                    response = new JsonRpcResponse(JsonRpcErrorCode.OK, respBody);
                }

                TracingManager.Info(
                    delegate()
                    {
                        string responseCode = warn == null ? "0" : warn;

                        _tracing.Info(string.Format("jsonrpc response:response rc ={0}\r\nresponse body:{1}",
                           responseCode, respBody));
                        //if (responseCode != "0")
                        //{
                        //    _tracing.ErrorFmt(response.Exception, "web data response error ,originalerrorcode is {0}", orginalErrorCode);
                        //}
                    }
                    );
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    response = new JsonRpcResponse(JsonRpcErrorCode.TransactionTimeout, new JsonRpcException(null, null, ex), 500);
                }
                else
                {
                    response = new JsonRpcResponse(JsonRpcErrorCode.SendFailed, new JsonRpcException(null, null, ex), 500);
                }
            }
            catch (Exception ex)
            {
                SystemLog.Error(LogEventID.RpcFailed, ex, "SendRequest failed");
                response = new JsonRpcResponse(JsonRpcErrorCode.SendFailed, new JsonRpcException(null, null, ex), 500);
            }
            finally
            {
                if (webResponse != null)
                    webResponse.Close();

                trans.OnCallback(response);
            }
        }

        private static void TimeoutCallback(object state, bool timedOut)
        {
            JsonRpcHttpClientTransaction trans = null;
            try
            {
                trans = (JsonRpcHttpClientTransaction)state;

                if (trans._registeredHandle != null)
                    trans._registeredHandle.Unregister(null);

                if (timedOut)
                { // Timeout
                    var response = new JsonRpcResponse(JsonRpcErrorCode.TransactionTimeout); //RpcResponse.Create(RpcErrorCode.TransactionTimeout, null);

                    trans.OnCallback(response);

                    //释放资源
                    trans.Dispose();
                }
            }
            catch (Exception ex)
            {
                SystemLog.Error(LogEventID.RpcFailed, ex, "TimeoutCallback");
            }
        }

        private void OnCallback(JsonRpcResponse resposne)
        {
            _callback(resposne);
            if (_waitHandle != null)
                _waitHandle.Set();

        }
    }
}
