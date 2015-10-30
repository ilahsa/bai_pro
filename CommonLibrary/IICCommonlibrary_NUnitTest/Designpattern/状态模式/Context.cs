using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IICCommonlibrary_NUnitTest.Designpattern.状态模式
{
    public class Context
    {
        private IStatus _status;

        public string Operation(string str)
        {
            return _status.Operation(str);
        }

        public void SetStatus(IStatus status)
        {
            _status = status;
        }

        public IStatus GetStatus()
        {
            return _status;
        }

    }
}
