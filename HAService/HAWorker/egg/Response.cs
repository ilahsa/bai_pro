using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HAWorker.egg
{
    [DataContract]
    public class Response<T>
    {
        [DataMember]
        public int rc;

        [DataMember]
        public T payroll;
    }

    public class Payroll_HandkShake {
        [DataMember]
        public string key;
    }
}
