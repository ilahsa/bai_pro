using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Imps.Services.CommonV4;

namespace StatDau
{
    public class commonlog
    {
        public string uuid;
        public string Event;

        [TableField("str1")]
        public string str1;
        [TableField("str2")]
        public string str2;
        [TableField("str3")]
        public string str3;
        [TableField("str4")]
        public string str4;
        [TableField("str5")]
        public string str5;
        [TableField("str6")]
        public string str6;
        [TableField("str7")]
        public string str7;
        [TableField("str8")]
        public string str8;
        [TableField("str9")]
        public string str9;
        [TableField("str10")]
        public string str10;

        [TableField("int1")]
        public Int64 int1;
        [TableField("int2")]
        public Int64 int2;
        [TableField("int3")]
        public Int64 int3;
        [TableField("int4")]
        public Int64 int4;
        [TableField("int5")]
        public Int64 int5;
        [TableField("int6")]
        public Int64 int6;

    }
}
