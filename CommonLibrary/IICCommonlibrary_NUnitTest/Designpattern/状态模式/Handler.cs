using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IICCommonlibrary_NUnitTest.Designpattern.状态模式
{
    class Handler
    {
        public void TT()
        {
            Context context = new Context();
            context.SetStatus(new AStatus());
            context.Operation("123");
            context.SetStatus(new BStatus());
            context.Operation("456");
        }
    }
}
