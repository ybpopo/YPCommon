using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YPCommon.Helper
{
    /// <summary>
    /// 委托帮助类
    /// </summary>
    public class DelegateHelper
    {
        /// <summary>
        /// 泛型委托
        /// </summary>
        /// <typeparam name="T">传入值类型</typeparam>
        /// <typeparam name="V">返回值类型</typeparam>
        /// <param name="value">传入值对象</param>
        /// <returns>返回值对象</returns>
        public delegate V AsyncDel<T, V>(T value);

        /// <summary>
        /// 泛型委托
        /// </summary>
        /// <typeparam name="T">传入值类型</typeparam>
        /// <param name="value">传入值对象</param>
        public delegate void AsyncDel<T>(T value);
    }
}
