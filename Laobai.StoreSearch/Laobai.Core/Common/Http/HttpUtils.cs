using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.Web.SessionState;

namespace Laobai.Core.Common.Http
{
    /// <summary>
    /// HTTP响应
    /// Version: 1.0
    /// Author: Atai Lu
    /// </summary>
    [Serializable]
    public class HttpUtils
    {
        #region RootPath
        /// <summary>
        /// 获取根路径
        /// </summary>
        public static string RootPath
        {
            get
            {
                return HttpContext.Current.Request.ApplicationPath;
            }
        }
        #endregion

        #region GetParameter
        /// <summary>
        /// 获取URL参数
        /// </summary>
        /// <param name="mode">GET或者POST</param>
        /// <param name="key"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        private static string GetParameter(string mode, string key, bool replace = true)
        {
            if (string.IsNullOrEmpty(mode))  return "";

            string result = "";
            if (mode.ToLower().Equals("get"))
                result = HttpContext.Current.Request.QueryString[key];
            else if (mode.ToLower().Equals("post"))
                result = HttpContext.Current.Request.Form[key];
            else
                result = HttpContext.Current.Request[key];
            if (string.IsNullOrEmpty(result))
                return "";

            if (replace)
            {
                result = Utils.HtmlEncode(result);
            }
            return result;
        }
        #endregion

        #region UserAgent
        /// <summary>
        /// UserAgent
        /// </summary>
        public static string UserAgent
        {
            get
            {
                return string.IsNullOrEmpty(HttpContext.Current.Request.UserAgent) ? "" : HttpContext.Current.Request.UserAgent;
            }
        }
        #endregion
        //
        #region Path
        /// <summary>
        /// 获取当前请求的虚拟路径
        /// </summary>
        public static string Path
        {
            get
            {
                return HttpContext.Current.Request.Path;
            }
        }
        #endregion
        //
        #region IsPost
        /// <summary>
        /// 判断当前页面是否接收到了Post请求
        /// </summary>
        /// <returns>是否接收到了Post请求</returns>
        public static bool IsPost
        {
            get
            {
                return HttpContext.Current.Request.HttpMethod.Equals("POST");
            }
        }
        #endregion

        #region IsGet
        /// <summary>
        /// 判断当前页面是否接收到了Get请求
        /// </summary>
        /// <returns>是否接收到了Get请求</returns>
        public static bool IsGet
        {
            get
            {
                return HttpContext.Current.Request.HttpMethod.Equals("GET");
            }
        }
        #endregion

        #region GetServerString
        /// <summary>
        /// 返回指定的服务器变量信息
        /// </summary>
        /// <param name="strName">服务器变量名</param>
        /// <returns>服务器变量信息</returns>
        public static string GetServerString(string strName)
        {
            if (HttpContext.Current.Request.ServerVariables[strName] == null)
            {
                return "";
            }
            return HttpContext.Current.Request.ServerVariables[strName].ToString().Trim();
        }
        #endregion

        #region UrlReferrer
        /// <summary>
        /// 返回上一个页面的地址
        /// </summary>
        /// <returns>上一个页面的地址</returns>
        public static string UrlReferrer
        {
            get
            {
                string retVal = "";
                try
                {
                    retVal = HttpContext.Current.Request.UrlReferrer.ToString().Trim();
                }
                catch { }
                return retVal;
            }

        }
        #endregion

        #region CurrentFullHost
        /// <summary>
        /// 得到当前完整主机头
        /// </summary>
        /// <returns></returns>
        public static string CurrentFullHost
        {
            get
            {
                HttpRequest Request = HttpContext.Current.Request;
                if (!Request.Url.IsDefaultPort)
                {
                    return string.Format("{0}:{1}", Request.Url.Host, Request.Url.Port.ToString().Trim());
                }
                return Request.Url.Host;
            }
        }
        #endregion

        #region Host
        /// <summary>
        /// 得到主机头
        /// </summary>
        /// <returns></returns>
        public static string Host
        {
            get
            {
                return HttpContext.Current.Request.Url.Host;
            }
        }
        #endregion

        #region RawUrl
        /// <summary>
        /// 获取当前请求的原始 URL(URL 中域信息之后的部分,包括查询字符串(如果存在))
        /// </summary>
        /// <returns>原始 URL</returns>
        public static string RawUrl
        {
            get
            {
                return HttpContext.Current.Request.RawUrl;
            }
        }
        #endregion

        #region Url
        /// <summary>
        /// 获得当前完整Url地址
        /// </summary>
        /// <returns>当前完整Url地址</returns>
        public static string Url
        {
            get
            {
                return HttpContext.Current.Request.Url.ToString().Trim();
            }
        }
        #endregion

        #region PageName
        /// <summary>
        /// 获得当前页面的名称
        /// </summary>
        /// <returns>当前页面的名称</returns>
        public static string PageName
        {
            get
            {
                string[] urlArr = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
                return urlArr[urlArr.Length - 1].ToLower();
            }
        }
        #endregion

        #region GetParamCount
        /// <summary>
        /// 返回表单或Url参数的总个数
        /// </summary>
        /// <returns></returns>
        public static int GetParamCount()
        {
            return (HttpContext.Current.Request.Form.Count + HttpContext.Current.Request.QueryString.Count);
        }
        #endregion

        #region ClientIP
        /// <summary>
        /// 获得当前页面客户端的外网IP
        /// </summary>
        /// <returns>当前页面客户端的外网IP</returns>
        public static string ClientIP
        {
            get
            {
                string UserIP = "127.0.0.1";
                try
                {
                    if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                    {
                        if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null)
                        {
                            if (HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"] != null)
                                UserIP = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];
                            else if (HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null)
                                UserIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        }
                        else
                        {
                            UserIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                        }
                    }
                    else
                        UserIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    if (string.IsNullOrEmpty(UserIP))
                        UserIP = HttpContext.Current.Request.UserHostAddress;
                }
                catch (Exception)
                {

                }
                return string.IsNullOrEmpty(UserIP) ? "127.0.0.1" : UserIP.Trim();
            }
        }
        #endregion

        #region IsCrossSitePost
        /// <summary>
        /// 返回当前页面是否是跨站提交(Post方式)
        /// </summary>
        /// <returns>当前页面是否是跨站提交</returns>
        public static bool IsCrossSitePost()
        {
            // 如果不是提交则为true
            if (!IsPost)
            {
                return true;
            }
            return IsCrossSitePost(UrlReferrer, Host);
        }
        /// <summary>
        /// 返回当前页面是否是跨站提交(Get方式)
        /// </summary>
        /// <returns>当前页面是否是跨站提交</returns>
        public static bool IsCrossSiteGet()
        {
            return IsCrossSitePost(UrlReferrer, Host);
        }
        /// <summary>
        /// 判断是否是跨站提交
        /// </summary>
        /// <param name="urlReferrer">上个页面地址</param>
        /// <param name="host">网站url</param>
        /// <returns></returns>
        public static bool IsCrossSitePost(string urlReferrer, string host)
        {
            if (urlReferrer.Length < 7)
            {
                return true;
            }
            // 移除http://
            string tmpReferrer = urlReferrer.Remove(0, 7);
            if (tmpReferrer.IndexOf(":") > -1)
                tmpReferrer = tmpReferrer.Substring(0, tmpReferrer.IndexOf(":"));
            else
                tmpReferrer = tmpReferrer.Substring(0, tmpReferrer.IndexOf('/'));
            return tmpReferrer != host;
        }
        #endregion
        //
        #region GoToReferrerPage
        /// <summary>
        /// 返回来源页面
        /// </summary>
        public static void GoToReferrerPage()
        {
            GoToReferrerPage("~/");
        }
        //
        public static void GoToReferrerPage(string def)
        {
            string url = def;
            if (HttpUtils.UrlReferrer != "")
                url = HttpUtils.UrlReferrer;
            HttpContext.Current.Response.Redirect(url);
        }
        #endregion

        #region Request
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="replace">是否进行HTML编码</param>
        /// <returns></returns>
        public static string Request(string key, bool replace = true)
        {
            return GetParameter("", key, replace);
        }
        #endregion

        #region RequestDate
        /// <summary>
        /// 转日期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static DateTime RequestDate(string key, DateTime defValue)
        {
            return Utils.StrToDateTime(Request(key), defValue);
        }
        public static DateTime RequestDate(string key)
        {
            return RequestDate(key, DateTime.Parse("1901-01-01 00:00:00"));
        }
        #endregion

        #region GetQueryString(Get)
        /// <summary>
        /// 获取URL参数(Get)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="replace">是否进行HTML编码</param>
        /// <returns></returns>
        public static string GetQueryString(string key, bool replace = true)
        {
            return GetParameter("get", key, replace);
        }
        #endregion

        #region GetQueryDate(Get)
        /// <summary>
        /// 参数转DateTime
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static DateTime GetQueryDate(string key, DateTime defValue)
        {
            return Utils.StrToDateTime(GetQueryString(key), defValue);
        }
        public static DateTime GetQueryDate(string key)
        {
            return GetQueryDate(key, DateTime.Parse("1900-01-01 00:00:00"));
        }
        #endregion

        #region GetFormString(Post)
        /// <summary>
        /// 获取表单参数(Post)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="replace">是否进行HTML编码</param>
        /// <returns></returns>
        public static string GetFormString(string key, bool replace = true)
        {
            return GetParameter("post", key, replace);
        }
        #endregion

        #region GetFormDate(Post)
        /// <summary>
        /// 获取日期类型(Post)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static DateTime GetFormDate(string key, DateTime defValue)
        {
            return Utils.StrToDateTime(GetFormString(key), defValue);
        }
        public static DateTime GetFormDate(string key)
        {
            return GetFormDate(key, DateTime.Parse("1901-01-01 00:00:00"));
        }
        #endregion
        //
        #region RequestInt
        public static int RequestInt(string key)
        {
            return Utils.StrToInt(HttpUtils.Request(key), 0);
        }
        //
        public static int RequestInt(string key, int def)
        {
            return Utils.StrToInt(HttpUtils.Request(key), def);
        }
        #endregion

        #region RequestLong
        public static long RequestLong(string key)
        {
            return RequestLong(key, 0);
        }
        public static long RequestLong(string key, long def)
        {
            string val = HttpUtils.Request(key);
            if (Utils.IsLong(val))
                return long.Parse(val);

            return def;
        }
        #endregion

        #region GetQueryInt
        public static int GetQueryInt(string key)
        {
            return Utils.StrToInt(HttpUtils.GetQueryString(key), 0);
        }
        public static int GetQueryInt(string key, int def)
        {
            return Utils.StrToInt(HttpUtils.GetQueryString(key), def);
        }
        #endregion

        #region GetQueryLong
        public static long GetQueryLong(string key)
        {
            return GetQueryLong(key, 0);
        }
        public static long GetQueryLong(string key, long def)
        {
            string val = HttpUtils.GetQueryString(key);
            if (Utils.IsLong(val))
                return long.Parse(val);

            return def;
        }
        #endregion

        #region GetQueryFloat
        public static float GetQueryFloat(string key)
        {
            return GetQueryFloat(key, 0f);
        }
        public static float GetQueryFloat(string key, float def)
        {
            string val = HttpUtils.GetQueryString(key);
            if (Utils.IsNumber(val))
                return float.Parse(val);
            return def;
        }
        #endregion

        #region GetFormInt
        public static int GetFormInt(string key)
        {
            return Utils.StrToInt(HttpUtils.GetFormString(key), 0);
        }
        public static int GetFormInt(string key, int def)
        {
            return Utils.StrToInt(HttpUtils.GetFormString(key), def);
        }
        #endregion

        #region GetFormLong
        public static long GetFormLong(string key)
        {
            return GetFormLong(key, 0);
        }
        public static long GetFormLong(string key, long def)
        {
            string val = HttpUtils.GetFormString(key);
            if (Utils.IsLong(val))
                return long.Parse(val);

            return def;
        }
        #endregion

        #region GetFormFloat
        public static float GetFormFloat(string key)
        {
            return GetFormFloat(key, 0f);
        }
        public static float GetFormFloat(string key, float def)
        {
            string val = HttpUtils.GetFormString(key);
            if (Utils.IsNumber(val))
                return float.Parse(val);
            return def;
        }
        #endregion

        #region GetFormDecimal
        public static decimal GetFormDecimal(string key)
        {
            return GetFormDecimal(key, 0m);
        }
        public static decimal GetFormDecimal(string key, decimal def)
        {
            string val = HttpUtils.GetFormString(key);
            if (Utils.IsNumber(val))
                return decimal.Parse(val);
            return def;
        }
        #endregion

        #region RequestGuid
        public static Guid RequestGuid(string key, Guid def)
        {
            try
            {
                if (Utils.IsMatch(HttpUtils.Request(key), @"^[a-zA-Z\d]{8}-[a-zA-Z\d]{4}-[a-zA-Z\d]{4}-[a-zA-Z\d]{4}-[a-zA-Z\d]{12}$"))
                    return new Guid(HttpUtils.Request(key));
                else
                    return def;
            }
            catch
            {
                return def;
            }
        }
        public static Guid RequestGuid(string key)
        {
            return RequestGuid(key, Guid.Empty);
        }
        #endregion

        #region GetQueryGuid
        public static Guid GetQueryGuid(string key, Guid def)
        {
            try
            {
                if (Utils.IsMatch(HttpUtils.GetQueryString(key), @"^[a-zA-Z\d]{8}-[a-zA-Z\d]{4}-[a-zA-Z\d]{4}-[a-zA-Z\d]{4}-[a-zA-Z\d]{12}$"))
                    return new Guid(HttpUtils.GetQueryString(key));
                else
                    return def;
            }
            catch
            {
                return def;
            }
        }
        public static Guid GetQueryGuid(string key)
        {
            return GetQueryGuid(key, new Guid());
        }
        #endregion

        #region GetFormGuid
        public static Guid GetFormGuid(string key, Guid def)
        {
            try
            {
                if (Utils.IsMatch(HttpUtils.GetFormString(key), @"^[a-zA-Z\d]{8}-[a-zA-Z\d]{4}-[a-zA-Z\d]{4}-[a-zA-Z\d]{4}-[a-zA-Z\d]{12}$"))
                    return new Guid(HttpUtils.GetFormString(key));
                else
                    return def;
            }
            catch
            {
                return def;
            }
        }
        public static Guid GetFormGuid(string key)
        {
            return GetFormGuid(key, new Guid());
        }
        #endregion
        //
        #region 设置Cookie
        /// <summary>
        /// 设置cookie
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expires">过期时间</param>
        /// <param name="expiresType">b:关闭浏览器时候cookie过期; d:过期单位为天; h:过期单位为小时; m:过期单位为分</param>
        /// <param name="domain">如果不为空,则设置Cookies的作用域为指定域名下所有子域名</param>
        public static void SetCookie(string key, string value, int expires, string expiresType, string domain)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
            if (cookie == null)
            {
                cookie = new HttpCookie(key);
            }

            cookie.Value = UrlEncode(value);
            expiresType = expiresType.ToLower();
            if (expires > 0)
            {
                switch (expiresType)
                {
                    case "d":
                        cookie.Expires = DateTime.Now.AddDays(expires);
                        break;
                    case "h":
                        cookie.Expires = DateTime.Now.AddHours(expires);
                        break;
                    case "m":
                        cookie.Expires = DateTime.Now.AddMinutes(expires);
                        break;
                    default:
                        break;
                }
            }

            if (domain.Trim() != "")
                cookie.Domain = domain.Trim();//设置Cookies的作用域为ccdodo.com下所有子域名

            HttpContext.Current.Response.AppendCookie(cookie);
        }
        #endregion
        //
        #region GetCookie
        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static string GetCookie(string key)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[key] != null)
            {
                return UrlDecode(HttpContext.Current.Request.Cookies[key].Value.ToString().Trim());
            }
            return "";
        }
        #endregion

        #region GetMapPath
        /// <summary>
        /// 根据虚拟路径，得到物理路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetMapPath(string path)
        {
            if (path == null || path == "")
                return "";
            return HttpContext.Current.Server.MapPath(path);
        }
        #endregion

        #region 返回 URL 字符串的编码结果
        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }
        public static string UrlEncode(string str, string charset)
        {
            Encoding ec = Encoding.UTF8;
            return HttpUtility.UrlEncode(str, Encoding.GetEncoding(charset));
        }
        #endregion

        #region 返回 URL 字符串的编码结果
        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }
        #endregion

        #region Md5加密,与Utils.MD5不同
        /// <summary>
        /// Md5加密,与Atai.Common.Utils.MD5不同
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5(string input)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(input, "MD5");
        }

        /// <summary>
        /// MD5加密 16位和32位
        /// </summary>
        /// <param name="str"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string MD5(string str, int code)
        {
            if (code == 16) //16位MD5加密（取32位加密的9~25字符） 
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToUpper().Substring(8, 16);
            }
            else//32位加密 
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToUpper();
            }
        }
        #endregion

        #region session的操作
        public void SetSession(string key, string val)
        {
            HttpContext.Current.Session.Timeout = 120;
            HttpContext.Current.Session[key] = val;
        }
        public string GetSession(string key)
        {
            if (HttpContext.Current.Session[key] != null)
            {
                return HttpContext.Current.Session[key].ToString();
            }
            return "";
        }
        #endregion
    }
}