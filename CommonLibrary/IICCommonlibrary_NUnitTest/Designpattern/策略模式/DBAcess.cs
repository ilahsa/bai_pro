using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IICCommonlibrary_NUnitTest.Designpattern.策略模式
{
    public class DBAcess
    {
        private IDB _idb;

        public void DBacess()
        {
            //这来可以根据配置文件 反射生成具体的 db操作类
            _idb = new MSDB();
        }

        public void Update()
        {
            _idb.Update();
        }

        public void Add()
        {
            _idb.Add();
        }

        public void Delete()
        {
            _idb.Delete();
        }
    }
}
