using System.Collections.Generic;
using System;

namespace Imps.Services.CommonV4
{
    /// <summary>
    /// This abstract class is a container for utility functions used by HttpLib
    /// </summary>
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
    }
}
