using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// Double扩展类
    /// </summary>
    public static class ExtDouble
    {
        /// <summary>
        /// 将时间戳转为C#格式时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime GetTime(this double value)
        {
            DateTime time = DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return startTime.AddMilliseconds(value);
        }
    }
}
