using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IICCommonlibrary_NUnitTest.Designpattern.状态模式
{
    public class AStatus : IStatus
    {
        public string Operation(string str)
        {
            return string.Format("{0}_{1}", "AStatus", str);
        }
    }
}
