using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;



namespace Imps.Services.CommonV4
{
    /*
     * A deligate void to be called if there's a network issue
     */
    public delegate void ConnectionIssue(WebException ex);


    /// <summary>
    /// Request is a static class that holds the http methods
    /// </summary>
    public static class SimpleHttpRequest
    {


        /*
         *  Events
         */
        public static event ConnectionIssue ConnectFailed = delegate { };



        /*
         * Cookie Container
         */
        private static CookieContainer cookies = new CookieContainer();

        /*
         * Request Settings
         */
        private const string UserAgent = "HttpLib 1.0.2";
        public static WebProxy Proxy;


        /*
         * Methods
         */
        #region Methods

        #region GET
        /// <summary>
        /// Performs a HTTP get request
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <param name="successCallback">A function that is called on success</param>
        public static void Get(string url, Action<string> successCallback)
        {
            Get(url, new { }, successCallback);
        }

        /// <summary>
        /// Performs a HTTP get request with parameters
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <param name="parameters">KVP Array of parameters</param>
        /// <param name="successCallback">A function that is called on success</param>
        public static void Get(string url, object parameters, Action<string> successCallback)
        {
            Get(url, parameters, successCallback, (webEx) =>
            {
                ConnectFailed(webEx);
            });
        }

        /// <summary>
        /// Performs a HTTP get request with parameters and a function that is called on failure
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <param name="parameters">KVP Array of parameters</param>
        /// <param name="successCallback">Function that is called on success</param>
        /// <param name="failCallback">Function that is called on failure</param>
        public static void Get(string url, object parameters, Action<string> successCallback, Action<WebException> failCallback)
        {
            UriBuilder b = new UriBuilder(url);

            /*
             * Append Paramters to the URL
             */


            if (parameters != null)
            {
                if (!string.IsNullOrWhiteSpace(b.Query))
                {
                    b.Query = b.Query.Substring(1) + "&" + HttpUtils.SerializeQueryString(parameters);
                }
                else
                {
                    b.Query = HttpUtils.SerializeQueryString(parameters);
                }

            }


            MakeRequest("application/x-www-form-urlencoded", "GET", b.Uri.ToString(), new { }, successCallback, failCallback);
        }
        #endregion

        #region HEAD
        /// <summary>
        /// Performs a HTTP head operation
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <param name="successCallback">A function that is called on success</param>
        public static void Head(string url, Action<string> successCallback)
        {
            Head(url, new { }, successCallback);
        }

        /// <summary>
        /// Performs a HTTP head operation with parameters
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <param name="parameters">KVP Array of parameters</param>
        /// <param name="successCallback">A function that is called on success</param>
        public static void Head(string url, object parameters, Action<string> successCallback)
        {
            Head(url, parameters, successCallback, (webEx) =>
            {
                ConnectFailed(webEx);
            });
        }

        /// <summary>
        /// Performs a HTTP head operation with parameters and a function that is called on failure
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <param name="parameters">KVP Array of parameters</param>
        /// <param name="successCallback">Function that is called on success</param>
        /// <param name="failCallback">Function that is called on failure</param>
        public static void Head(string url, object parameters, Action<string> successCallback, Action<WebException> failCallback)
        {
            UriBuilder b = new UriBuilder(url);

            /*
             * Append Paramters to the URL
             */
            if (parameters.GetType().GetProperties().Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(b.Query))
                {
                    b.Query = b.Query.Substring(1) + "&" + HttpUtils.SerializeQueryString(parameters);
                }
                else
                {
                    b.Query = HttpUtils.SerializeQueryString(parameters);
                }

            }


            MakeRequest("application/x-www-form-urlencoded", "GET", b.Uri.ToString(), new { }, successCallback, failCallback);
        }
        #endregion

        #region POST
        /// <summary>
        /// Performs a HTTP post request on a target with parameters
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <param name="parameters">Array of parameters</param>
        /// <param name="successCallback">Function that is called on success</param>
        public static void Post(string url, object parameters, Action<string> successCallback)
        {
            MakeRequest("application/x-www-form-urlencoded", "POST", url, parameters, successCallback, (webEx) =>
            {
                ConnectFailed(webEx);

            });
        }

        /// <summary>
        /// Performs a HTTP post request with parameters and a fail function
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <param name="parameters">Array of parameters</param>
        /// <param name="successCallback">Function that is called on success</param>
        /// <param name="failCallback">Function that is called on failure</param>
        public static void Post(string url, object parameters, Action<string> successCallback, Action<WebException> failCallback)
        {
            MakeRequest("application/x-www-form-urlencoded", "POST", url, parameters, successCallback, failCallback);
        }

        #endregion

        #region PUT
        /// <summary>
        /// Performs a HTTP put request on a target with parameters
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <param name="parameters">Array of parameters</param>
        /// <param name="successCallback">Function that is called on success</param>
        public static void Put(string url, object parameters, Action<string> successCallback)
        {
            MakeRequest("application/x-www-form-urlencoded", "PUT", url, parameters, successCallback, (webEx) =>
            {
                ConnectFailed(webEx);

            });
        }

        /// <summary>
        /// Performs a HTTP put request with parameters and a fail function
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <param name="parameters">Array of parameters</param>
        /// <param name="successCallback">Function that is called on success</param>
        /// <param name="failCallback">Function that is called on failure</param>
        public static void Put(string url, object parameters, Action<string> successCallback, Action<WebException> failCallback)
        {
            MakeRequest("application/x-www-form-urlencoded", "PUT", url, parameters, successCallback, failCallback);
        }
        #endregion

        #region DELETE
        /// <summary>
        /// Performs a HTTP delete request with parameters and a fail function
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <param name="parameters">Array of parameters</param>
        /// <param name="successCallback">Function that is called on success</param>
        public static void Delete(string url, object parameters, Action<string> successCallback)
        {
            MakeRequest("application/x-www-form-urlencoded", "DELETE", url, parameters, successCallback, (webEx) =>
            {
                ConnectFailed(webEx);

            });
        }

        /// <summary>
        /// Performs a HTTP delete request with parameters and a fail function
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <param name="parameters">Array of parameters</param>
        /// <param name="successCallback">Function that is called on success</param>
        /// <param name="failCallback">Function that is called on failure</param>
        public static void Delete(string url, object parameters, Action<string> successCallback, Action<WebException> failCallback)
        {
            MakeRequest("application/x-www-form-urlencoded", "DELETE", url, parameters, successCallback, failCallback);
        }
        #endregion

        #region UPLOAD
        /// <summary>
        /// Upload an array of files to a remote host using the HTTP post multipart method
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <param name="parameters">Parmaters</param>
        /// <param name="files">An array of files</param>
        /// <param name="successCallback">funciton that is called on success</param>
        public static void Upload(string url, object parameters, NamedFileStream[] files, Action<string> successCallback)
        {
            Upload(url, parameters, files, successCallback, (webEx) =>
            {
                ConnectFailed(webEx);
            });
        }

        /// <summary>
        /// Upload an array of files to a remote host using the HTTP post multipart method
        /// </summary>
        /// <param name="url">Target URL</param>
        /// <param name="parameters">Parmaters</param>
        /// <param name="files">An array of files</param>
        /// <param name="successCallback">Funciton that is called on success</param>
        /// <param name="FailCallback">Function that is called on failure</param>
        public static void Upload(string url, object parameters, NamedFileStream[] files, Action<string> successCallback, Action<WebException> FailCallback)
        {
            try
            {
                /*
                 * Generate a random boundry string
                 */
                string boundary = RandomString(12);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                request.Method = "POST";
                request.ContentType = "multipart/form-data, boundary=" + boundary;
                request.CookieContainer = cookies;
                request.UserAgent = UserAgent;

                if (Proxy != null)
                {
                    request.Proxy = Proxy;
                }


                request.BeginGetRequestStream(new AsyncCallback((IAsyncResult asynchronousResult) =>
                {
                    /*
                     * Create a new request
                     */
                    HttpWebRequest tmprequest = (HttpWebRequest)asynchronousResult.AsyncState;

                    /*
                     * Get a stream that we can write to
                     */
                    Stream postStream = tmprequest.EndGetRequestStream(asynchronousResult);
                    string querystring = "\n";

                    /*
                     * Serialize parameters in multipart manner
                     */
                    foreach (var property in parameters.GetType().GetProperties())
                    {
                        querystring += "--" + boundary + "\n";
                        querystring += "content-disposition: form-data; name=\"" + System.Uri.EscapeDataString(property.Name) + "\"\n\n";
                        querystring += System.Uri.EscapeDataString(property.GetValue(parameters, null).ToString());
                        querystring += "\n";
                    }


                    /*
                     * Then write query string to the postStream
                     */
                    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(querystring);
                    postStream.Write(byteArray, 0, byteArray.Length);

                    /*
                     * A boundary string that we'll reuse to separate files
                     */
                    byte[] closing = System.Text.Encoding.UTF8.GetBytes("\n--" + boundary + "--\n");


                    /*
                     * Write each files to the postStream
                     */
                    foreach (NamedFileStream file in files)
                    {
                        /*
                         * A temporary buffer to hold the file stream
                         * Not sure if this is needed ???
                         */
                        Stream outBuffer = new MemoryStream();

                        /*
                         * Additional info that is prepended to the file
                         */
                        string qsAppend;
                        qsAppend = "--" + boundary + "\ncontent-disposition: form-data; name=\"" + file.Name + "\"; filename=\"" + file.Filename + "\"\r\nContent-Type: " + file.ContentType + "\r\n\r\n";

                        /*
                         * Read the file into the output buffer
                         */
                        StreamReader sr = new StreamReader(file.Stream);
                        outBuffer.Write(System.Text.Encoding.UTF8.GetBytes(qsAppend), 0, qsAppend.Length);

                        int bytesRead = 0;
                        byte[] buffer = new byte[4096];

                        while ((bytesRead = file.Stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            outBuffer.Write(buffer, 0, bytesRead);

                        }


                        /*
                         * Write the delimiter to the output buffer
                         */
                        outBuffer.Write(closing, 0, closing.Length);



                        /*
                         * Write the output buffer to the post stream using an intemediate byteArray
                         */
                        outBuffer.Position = 0;
                        byte[] tempBuffer = new byte[outBuffer.Length];
                        outBuffer.Read(tempBuffer, 0, tempBuffer.Length);
                        postStream.Write(tempBuffer, 0, tempBuffer.Length);
                        postStream.Flush();
                    }


                    postStream.Flush();
                    postStream.Dispose();

                    tmprequest.BeginGetResponse(ProcessCallback(successCallback, FailCallback), tmprequest);

                }), request);
            }
            catch (WebException webEx)
            {
                FailCallback(webEx);
            }

        }
        #endregion

        #region Private Methods
        private static void MakeRequest(string ContentType, string Method, string URL, object Parameters, Action<string> SuccessCallback, Action<WebException> FailCallback)
        {
            if (Parameters == null)
            {
                throw new ArgumentNullException("Parameters object cannot be null");
            }

            if (string.IsNullOrWhiteSpace(ContentType))
            {
                throw new ArgumentException("Content type is missing");
            }

            if (string.IsNullOrWhiteSpace(URL))
            {
                throw new ArgumentException("URL is empty");
            }

            if (Method != "HEAD" && Method != "GET" && Method != "POST" && Method != "DELETE" && Method != "PUT")
            {
                throw new ArgumentException("Invalid Method");
            }

            try
            {
                /*
                 * Create new Request
                 */
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(URL));
                request.CookieContainer = cookies;
                request.Method = Method;
                request.ContentType = ContentType;

                request.UserAgent = UserAgent;

                if (Proxy != null)
                {
                    request.Proxy = Proxy;
                }


                /*
                 * Asynchronously get the response
                 */
                if (Method == "POST" || Method == "PUT" || Method == "DELETE")
                {
                    request.BeginGetRequestStream(new AsyncCallback((IAsyncResult callbackResult) =>
                    {

                        HttpWebRequest tmprequest = (HttpWebRequest)callbackResult.AsyncState;
                        Stream postStream;

                        postStream = tmprequest.EndGetRequestStream(callbackResult);


                        string postbody = "";


                        postbody = HttpUtils.SerializeQueryString(Parameters);


                        // Convert the string into a byte array. 
                        byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postbody);

                        // Write to the request stream.
                        postStream.Write(byteArray, 0, byteArray.Length);
                        postStream.Flush();
                        postStream.Dispose();

                        // Start the asynchronous operation to get the response
                        tmprequest.BeginGetResponse(ProcessCallback(SuccessCallback, FailCallback), tmprequest);


                    }), request);
                }
                else if (Method == "GET" || Method == "HEAD")
                {
                    request.BeginGetResponse(ProcessCallback(SuccessCallback, FailCallback), request);
                }
            }
            catch (WebException webEx)
            {
                FailCallback(webEx);
            }
        }

        /*
         * Muhammad Akhtar @StackOverflow
         */
        private static string RandomString(int passwordLength)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            char[] chars = new char[passwordLength];
            Random rd = new Random();

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }



        private static AsyncCallback ProcessCallback(Action<string> Success, Action<WebException> Fail)
        {
            return new AsyncCallback((callbackResult) =>
            {
                HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;

                try
                {
                    using (HttpWebResponse myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult))
                    {


                        using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
                        {
                            string results = httpwebStreamReader.ReadToEnd();

                            Success(results);
                        }
                    }

                }
                catch (WebException webEx)
                {
                    if (ConnectFailed != null)
                    {
                        Fail(webEx);

                    }

                }
            });
        }
        #endregion




        #endregion
    }


}
