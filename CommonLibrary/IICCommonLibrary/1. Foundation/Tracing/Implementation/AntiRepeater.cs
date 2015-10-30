//
// NextVersion
//
//using System;
//using System.Threading;
//using System.Diagnostics;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Imps.Services.CommonV4.Tracing
//{
//    public static class AntiRepeater
//    {
//        private static SafeDictionary<long, ComboClass<int, int>> _repeaters = new SafeDictionary<long, ComboClass<int, int>>();

//        public static bool IsRepeated(out int repeat)
//        {
//            long offset = GetCallStackOffset();
//            ComboClass<int, int> r;
//            if (!_repeaters.TryGetValue(offset, out r)) {
//                int tick = Environment.TickCount;
//                r = new ComboClass<int, int>(tick, 1);
//                _repeaters[offset] = r;
//                repeat = 1;
//                return false;
//            } else {
//                int elapsed = Environment.TickCount - r.V1;
//                if (elapsed > 5000) {
//                    repeat = r.V2;
//                    _repeaters.Remove(offset);
//                    return false;
//                } else {
//                    Interlocked.Increment(ref r.V2);
//                    repeat = r.V2;
//                    return true;
//                }
//            }
//        }

//        private static long GetCallStackOffset()
//        {
//            StackTrace trace = new StackTrace(0, false);
//            //
//            // AntiRepeater.GetCallStackOffSet
//            // AntiRepeater.IsRepeated
//            // SystemLog.WriteEventLog
//            // SystemLog.Info
//            // --> CallerMethod
//            StackFrame frame = trace.GetFrame(4);
//            return (frame.GetMethod().MetadataToken << 32) | frame.GetILOffset();
//        }
//    }
//}
