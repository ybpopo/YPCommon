using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace System
{
    /// <summary>
    /// string扩展方法
    /// </summary>
    public static class ExtString
    {
        /// <summary>
        /// 指定正则表达式匹配信息。
        /// </summary>
        /// <param name="str">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <returns>返回匹配到的内容</returns>
        public static Match ExtMatch(this string str, string pattern, RegexOptions options = RegexOptions.None)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(pattern)) return null;
            Match mc = Regex.Match(str, pattern, options);
            return mc;
        }

        /// <summary>
        /// 使用指定字符串拆分字符串
        /// </summary>
        /// <param name="str">
        /// 操作字符串
        /// </param>
        /// <param name="separator">
        /// 分隔此字符串中子字符串的字符串数组、不包含分隔符的空数组或 null。
        /// </param>
        /// <param name="options">
        /// 要省略返回的数组中的空数组元素，则为 System.StringSplitOptions.RemoveEmptyEntries；
        /// 要包含返回的数组中的空数组元素，则为System.StringSplitOptions.None。
        /// </param>
        /// <returns>返回一个字符串集合</returns>
        public static List<string> ExtSplit(this string str, string separator, StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(separator)) return new List<string>();
            string[] strArray = str.Split(new string[] { separator }, options);
            if (strArray == null || strArray.Length <= 0) return new List<string>();
            return strArray.ToList();
        }

        /// <summary>
        /// json 序列化
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="json">操作字符串</param>
        /// <returns>转换对象</returns>
        public static T ExtJsonToModel<T>(this string json)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize<T>(json);
        }
        
        /// <summary>
        /// 检测并创建文件路径
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void ExtCreatePath(this string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// 获取指定url的html
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ExtGetUrlHtml(this string url)
        {
            WebClient wc = new WebClient();
            return wc.DownloadString(url);
        }

    }
}
