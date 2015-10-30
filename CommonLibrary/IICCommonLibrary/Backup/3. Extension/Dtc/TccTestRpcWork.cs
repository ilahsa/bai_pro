//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Imps.Services.CommonV4;
//using Imps.Services.CommonV4.Dtc;

//namespace UnitTest.Dtc
//{
//    class TccTestRpcWork: TccRpcWorkUnit<TccTestContext, int, int>
//    {
//        public TccTestRpcWork(string name)
//            : base(name)
//        {
//        }

//        protected override ResolvableUri GetUri(TccTestContext context)
//        {
//            // return 
//            // return new InprocUri();
//            return new ServiceUri("UID");
//        }
	
//        protected override int ConvertArgs(TccTestContext context)
//        {
//            return context.UserId;
//        }

//        protected override void  ConvertResults(int results, TccTestContext context)
//        {
//            context.Sid = results;
//        }
//    }
//}
