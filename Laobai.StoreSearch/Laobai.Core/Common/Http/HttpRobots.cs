using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.IO;
using System.Net;

//using System.Net.NetworkInformation;
/// <summary>
/// Version: 1.0
/// Author: Atai Lu
/// </summary>
namespace Laobai.Core.Common.Http
{
    public class HttpRobots
    {
        public HttpRobots()
        {

        }
        #region 属性
        private bool _isError = false;
        /// <summary>
        /// 是否出错
        /// </summary>
        public bool IsError
        {
            get { return _isError; }
            //set { _isError = value; }
        }
        //
        private string _errorMsg = "";
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg
        {
            get { return _errorMsg; }
            //set { _errorMsg = value; }
        }
        //"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2526.106 Safari/537.36 (Atai Robots 1.0)";
        private string _UserAgent = "Mozilla/5.0 (compatible; Chrome/56.0.2526.106; Windows NT 10.0; Atai Robots ; .NET CLR 2.0.50727)";
        /// <summary>
        /// UserAgent
        /// </summary>
        public string UserAgent
        {
            get { return _UserAgent; }
            set { _UserAgent = value; }
        }
        private string _ContentType = "";
        /// <summary>
        /// 响应的内容类型
        /// </summary>
        public string ContentType
        {
            get { return _ContentType; }
        }
        private string _PageEncoding = "";
        public string PageEncoding
        {
            get { return _PageEncoding; }
        }

        private HttpStatusCode _HttpStatusCode;
        /// <summary>
        /// 远程服务器响应状态
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get { return _HttpStatusCode; }
        }
        #endregion

        #region 识别编码
        private string GetEncoding(string html)
        {
            string result = "utf-8";
            Regex reg_charset = new Regex(@"charset\b\s*=\s*(?<charset>[^""'\s]*)");

            if (reg_charset.IsMatch(html))
            {

                return reg_charset.Match(html).Groups["charset"].Value;

            }
            return result;
        }
        #endregion

        #region 抓取远程HTML
        public string GetHtml(string url, string charset="")
        {
            string result = "";
            string referer = url;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //声明一个HttpWebRequest请求
                request.Timeout = 30000;//设置连接超时时间
                request.UserAgent = this.UserAgent;
                request.AllowAutoRedirect = true;
                //
                request.Referer = referer;//设置标头
                //
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //HttpStatusCode status = ;
                this._HttpStatusCode = response.StatusCode;
                if (response.StatusCode == HttpStatusCode.OK && response.ContentLength < 1024 * 2048)//大于2M的文件不读取
                {
                    Stream streamReceive = response.GetResponseStream();//response.ContentType
                    //Stream streamReceive = Gzip(response);

                    //Encoding encoding = Encoding.Default;
                    StreamReader streamReader = null;
                    if (charset == null || charset == "")
                    {
                        streamReader = new StreamReader(streamReceive, Encoding.ASCII);
                        if (response.CharacterSet != string.Empty)
                            charset = response.CharacterSet;
                        else
                            charset = this.GetEncoding(streamReader.ReadToEnd());

                    }
                    this._PageEncoding = charset;
                    Encoding encoding = Encoding.GetEncoding(charset);//
                    streamReader = new StreamReader(streamReceive, encoding);

                    result = streamReader.ReadToEnd();
                    streamReader.Close();
                    streamReader.Dispose();
                    streamReceive.Close();
                    streamReceive.Dispose();
                }
                else
                {
                    this._isError = true; this._errorMsg = response.StatusCode.ToString();
                }
                this._ContentType = response.ContentType;
                response.Close();
            }
            catch (Exception e) { this._isError = true; this._errorMsg = e.ToString(); }
            return result;
        }
        #endregion

        #region 把特定数据Post到远程服务器并获取返回结果
        public string Post(string url, string postdata, string charset="")
        {
            string result = "";
            //string referer = url;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //声明一个HttpWebRequest请求
                request.Timeout = 30000;//设置连接超时时间
                request.UserAgent = this.UserAgent;
                request.AllowAutoRedirect = true;
                //
                //request.Referer = referer;//设置标头
                request.Method = "POST";
                //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                request.ContentType = "application/x-www-form-urlencoded";
                Encoding encoding = Encoding.GetEncoding(string.IsNullOrEmpty(charset) ? "gb2312" : charset);
                StreamWriter streamWriter = null;
                streamWriter = new StreamWriter(request.GetRequestStream(), encoding);
                streamWriter.Write(postdata);
                streamWriter.Close();
                //request.Headers.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //response.CharacterSet
                //HttpStatusCode status = ;
                //response.StatusCode
                if (response.StatusCode == HttpStatusCode.OK && response.ContentLength < 1024 * 2048)//大于2M的文件不读取
                {
                    Stream streamReceive = response.GetResponseStream();
                    //Stream streamReceive = Gzip(response);
                    this._PageEncoding = charset;
                    StreamReader streamReader = new StreamReader(streamReceive);
                    result = streamReader.ReadToEnd();
                    streamReader.Close();
                    streamReader.Dispose();
                    streamReceive.Close();
                    streamReceive.Dispose();
                }
                else
                {
                    this._isError = true; this._errorMsg = response.StatusCode.ToString();
                }
                this._ContentType = response.ContentType;
                response.Close();
            }
            catch (Exception e)
            {
                this._isError = true; this._errorMsg = e.ToString();
                Logger.Error(url + "\r\n" + postdata + "\r\n" + e.ToString());
            }
            return result;
        }
        #endregion

        #region 把文件传输到远程服务器指定页面
        public string PostFile(string url, System.Web.HttpPostedFile file, string charset)
        {
            string result = "";
            string referer = url;
            try
            {
                Encoding encoding = Encoding.GetEncoding(charset);//
                //FileStream fs = file.InputStream;
                BinaryReader r = new BinaryReader(file.InputStream);
                string strBoundary = "----------" + DateTime.Now.Ticks.ToString("x");//时间戳
                byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "\r\n");
                //头信息
                //string filePartHeader = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n"
                //    + "Content-Type: application/octet-stream\r\n\r\n";
                StringBuilder sb = new StringBuilder();
                sb.Append("--");
                sb.AppendLine(strBoundary);
                sb.Append("Content-Disposition: form-data; name=\"file\"; ");
                sb.AppendLine("filename=\"" + file.FileName + "\"");

                sb.Append("Content-Type: ");
                sb.Append("application/octet-stream");
                sb.AppendLine("");
                sb.AppendLine("");
                string strPostHeader = sb.ToString();
                byte[] postHeaderBytes = encoding.GetBytes(strPostHeader);

                //写入字符串

                string stringKeyHeader = "\r\n--" + strBoundary +
                           "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                           "\r\n\r\n{1}\r\n";
                

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 30000;//设置连接超时时间
                request.UserAgent = this.UserAgent;
                request.Referer = referer;//设置标头
                request.Method = "POST";
                request.ContentType = "multipart/form-data; boundary=" + strBoundary;
                long length = file.InputStream.Length + postHeaderBytes.Length + boundaryBytes.Length;
                request.ContentLength = length;

                byte[] byteArray = new byte[file.InputStream.Length];
                file.InputStream.Read(byteArray, 0, byteArray.Length);

                Stream stream = request.GetRequestStream();
                stream.Write(postHeaderBytes, 0, postHeaderBytes.Length);//头信息
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Write(boundaryBytes, 0, boundaryBytes.Length);//尾部时间戳

                stream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK && response.ContentLength < 1024 * 2048)//大于2M的文件不读取
                {
                    Stream streamReceive = response.GetResponseStream();
                    //Stream streamReceive = Gzip(response);
                    this._PageEncoding = charset;
                    StreamReader streamReader = new StreamReader(streamReceive, encoding);
                    result = streamReader.ReadToEnd();
                    streamReader.Close();
                    streamReader.Dispose();
                    streamReceive.Close();
                    streamReceive.Dispose();
                }
                else
                {
                    this._isError = true; this._errorMsg = response.StatusCode.ToString();
                }
                this._ContentType = response.ContentType;
                response.Close();

            }
            catch (Exception e) { this._isError = true; this._errorMsg = e.ToString(); }
            return result;
        }
        #endregion

        #region Ping
        public static System.Net.NetworkInformation.IPStatus Ping(string ip)
        {
            System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
            options.DontFragment = true;
            string data = "atai";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            //Wait seconds for a reply. 
            int timeout = 1000;
            System.Net.NetworkInformation.PingReply reply = p.Send(ip, timeout, buffer, options);
            return reply.Status;

        }
        #endregion

        #region HttpPost
        public string HttpPost(string url, string postData)
        {
            string responseStr = string.Empty;
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = "POST";
                request.Timeout = 30000;
                request.ContentType = "application/x-www-form-urlencoded;charset=gb2312";

                WebResponse webResponse = null;
                StreamWriter streamWriter = null;
                Encoding myEncoding = Encoding.GetEncoding("gb2312");
                try
                {
                    streamWriter = new StreamWriter(request.GetRequestStream(), myEncoding);

                    streamWriter.Write(postData);
                    streamWriter.Close();

                    webResponse = request.GetResponse();
                    if (webResponse != null)
                    {
                        StreamReader streamReader = new StreamReader(webResponse.GetResponseStream());
                        responseStr = streamReader.ReadToEnd();
                        streamReader.Close();
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                }
            }
            catch (Exception e)
            {
                Logger.Error(url + "\n" + e.ToString());
            }
            return responseStr;
        }
        #endregion
    }
}
