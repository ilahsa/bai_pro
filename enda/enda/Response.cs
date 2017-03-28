using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace enda
{
    [DataContract]
    public  class Response
    {
        [DataMember]
        public int rc;
        [DataMember]
        public data data;
    }

    [DataContract]
    public class data {
        [DataMember]
        public string r1;
        [DataMember]
        public string r2;
    }
}
