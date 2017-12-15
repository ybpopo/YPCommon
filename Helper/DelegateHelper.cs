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
        public delegate V GenericDel<T, V>(T value);

        /// <summary>
        /// 泛型委托
        /// </summary>
        /// <typeparam name="T">传入值类型</typeparam>
        /// <param name="value">传入值对象</param>
        public delegate void GenericDel<T>(T value);

        /// <summary>
        /// 委托
        /// </summary>
        public delegate void Generic();

        /// <summary>
        /// 异步委托
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static KeyValuePair<IAsyncResult, Generic> SyncDel(Action action)
        {
            var del = new Generic(action);
            var async = del.BeginInvoke(null, null);
            return new KeyValuePair<IAsyncResult, Generic>(async, del);
        }


    }
}
