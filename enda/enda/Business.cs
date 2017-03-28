using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enda
{
    public class Business
    {
        //这里处理业务，读数据 封装成json
        public static string GetData(Entity en) {
            //处理业务逻辑
            //switch (en.type) {
            //    case "typ1":
            //        //do something

            //}
            System.Threading.Thread.Sleep(1000 * 10);
            var resp = new Response();
            resp.rc = 200;
            resp.data = new data() { r1 = "test1", r2 = "test2" };
            var retStr = JsonSerializer.Object2JString(resp);

            return retStr;
        }
    }
}
