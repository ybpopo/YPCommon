using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace System.Data
{
    /// <summary>
    /// DataRow扩展类型
    /// </summary>
    public static class ExtDataRow
    {
        /// <summary>
        /// 将DataRow转为对应的数据对象
        /// </summary>
        /// <typeparam name="T">数据对象</typeparam>
        /// <param name="dr">数据行</param>
        /// <returns>数据对象</returns>
        public static T ExtGetModel<T>(this DataRow dr) where T : new()
        {
            T model = new T();
            foreach (PropertyInfo pInfo in model.GetType().GetProperties())
            {
                object val = getValueByColumnName(dr, pInfo.Name);
                pInfo.SetValue(model, val, null);
            }
            return model;
        }

        /// <summary>
        /// 返回DataRow 中对应的列的值。  
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        private static object getValueByColumnName(DataRow dr, string columnName)
        {
            if (dr.Table.Columns.IndexOf(columnName) >= 0)
            {
                if (dr[columnName] == DBNull.Value)
                    return null;
                return dr[columnName];
            }
            return null;
        }
    }
}
