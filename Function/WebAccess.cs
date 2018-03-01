using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace YPCommon.Function
{
    /// <summary>
    /// web访问类
    /// </summary>
    public class WebAccess
    {
        /// <summary>
        /// GetUrlHtml
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>html</returns>
        public static string GetUrlHtml(string url)
        {
            return GetUrlHtml(url, "Get", null, null, null);
        }

        /// <summary>
        /// GetUrlHtml
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="method">method</param>
        /// <returns>html</returns>
        public static string GetUrlHtml(string url, string method)
        {
            return GetUrlHtml(url, method, null, null, null);
        }

        /// <summary>
        /// GetUrlHtml
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="method">method</param>
        /// <param name="encoding">encoding</param>
        /// <returns>html</returns>
        public static string GetUrlHtml(string url, string method, Encoding encoding)
        {
            return GetUrlHtml(url, method, encoding, null, null);
        }

        /// <summary>
        /// GetUrlHtml
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="method">method</param>
        /// <param name="encoding">encoding</param>
        /// <param name="dataStr">dataStr</param>
        /// <returns>html</returns>
        public static string GetUrlHtml(string url, string method, Encoding encoding, string dataStr)
        {
            return GetUrlHtml(url, method, encoding, dataStr, null);
        }

        /// <summary>
        /// GetUrlHtml
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="method">method</param>
        /// <param name="encoding">encoding</param>
        /// <param name="dataStr">dataStr</param>
        /// <param name="cookie">cookie</param>
        /// <returns>html</returns>
        public static string GetUrlHtml(string url, string method, Encoding encoding, string dataStr, CookieContainer cookie)
        {
            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(url));
                httpReq.Method = method;
                if (cookie != null) httpReq.CookieContainer = cookie;
                if (encoding == null) encoding = Encoding.UTF8;
                if (!string.IsNullOrEmpty(dataStr))
                {
                    Stream myRequestStream = httpReq.GetRequestStream();
                    StreamWriter myStreamWriter = new StreamWriter(myRequestStream, encoding);
                    myStreamWriter.Write(dataStr);
                    myStreamWriter.Close();
                }
                HttpWebResponse response = (HttpWebResponse)httpReq.GetResponse();
                if (response.ContentEncoding.ToLower() == "gzip")//如果使用了GZip则先解压
                {
                    using (System.IO.Stream streamReceive = response.GetResponseStream())
                    {
                        using (var zipStream = new System.IO.Compression.GZipStream(streamReceive, System.IO.Compression.CompressionMode.Decompress))
                        {
                            using (StreamReader sr = new StreamReader(zipStream, encoding))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
                else
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), encoding);
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        /// <summary>
        /// 将cookie添加到CookieContainer中
        /// </summary>
        /// <param name="cookies">cookie字符串</param>
        /// <param name="domain">cookie的域</param>
        /// <param name="cookieContainer">CookieContainer</param>
        public static void AddCookies(string cookies, string domain, ref CookieContainer cookieContainer)
        {
            if (string.IsNullOrEmpty(cookies))
                return;

            if (cookieContainer == null)
                cookieContainer = new CookieContainer();

            var cookieDict = GetCookieDictionary(cookies);
            var cookieUri = GetUri(domain);

            var cookieCollection = cookieContainer.GetCookies(cookieUri);
            if (cookieDict.Count > 0)
            {
                if (cookieDict.Count > 20 && cookieDict.Count < 50) cookieContainer.PerDomainCapacity = cookieDict.Count;
                foreach (var key in cookieDict.Keys)
                {
                    var isExist = false;
                    foreach (Cookie cookie in cookieCollection)
                    {
                        if (cookie.Name == key)
                        {
                            isExist = true;
                            cookie.Domain = domain;
                            cookie.Value = cookieDict[key];
                            break;
                        }
                    }

                    if (!isExist)
                    {
                        try
                        {
                            var ck = new Cookie(key, cookieDict[key]) { Domain = domain };
                            cookieContainer.Add(ck);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取cookie信息字典
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetCookieDictionary(string cookies)
        {
            var cookieDict = new Dictionary<string, string>();
            var cookstr = cookies.Split(';');
            foreach (var str in cookstr)
            {
                if (str.Contains("="))
                {
                    var index = str.IndexOf("=", StringComparison.Ordinal);
                    var cname = str.Substring(0, index).Trim();
                    var value = str.Substring(index + 1, str.Length - index - 1).Trim();
                    if (!string.IsNullOrEmpty(cname) && !cname.StartsWith("$"))
                    {
                        if (!cookieDict.ContainsKey(cname))
                        {
                            cookieDict.Add(cname, value);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(cookieDict[cname]) && !string.IsNullOrEmpty(value))
                            {
                                cookieDict[cname] = value;
                            }
                        }
                    }
                }
            }
            return cookieDict;
        }

        /// <summary>
        /// 获取匹配的域名uri
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        private static Uri GetUri(string domain)
        {
            Uri cookieUri = domain.StartsWith("http://") ?
                new Uri(domain) : domain.StartsWith(".") ?
                new Uri(string.Format("http://www{0}", domain)) :
                new Uri(string.Format("http://{0}", domain));
            return cookieUri;
        }
    }
}
