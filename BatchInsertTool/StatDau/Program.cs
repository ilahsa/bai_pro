using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using BatchInsertTool;
using Imps.Services.CommonV4;

namespace StatDau
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceSettings.InitService("test");
            // HaveEggLog();
            // InsertLog();
          //  LogInUUids();
            //CheckTaskFineshed();
           // return;

            var path = System.Configuration.ConfigurationSettings.AppSettings["path"];
            DirectoryInfo TheFolder = new DirectoryInfo(path);
         
            Console.WriteLine("start " + DateTime.Now);

            var ff = TheFolder.GetFiles();
           
            for (var i = 0; i < ff.Length; i++)
            {
                var lst = new Dictionary<string, commonlog>();
                Console.WriteLine("process " + ff[i].FullName);
                var lines = File.ReadLines(ff[i].FullName);
                var total = 0;
                foreach (var str in lines) {
                    try
                    {
                        total++;
                        //计算killer
                        //var cl = taskresult_killer(str);
                        //计算dsq
                        var cl = update_dsq(str);
                        if (cl != null) {
                            var key = cl.uuid + "_" + cl.Event;
                            if (!lst.Keys.Contains(key)) {
                                lst.Add(key, cl);
                            }
                            
                        }
                        if (lst.Values.Count >= 80000) {
                            DB._proxyDb.BulkInsert<commonlog>("commonlog", lst.Values.ToArray());
                            Console.WriteLine("lines " + lst.Values.Count);
                            lst = new Dictionary<string, commonlog>();
                        }
         
                    }
                    catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                    }
                }
                if (lst.Values.Count > 0) {
                    DB._proxyDb.BulkInsert<commonlog>("commonlog", lst.Values.ToArray());
                    Console.WriteLine("lines " + lst.Values.Count);
                }
                Console.WriteLine("finished lines " + total);
    
            }


            Console.WriteLine("finished");
            Console.ReadLine();

        }

        public static void HaveEggLog() {
            var filename = @"C:\Users\baibq\Desktop\webstat\ret.txt";
            var lines = File.ReadAllLines(filename);
            var path1 = @"C:\Users\baibq\Desktop\egglog\egg20160213";
            DirectoryInfo TheFolder = new DirectoryInfo(path1);

            Console.WriteLine("start " + DateTime.Now);

            var ff = TheFolder.GetFiles();
            var finishLst = new List<string>();
            var total = 0;
            for (var i = 0; i < ff.Length; i++)
            {
                var lst = new Dictionary<string, commonlog>();
                Console.WriteLine("process " + ff[i].FullName);
                var lines1 = File.ReadLines(ff[i].FullName);

                var regexTool = new RegexTool();
             
                foreach (var str in lines1)
                {
                    try
                    {
                        var tmp = "";
                        Regex gex3 = new Regex(@"{([\S +|\s +])*");
                        var m3 = gex3.Match(str);
                        if (m3.Success)
                        {
                            tmp = m3.Value;
                        }
                        var uid = regexTool.Substring(tmp, @"""uid"":""", @"""");
                        if (finishLst.Contains(uid)) {
                            continue;
                        }
                        if (tmp.IndexOf("{")>-1 && tmp.IndexOf("}") > -1&& tmp.IndexOf("checkupdate") > -1 && lines.Contains(uid)) {
                            using (StreamWriter write = new StreamWriter(@"C:\Users\baibq\Desktop\webstat\finish.txt",true)) {
                                write.WriteLine(tmp);
                            }
                            finishLst.Add(uid);
                            total++;
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
           
                Console.WriteLine("finished lines " + total);

            }

        }
        /// <summary>
        /// 插入单条log
        /// </summary>
        public static void InsertLog()
        {
            var path = System.Configuration.ConfigurationSettings.AppSettings["file"];
            var lines = File.ReadLines(path);

            var lst = new Dictionary<string, commonlog>();
            var finishcound = 0;
            foreach (var str in lines)
            {
                if (!string.IsNullOrEmpty(str) )
                {
                    var cm = new commonlog();

                    cm.str1 = "go";
                    cm.str2 = str;
                    cm.str3 = "0314";
                    cm.uuid = Guid.NewGuid().ToString();//str;
                    cm.dt = DateTime.Now;

                    if (cm != null)
                    {
                        var key = cm.uuid ;
                        if (!lst.Keys.Contains(key))
                        {
                            lst.Add(key, cm);
                        }
                    }

                 

                    if (lst.Values.Count > 1000)
                    {
                        DB._proxyDb.BulkInsert<commonlog>("commonlog", lst.Values.ToArray());
                        Console.WriteLine("lines " + lst.Values.Count);
                        finishcound = finishcound + lst.Values.Count;
                        Console.WriteLine("totallines " + finishcound);
                        lst = new Dictionary<string, commonlog>();

                    }
                }
            }
            if (lst.Values.Count > 0)
            {
                DB._proxyDb.BulkInsert<commonlog>("commonlog", lst.Values.ToArray());
                Console.WriteLine("lines " + lst.Values.Count);
                finishcound = finishcound + lst.Values.Count;
                Console.WriteLine("totallines " + finishcound);
            }
        }
        public static void CheckTaskFineshed() {
            var path = System.Configuration.ConfigurationSettings.AppSettings["file"];
            var url = "http://eggserver1.brotlab.net/admin.php";
            var lines = File.ReadLines(path);

            var lst = new Dictionary<string, commonlog>();
            var finishcound = 0;
            foreach (var str in lines) {
                if (!string.IsNullOrEmpty(str) && str.IndexOf("{") > -1 && str.IndexOf("}") > -1) {
                    var cm = CheckTaskFineshed_1(url, 49, str.Trim());
                    var cm1 = CheckTaskFineshed_1(url, 66, str.Trim());
                    if (cm != null) {
                        var key = cm.uuid + "_49";
                        if (!lst.Keys.Contains(key))
                        {
                            lst.Add(key, cm);
                        }
                    }

                    if (cm1 != null)
                    {
                        var key = cm1.uuid + "_66";
                        if (!lst.Keys.Contains(key))
                        {
                            lst.Add(key, cm1);
                        }
                    }

                    if (lst.Values.Count > 100)
                    {
                        DB._proxyDb.BulkInsert<commonlog>("commonlog", lst.Values.ToArray());
                        Console.WriteLine("lines " + lst.Values.Count);
                        finishcound = finishcound + lst.Values.Count;
                        Console.WriteLine("totallines " + finishcound);
                        lst = new Dictionary<string, commonlog>();
                        
                    }
                }
            }
            if (lst.Values.Count > 0)
            {
                DB._proxyDb.BulkInsert<commonlog>("commonlog", lst.Values.ToArray());
                Console.WriteLine("lines " + lst.Values.Count);
                finishcound = finishcound + lst.Values.Count;
                Console.WriteLine("totallines " + finishcound);
            }
        }

        public static commonlog CheckTaskFineshed_1(string url,int taskId,string uuid) {
            var reqStr = @"{""controller"":""task"",""action"":""checkusertaskstatus"",""data"":{""taskid"":@taskid,""uuid"":""@uuid""}}";
            var tmp = reqStr.Replace("@uuid",uuid);
            tmp = tmp.Replace("@taskid", taskId+"");
            List<KeyValuePair<string, string>> outheader = null;
            byte[] outbody = null;
            try
            {
                HttpUtils.HttpRequest("POST", url, "application/json", null, Encoding.UTF8.GetBytes(tmp),
out outheader, out outbody);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (outbody != null)
            {
                var resp = JsonConvert.DeserializeObject<resp>(Encoding.UTF8.GetString(outbody));

                if (resp.rc == 0 && resp.payroll != null)
                {
                    var cm = new commonlog();
                    //cm.int1 = resp.payroll.result;
                    cm.int2 = resp.payroll.completed_at;
                    cm.int3 = taskId;
                    cm.str1 = resp.payroll.result + "";
                    cm.str2 = uuid;
                    cm.uuid = uuid;
                    cm.str3 = "dsq_notalive";
                    cm.str4 = DateTimeExtension.StampToDateTime(cm.int2 + "").ToString();
                    return cm;
                }

                if (resp.rc == 0 && resp.payroll == null)
                {
                    var cm = new commonlog();
                    //cm.int1 = resp.payroll.result;
                    //cm.int2 = resp.payroll.completed_at;
                    cm.int3 = taskId;
                    //cm.str1 = resp.payroll.result + "";
                    cm.str2 = uuid;
                    cm.uuid = uuid;
                    cm.str3 = "dsq_notalive_notfinishtask";
                    //cm.str4 = DateTimeExtension.StampToDateTime(cm.int2 + "").ToString();
                    return cm;
                }
            }

            return null;
        }
        public static commonlog update_dsq(string str)
        {
            if (!(str.IndexOf("updateresult_dsq") > 0 ))
            {
                return null;
            }
            //匹配日期
            string date = null;
            string ip = null;
            Regex gex1 = new Regex(@"\d+\/\d+\/\d+ \d+\:\d+\:\d+");
            var m1 = gex1.Match(str);
            if (m1.Success)
            {
                date = m1.Value.Replace("/", "-");
            }
            //ip 
            Regex gex2 = new Regex(@"\d+\.\d+\.\d+\.\d+\:\d+");
            var m2 = gex2.Match(str);
            if (m2.Success)
            {
                ip = m2.Value.Split(new char[] { ':' })[0];
            }

            Regex gex3 = new Regex(@"{([\S +|\s +])*");
            var m3 = gex3.Match(str);
            if (m3.Success)
            {
                str = m3.Value;
            }



            if ((str.IndexOf(@"checkupdate") > 0 || str.IndexOf(@"start") > 0 || str.IndexOf(@"get") > 0 ||str.IndexOf(@"report") > 0 || str.IndexOf(@"taskresult") > 0 ||
                str.IndexOf(@"update") > 0 || str.IndexOf(@"adjs") > 0) && str.IndexOf("{")>-1 && str.IndexOf("{") > -1)
            {
                var data = JsonConvert.DeserializeObject<dsq>(str);
                if (data.Event == "checkupdate"||data.Event == "start" || data.Event == "get" || data.Event == "report"|| data.Event == "taskresult" || data.Event == "update_dsq" || data.Event == "update_dsqservice" || data.Event == "updateresult_dsq"
                     || data.Event == "updateresult_dsqservice" || data.Event == "adjs")
                {
                    var cl = new commonlog();
                    cl.str1 = data.Event;
                    cl.str2 = date;
                    cl.dt = DateTime.Parse(date);
                   // cl.str3 = ip;
                    cl.str4 = data.uuid;

                    if (data.uuid.Length > 60) {
                        return null;
                    }

                    cl.uuid = Guid.NewGuid().ToString();//data.uuid;
                    cl.str5 = data.version;
                    cl.Event = data.Event;
                    //cl.str6 = data.version;
                    cl.str6 = data.eggid+"";
                    //cl.str8 = data.os;
                    cl.int2 = data.locale;
                    //if (cl.str5+"".Length > 80) {
                    //    Console.WriteLine("aaaaa");
                    //}

                    if (data.data != null)
                    {
                        cl.str7 = data.data.parameter;
                        if (cl.str7 != null && cl.str7.Length > 100)
                        {
                            cl.str7 = data.data.parameter.Substring(0, 100);
                        }
                        cl.str8 = data.data.Result + "";
                        cl.str9 = data.data.Return +"";
                        cl.int1 = data.data.taskid;
                    }

                    //cl.int1 = data.locale;
                    return cl;
                }
            }

            return null;
        }
        public static commonlog taskresult_killer(string str)
        {
            Regex gex3 = new Regex(@"{([\S +|\s +])*");
            var m3 = gex3.Match(str);
            if (m3.Success)
            {
                str = m3.Value;
            }
            
            if (str.IndexOf("updateresult_dsq") > 0)//str.IndexOf("taskresult") > 0 && str.IndexOf(@"""eggid"":13") > 0 && str.IndexOf(@"""taskid"":55")>0)
            {
                var data = JsonConvert.DeserializeObject<taskresult_kill>(str);
                if (data.data != null )//&& data.data.Return == 0 && data.data.parameter != "")
                {
                    var cl = new commonlog();
                    cl.int1 = data.eggid;
                    cl.str1 = data.Event;
                    cl.str2 = data.version;
                    cl.int2 = data.data.Return;
                    cl.int3 = data.data.Result;
                    cl.str3 = data.data.parameter;
                    if (cl.str3.Length > 50) {
                        cl.str3 = cl.str3.Substring(0, 50);
                    }
                    cl.str4 = data.uuid;
                    cl.uuid = data.uuid ;
                    cl.int3 = data.data.taskid;

                    return cl;
                }
            }

            return null;
        }

        public static void LogInUUids() {
            var uuids = File.ReadAllLines(@"C:\Users\baibq\Downloads\uuids.txt");
            var lst = new List<string>(uuids);

            DirectoryInfo TheFolder = new DirectoryInfo(@"C:\Users\baibq\Downloads\kegg20160314");

            Console.WriteLine("start " + DateTime.Now);

            var ff = TheFolder.GetFiles();
            var rt = new RegexTool();
            for (int i = 0; i < ff.Length; i++) {
                var reads = File.ReadLines(ff[i].FullName);

                //var uids = new List<string>();
                foreach (var v in reads) {
                    var uid = rt.Substring(v, @"""uid"":""",@"""");
                    if (lst.Contains(uid)) {
                       // uids.Add(uid);
                        File.AppendAllLines(@"C:\Users\baibq\Downloads\0314\"+ ff[i].Name, new string[] { v});
                    }
                }
             

            }
        }
    }

    public class dsq {
        public string uuid;
        public string uid;
        public string sid;
        public string os;
        public int locale;

        public int eggid;
        public string channel;
        public string version;
        [JsonProperty("event")]
        public string Event;
        //  public int locale;
        public data data;

        public status status;
        public int value;
    }

    public class taskresult_kill {
        public string uuid;
        public string uid;
        public string channel;
        public int eggid;
        public string Event;
        public string version;
        public data data;
    }

    public class data {

        public string parameter;

        [JsonProperty("result")]
        public Int64 Result;

        [JsonProperty("return")]
        public Int64 Return;

        public Int64 taskid;
    }

    public class resp {
        public string uuid;
        public int rc;
        public payroll payroll;
        public status status;
    }

    public class status {
        public string host;

        [JsonProperty("oid")]
        public string Oid;
    }
    public class payroll {
        public Int64 completed_at;

        public object result;
    }
}
