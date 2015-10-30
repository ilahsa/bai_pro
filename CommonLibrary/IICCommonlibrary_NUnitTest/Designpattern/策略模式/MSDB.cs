using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IICCommonlibrary_NUnitTest.Designpattern.策略模式
{
    public class MSDB:IDB
    {
        public void Update()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Add()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Delete()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
