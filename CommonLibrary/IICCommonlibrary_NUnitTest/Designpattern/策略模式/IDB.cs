using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IICCommonlibrary_NUnitTest.Designpattern.策略模式
{
    public interface IDB
    {
        void Update();

        void Add();

        void Delete();
    }
}
