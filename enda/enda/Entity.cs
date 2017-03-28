using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace enda
{
    [DataContract]
    public class Entity
    {
        //time0,str1,str2,str3,str4
        //这个type 区分类型 用了做业务判断
        [DataMember]
        public string type;
        [DataMember]
        public int time0;
        [DataMember]
        public int str1;
        [DataMember]
        public int str2;
        [DataMember]
        public int str3;
        [DataMember]
        public int str;
    }

  
}
