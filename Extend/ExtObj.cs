using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace System
{
    /// <summary>
    /// Object扩展类
    /// </summary>
    public static class ExtObj
    {
        /// <summary>
        /// 单个对象转Json
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">操作对象</param>
        /// <returns>Json字符串</returns>
        public static string ExtToJson<T>(this T obj)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(obj);
        }

        /// <summary>
        /// 复制本对象属性值到新对象
        /// </summary>
        /// <param name="source">操作对象</param>
        /// <param name="target">目标对象</param>
        public static void ExtCopyProperties(this object source, object target)
        {
            Type sourceType = source.GetType(), targetType = target.GetType();
            PropertyInfo[] sourceProperties = sourceType.GetProperties(), targetProperties = targetType.GetProperties();
            foreach (var s in sourceProperties)
            {
                foreach (var t in targetProperties)
                {
                    if (s.Name == t.Name && s.CanRead && t.CanWrite && s.PropertyType == t.PropertyType)
                    {
                        var sourceValue = s.GetValue(source, null);
                        t.SetValue(target, sourceValue, null);
                    }
                }
            }
        }

        /// <summary>
        /// 给对象属性附值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void ExtSetPropertyValue(this object obj, string propertyName, object value)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property != null) property.SetValue(obj, value, null);
        }

        /// <summary>
        /// 获取所有枚举项描述信息
        /// </summary>
        /// <param name="obj">枚举对象</param>
        /// <returns></returns>
        public static string[] ExtGetEnumDescriptions(this Type obj)
        {
            FieldInfo[] fieldinfos = obj.GetFields();
            List<string> enumDescription = new List<string>();
            foreach (FieldInfo field in fieldinfos)
            {
                if (field.FieldType.IsEnum)
                {
                    Object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    string value = objs != null && objs.Length > 0 ? ((DescriptionAttribute)objs[0]).Description : string.Empty;
                    enumDescription.Add(value);
                }
            }
            return enumDescription.ToArray();
        }
    }
}
