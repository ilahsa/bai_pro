using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4;

namespace IICCommonlibrary_NUnitTest
{
    class HttpRequestTest
    {
        public static void GetTest()
        {
            HttpRequest.Get("http://uuservice.fbdesktop.com/webnav/prelogin?clienttype=uufb&clientversion=1.0.1.40&navconfigversion=0", x =>
            {
                Console.WriteLine(x);
            });
        }

        public static void Ser()
        {
            string str =HttpUtils.SerializeQueryString(new { k1="a",k2="3"});
            Console.WriteLine(str);
        }
    }
}
