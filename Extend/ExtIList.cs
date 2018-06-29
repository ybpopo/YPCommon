using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class ExtIList
    {
        /// <summary>
        /// 判断集合是否为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool ExtIsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count <= 0;
        }
    }
}
