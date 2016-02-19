using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Imps.Services.CommonV4;
using System.Threading;
using System.Collections.Concurrent;

using Imps.Services.CommonV4;

namespace BatchInsertTool
{
    class Program
    {
        static void Main(string[] args)
        {
            //var strt= @"{""appid"":2312335602,""channel"":""ntsvc"",""event"":""updateresult"",""eggid"":7,""version"":""1000.0.0.112"",""locale"":2058,""os"":""5.1"",""uid"":""{ 4fe985127e49412bb9480bf1f6ec1cc1}"",""amd64"":false,""data"":{""eggid"":7,""parameter"":""update result"",""result"":3758096384}}";
            //var aliveLog1 = JsonConvert.DeserializeObject<Alivelog>(strt);

            var path = System.Configuration.ConfigurationSettings.AppSettings["path"];
            ServiceSettings.InitService("test");

            //Console.WriteLine(DateTime.Now);

            //for (var i = 0; i < 1000; i++) {
            //    DB.GetInstance.Test();
            //}
            //Console.WriteLine(DateTime.Now);

            //return;
            AliveFilter afilter = new AliveFilter();
            DirectoryInfo TheFolder = new DirectoryInfo(path);
            var lst = new List<string>();
            //遍历文件夹
            // var total = 0;

            System.Diagnostics.Stopwatch sp = new System.Diagnostics.Stopwatch();
            Console.WriteLine("start " + DateTime.Now);

            var ff = TheFolder.GetFiles();
            for (var i=0;i<ff.Length;i++) {

                Console.WriteLine("process " + ff[i].FullName);


                var lines = File.ReadLines(ff[i].FullName);
                //lstStr.AddRange(lines);
               // tmpInt ++;
                // 3g个文件一批

                    sp.Start();
                    var aliveLst = new ConcurrentDictionary<string, Alivelog>();
                Semaphore sem = new Semaphore(8, 8);
                try
                {
                    // foreach(string str in lines)
                    Console.WriteLine("foreach 开始" + DateTime.Now.ToString());
                    System.Threading.Tasks.Parallel.ForEach(lines, (str) =>
                    {
                        sem.WaitOne();
                        string type = null;
                        string date = null;
                        string ip = null;
                        string jsonStr = null;
                        string eventValue = null;
                        string killer = null;
                        var b = BeforeFilter.Filter(str,
    out type, out date, out ip, out jsonStr, out eventValue,out killer);
                        if (!b)
                        {
                            if //(eventValue == "taskresult")
                            (eventValue == "checkupdate")
                            //  (eventValue == "checkupdate2")
                            //(eventValue == "updateresult" || eventValue == "checkupdate")
                            {
                                try
                                {
                                    var aliveLog = JsonConvert.DeserializeObject<Alivelog>(jsonStr);
             
                                    
                                    if (aliveLog.uid.Length > 50)
                                    {
                                        aliveLog.uid = aliveLog.uid.Substring(0, 50);
                                    }
                                    if (aliveLog.channel.Length > 20)
                                    {
                                        aliveLog.channel = aliveLog.channel.Substring(0, 20);
                                    }

                                    if (aliveLog.data != null) {
                                        aliveLog.result = aliveLog.data.result;
                                    }
                                    aliveLog.Event = eventValue;

                                    aliveLog.kill = killer;
                                    aliveLog.alivetime = DateTime.Parse(date);
                                    aliveLog.ip = ip;
                                        //afilter.Process(aliveLog, aliveLst);
                                    if (!aliveLst.ContainsKey(aliveLog.uid))
                                    {
                                        aliveLst.TryAdd(Guid.NewGuid().ToString(), aliveLog);
                                    }
                                }
                                catch (Exception ex)
                                {
                                        //eat
                                    }

                            }
                        }
                        sem.Release();
                    }
                );
                    Console.WriteLine("foreach 结束" + DateTime.Now.ToString());
                    Console.WriteLine("total lines " + aliveLst.Count);
                    Console.WriteLine("bulkinsert 开始" + DateTime.Now.ToString());
                    DB._proxyDb.BulkInsert<Alivelog>("alivelog", aliveLst.Values.ToArray());
                    Console.WriteLine("bulkinsert 结束" + DateTime.Now.ToString());
                    //  Thread.Sleep(1000 * 1);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally {
                    GC.Collect();
                }
                    sp.Stop();

                    Console.WriteLine(sp.ElapsedMilliseconds);

                  
            }

            Console.WriteLine("finished");
            Console.WriteLine("end " + DateTime.Now);
            Console.ReadLine();

        }
    }

    class Req {
        [JsonProperty("uid")]
        [TableField("uid")]
        public string uid;
        [JsonProperty("locale")]
        [TableField("locale")]
        public int locale;

        [TableField("time")]
        public DateTime time;

    }
}
