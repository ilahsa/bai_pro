using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IICCommonlibrary_NUnitTest.Designpattern.中介者
{
    /// <summary>
    /// 将具体的业务 对象 封装，对外隐藏
    /// </summary>
    public class Context
    {
        private Colleague _a;
        private Colleague _b;

        public Context()
        {
            _a = new AColleague();
            _b = new BColleague();
        }

        public void DealA()
        {
            _a.actino();
            _b.actino();
        }

        public void DealB()
        {
            _b.actino();
            _a.actino();
        }
    }
}
