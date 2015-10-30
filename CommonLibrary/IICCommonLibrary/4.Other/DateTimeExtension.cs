using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 获取 dt 的时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int GetTimestamp(this DateTime dt)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(dt - startTime).TotalSeconds;
        }
    }
}
