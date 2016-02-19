using Imps.Services.CommonV4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchInsertTool
{


    public class EntityBase
    {
        [TableField("eggid")]
        public int eggid;
        [TableField("uid")]
        public string uid;
        [TableField("sid")]
        public string sid;
        [TableField("hid")]
        public string hid;
        [TableField("sysid")]
        public string sysid;
        [TableField("vid")]
        public Int64 vid;
        [TableField("ip")]
        public string ip;
    }

    public class Alivelog : EntityBase
    {
        [TableField("alivetime")]
        public DateTime alivetime;
        [TableField("locale")]
        public int locale;
        [TableField("version")]
        public string version;
        [TableField("killer")]
        public string kill;
        [TableField("channel")]
        public string channel;

        [TableField("event")]
        public string Event;

        public data data;

        [TableField("result")]
        public Int64 result;

    }

    public class data {
        public Int64 result;
    }
    public class Tasklog : EntityBase {
        [TableField("taskid")]
        public int taskid;
        [TableField("taskreturn")]
        public int taskreturn;
        [TableField("parameter")]
        public string parameter;
        [TableField("str1")]
        public string str1;
        [TableField("str2")]
        public string str2;
        [TableField("data")]
        public string data;
        [TableField("datamd5")]
        public string datamd5;
        [TableField("tasktime")]
        public DateTime tasktime;
    }



    public class Clientinfo : EntityBase {

        public string channel;
        public int locale;
        public string os;
        public bool amd64;
        public string data;
        public string datamd5;
        public DateTime createtime;
        public DateTime updatetime;
    }
}
