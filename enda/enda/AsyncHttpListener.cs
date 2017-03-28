using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace enda
{
    public abstract class AsyncHttpListener
    {
        private HttpListener _listener;
        private int _pipeline = 50;
        #region Constructors
        public AsyncHttpListener(string prefix)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(prefix);
        }

        public AsyncHttpListener(int port, int pipeline = 50)
            : this(string.Format("http://*:{0}/", port))
        {
            _pipeline = pipeline;
        }
        #endregion

        public void Start()
        {
            try
            {
                _listener.Start();
                for (int i = 0; i < _pipeline; i++)
                {
                    _listener.BeginGetContext(new AsyncCallback(ListenerCallback), this);
                }
            }
            catch (Exception ex)
            {

            }

        }

        public void Stop()
        {
            _listener.Stop();
        }

        public abstract void ProcessRequest(HttpListenerContext httpContext);

        #region Private Listener Logic
        private void ListenerCallback(IAsyncResult result)
        {
            try
            {
                AsyncHttpListener listener = (AsyncHttpListener)result.AsyncState;
                listener._listener.BeginGetContext(ListenerCallback, listener);

                HttpListenerContext context = listener._listener.EndGetContext(result);
                try
                {
                    listener.ProcessRequest(context);
                }
                catch (Exception ex)
                {
                    NLog.Info("ProcessRequest error" +ex.Message);
                }

            }
            catch (Exception ex)
            {
                NLog.Info("AsyncHttpListener.ListenerCallback Failed" + ex.Message);
            }
        }

        #endregion
    }
}
