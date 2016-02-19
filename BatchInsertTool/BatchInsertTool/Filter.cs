using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

using Imps.Services.CommonV4;

namespace BatchInsertTool
{
    public class AliveFilter
    {
        ConcurrentDictionary<string, bool> dic = null;
        bool isClear = false;
        public AliveFilter() {
            dic = new ConcurrentDictionary<string, bool>();
            
        }
        //每天的  日活支取一条记录
        public  void Process(Alivelog log, ConcurrentDictionary<string,Alivelog> lst) {

            //utc 时间16 点 北京时间的临界点
            if (log.alivetime.Hour >= 16 && !isClear) {
                dic.Clear();
                
                isClear = true;
            }
            var key = string.Format("{0}{1}", log.uid, log.version);
            if (!dic.ContainsKey(key))
            {
                dic.TryAdd(key, true);
                if (!lst.ContainsKey(log.uid)) {
                    lst.TryAdd(log.uid, log);
                }
                
            }
            else {
                //Console.WriteLine("2222");
            }
        }
    }

    public class BeforeFilter {
        public static bool Filter(string log,out string type,out string date,
            out string ip,out string jsonStr,out string eventValue,out string killer) {
            type = null;
            date = null;
            ip = null;
            jsonStr = null;
            eventValue = null;
            killer = null;
            ////  receive 85.97.191.69:52684 req {"event":"handshake"} 
            //if (log.IndexOf(" req ") > -1 && log.IndexOf("handshake") > -1) {
            //    return true;
            //}
            //send 85.97.191.69:52684 resp: {"rc":0,"payroll":{"key":"HQMLWBWXYECUUJME"}}
            if (log.IndexOf(" resp: ") > -1 && log.IndexOf("payroll") > -1 && log.IndexOf("key") > -1)
            {
                return true;
            }
            // send to eggserver tcpaddr 181.49.76.77:50068  httpaddr 10.58.241.179:49612 event checkupdate elapsedtime 1 resp code 200
            if (log.IndexOf("send to eggserver tcpaddr") > -1) {
                return true;
            }
            //send 181.49.76.77:50068 resp: {"rc":0,"st":1448494260,"today":"20151125"}
            if (log.IndexOf(" resp: ") > -1 && log.IndexOf("rc") > -1 && log.IndexOf("payroll") < 0) {
                return true;
            }
            //type

            if (log.IndexOf(" resp: ") > -1)
            {
                type = "resp";
            }
            else {
                type = "req";
            }


            //匹配日期
            Regex gex1 = new Regex(@"\d+\/\d+\/\d+ \d+\:\d+\:\d+");
            var m1 = gex1.Match(log);
            if (m1.Success) {
                date = m1.Value.Replace("/", "-");
            }
            //ip 
            Regex gex2 = new Regex(@"\d+\.\d+\.\d+\.\d+\:\d+");
            var m2 = gex2.Match(log);
            if (m2.Success)
            {
                ip = m2.Value.Split(new char[] { ':' })[0];
            }

            //
            Regex gex3 = new Regex(@"{([\S +|\s +])*");
            var m3 = gex3.Match(log);
            if (m3.Success)
            {
                jsonStr = m3.Value;
            }

            var rg = new RegexTool();
            var strt = rg.Substring(log, @"""kill"":""", @"""");
            if (!string.IsNullOrEmpty(strt))
            {
                killer = strt;
            }

            //var ch = rg.Substring(log, @"""channel"":""", @"""");
            //if (!string.IsNullOrEmpty(ch))
            //{
            //    channel = ch;
            //}
            //var vr = rg.Substring(log, @"""version"":""", @"""");
            //if (!string.IsNullOrEmpty(vr))
            //{
            //    version = vr;
            //}
            //var ll = rg.Substring(log, @"""locale"":""", @",");
            //if (!string.IsNullOrEmpty(ll))
            //{
            //    locale = int.Parse(ll) ;
            //}

            //var u = rg.Substring(log, @"""uid"":""", @"""");
            //if (!string.IsNullOrEmpty(u))
            //{
            //    uid = u;
            //}

            if (log.IndexOf(@"""handshake""") > -1) {
                eventValue = "handshake";
            } else
            if (log.IndexOf(@"""checkupdate""") > -1) {
                eventValue = "checkupdate";
            } else if (log.IndexOf(@"""checktask""") > -1)
            {
                eventValue = "checktask";
            }
            else if (log.IndexOf(@"""updateresult""") > -1)
            {
                eventValue = "updateresult";
            } else if (log.IndexOf(@"""taskresult""") > -1)
            {
                eventValue = "taskresult";
            }
            else if (log.IndexOf(@"""checkupdate2""") > -1)
            {
                eventValue = "checkupdate2";
            }
            else if (log.IndexOf(@"""host""") > -1 && type == "resp")
            {
                eventValue = "taskresponse";
            }

            return false;
        }
    }
}
