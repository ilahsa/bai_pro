using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IICCommonlibrary_NUnitTest.Designpattern.状态模式
{
    public interface IStatus
    {
        string Operation(string str);
    }
}
