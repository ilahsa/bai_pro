using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Imps.Services.CommonV4
{
    /// <summary>
    /// task ex
    /// </summary>
    static public class TaskDelay
    {
        /// <summary>
        /// 延迟执行某个动作
        /// </summary>
        /// <param name="dueTime"></param>
        /// <param name="callback"></param>
        /// <exception cref="ArgumentOutOfRangeException">dueTime</exception>
        /// <exception cref="ArgumentNullException">callback is null</exception>
        public static void Delay(int dueTime, Action callback)
        {
            if (dueTime < -1) throw new ArgumentOutOfRangeException("dueTime");
            if (callback == null) throw new ArgumentNullException("callback");

            if (dueTime == 0)
            {
                try { callback(); }
                catch { }
            }

            Timer timer = null;
            timer = new Timer(_ =>
            {
                timer.Dispose();
                try { callback(); }
                catch { }
            }, null, dueTime, Timeout.Infinite);
        }
    }
}
