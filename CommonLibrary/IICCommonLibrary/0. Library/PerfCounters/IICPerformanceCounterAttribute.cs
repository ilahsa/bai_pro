using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class IICPerformanceCounterAttribute: Attribute
    {
        public IICPerformanceCounterAttribute(string counterName, PerformanceCounterType counterType)
            : this(counterName, counterType, string.Empty)
        {
        }

        public IICPerformanceCounterAttribute(string counterName, PerformanceCounterType counterType, string counterHelp)
        {
            CounterName = counterName;
            CounterType = counterType;
            CounterHelp = counterHelp;
        }

        public string CounterName
        {
            get;
            set;
        }

        public PerformanceCounterType CounterType
        {
            get;
            set;
        }

        public string CounterHelp
        {
            get;
            set;
        }
    }
}
