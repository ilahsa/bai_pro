using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IICCommonlibrary_NUnitTest.Designpattern.模板方法
{
    public abstract class AbstractClass
    {
        public void TemplateMethod()
        {
            Method1();
            Method2();
            Method3();
        }

        public abstract void Method1();

        public abstract void Method2();

        private void Method3()
        {

        }  
    }
}
