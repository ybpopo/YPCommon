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

    }
}
