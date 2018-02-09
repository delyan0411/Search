using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laobai.Model;
using Laobai.Core.Common;
using Laobai.Core.Common.Http;
using Laobai.Core.Common.Json;
using System.Runtime.Serialization.Json;
using System.IO;

namespace Laobai.Core.Data
{
    public class DBHelper<T>
    {
        /// <summary>
        /// 默认缓存3600 * 24秒
        /// </summary>
        private static int cacheExpired
        {
            get { return 3600 * 24; }
        }
        /// <summary>
        /// 配置信息
        /// </summary>
        private static ConfigInfo config = Settings.BaseConfig;

        #region RequestHead
        /// <summary>
        /// 头信息
        /// </summary>
        public static RequestHeader Header
        {
            get
            {
                RequestHeader header = new RequestHeader();
                header.dev_model = "LuceneService 2.0";//设备型号
                header.dev_no = "LuceneIndex";//手机是设备号，浏览器是自动生成的cookie
                header.dev_plat = "Windows";//设备平台,电脑-WEB，微网站-MICROWEB,其它手机
                header.dev_ver = "2.0";
                header.ip_addr = "127.0.0.1";//HttpUtils.ClientIP;
                header.push_id = "";
                header.soft_ver = "2.0";
                header.token_id = "";
                header.user_id = "0";//用户ID
                return header;
            }
        }
        #endregion

        #region 拼接请求的Json
        /// <summary>
        /// 拼接请求的Json
        /// </summary>
        /// <param name="key"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private static string FormatRequestJson(string key, string body)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{")
                .Append("\"key\":").Append("\"" + key + "\"")
                .Append(",\"header\":").Append(JsonHelper.ObjectToJson(Header))
                .Append(",\"body\":").Append(body)
                .Append("}");
            return sb.ToString();
        }
        #endregion

        #region 获取ResponseResult
        private static ResponseResult defRes = new ResponseResult(-1, "系统异常");//默认的返回结果
        /// <summary>
        /// 获取ResponseResult
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static ResponseResult GetResult(string response)
        {
            ResponseResult res = defRes;
            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    Response<ResponseBodyEmpty> json = JsonHelper.JsonToObject<Response<ResponseBodyEmpty>>(response);
                    if (json != null
                        && json.header != null
                        && json.header.ret_result != null)
                        return json.header.ret_result;
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                }
            }
            return res;
        }
        #endregion

        #region 获取接口返回的Json内容
        /// <summary>
        /// 获取接口返回的Json内容
        /// </summary>
        /// <param name="key">请求的Key</param>
        /// <param name="bodyJson">请求内容的Body</param>
        /// <param name="apiUrl">请求的接口地址</param>
        /// <param name="expired">缓存时长(单位秒，-1表示不缓存)</param>
        /// <returns></returns>
        public static string GetResponseJson(string key, string bodyJson, string apiUrl, int expired = 3600)
        {
            string requestJson = FormatRequestJson(key, bodyJson);//初始化请求参数
            string url = string.IsNullOrEmpty(apiUrl) ? config.DataUrl : apiUrl;//接口地址
            string cacheKey = key + "@" + requestJson + "@" + apiUrl;//设置缓存的Key
            DoCache _cache = new DoCache();
            if (expired > 0 && _cache.GetCache(cacheKey) != null)//存在缓存
            {
                return _cache.GetCache(cacheKey).ToString();
            }
            var http = new HttpRobots();
            string response = http.Post(url, requestJson);
            if (http.IsError)//远程服务器错误
            {
                Logger.Error(http.ErrorMsg);
                return "";
            }
            ResponseResult res_msg = GetResult(response);
            if (res_msg.ret_code == 0 && expired > 0)
            {//返回正确结果时，缓存
                _cache.SetCache(cacheKey, response);
            }
            if (config.IsDebug)
            {//Debug模式记录返回信息
                StringBuilder sb = new StringBuilder();
                sb.Append("\r\n>>>>>>> +start request+ >>>>>>>---+--->>>>>>>-------+----->>>>>>>\r\n");
                sb.Append(requestJson);
                sb.Append("\r\n>>>>>>> +response Json+ >>>>>>>---+--->>>>>>>---->>>+>>\r\n");
                sb.Append(response);//输出内容
                sb.Append("\r\n<<<<<<< +finsh request+ <<<<<<<---+---<<<<<<<-------+-----<<<<<<<\r\n\r\n");
                Logger.Debug(sb.ToString());
            }
            return response;
        }
        #endregion

        #region 获取接口返回的Json内容
        /// <summary>
        /// 获取接口返回的Json内容
        /// </summary>
        /// <param name="key">请求的Key</param>
        /// <param name="bodyJson">请求内容的Body</param>
        /// <param name="apiUrl">请求的接口地址</param>
        /// <param name="isCache">是否缓存(默认缓存360秒)</param>
        /// <returns></returns>
        public static string GetResponseJson(string key, string bodyJson, string apiUrl, bool isCache)
        {
            return GetResponseJson(key, bodyJson, apiUrl, isCache ? cacheExpired : -1);
        }
        #endregion

        #region 获取接口返回的Json内容
        /// <summary>
        /// 获取接口返回的Json内容(默认请求的接口为DataUrl,默认缓存360秒)
        /// </summary>
        /// <param name="key">请求的Key</param>
        /// <param name="bodyJson">请求内容的Body</param>
        /// <returns></returns>
        public static string GetResponseJson(string key, string bodyJson)
        {
            return GetResponseJson(key, bodyJson, "", cacheExpired);
        }
        #endregion

        #region 获取完整的响应信息
        /// <summary>
        /// 获取完整的响应信息
        /// </summary>
        /// <param name="responseString"></param>
        /// <returns></returns>
        public static Response<T> GetFull(string responseString)
        {
            var rt = new Response<T>();
            rt.header = new ResponseHeader();
            rt.header.ret_result = defRes;
            if (string.IsNullOrEmpty(responseString))
                return rt;
            try
            {
                rt = JsonHelper.JsonToObject<Response<T>>(responseString);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString() + "\r\n" + responseString);
            }
            if (rt == null)
            {
                rt = new Response<T>();
                rt.header = new ResponseHeader();
                rt.header.ret_result = defRes;
            }
            if (rt.header == null)
            {
                rt.header = new ResponseHeader();
                rt.header.ret_result = defRes;
            }
            return rt;
        }
        #endregion

        #region 判断返回的内容是否有异常
        /// <summary>
        /// 判断返回的内容是否有异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public static bool IsOK<T>(Response<T> response)
        {
            if (response != null && response.header != null
                && response.header.ret_result != null && response.header.ret_result.ret_code == 0 && response.body != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断响应是否正确
        /// </summary>
        /// <param name="json">响应的内容</param>
        /// <returns></returns>
        public static bool IsOK(string responseString)
        {
            if (string.IsNullOrEmpty(responseString))
                return false;
            try
            {
                return IsOK(GetFull(responseString));
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
            return false;
        }
        #endregion

        #region 获取反序列化后的数据
        /// <summary>
        /// 获取反序列化后的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="bodyJson"></param>
        /// <param name="apiUrl"></param>
        /// <param name="expired"></param>
        /// <returns></returns>
        public static Response<T> GetResponse(string key, string bodyJson, string apiUrl, int expired = 3600)
        {
            var rt = new Response<T>();
            try
            {
                string response = GetResponseJson(key, bodyJson, apiUrl, expired);
                return GetFull(response);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
            return rt;
        }
        /// <summary>
        /// 获取反序列化后的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="bodyJson"></param>
        /// <param name="apiUrl"></param>
        /// <param name="isCache"></param>
        /// <returns></returns>
        public static Response<T> GetResponse(string key, string bodyJson, string apiUrl, bool isCache)
        {
            return GetResponse(key, bodyJson, apiUrl, isCache ? cacheExpired : -1);
        }
        /// <summary>
        /// 获取反序列化后的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="bodyJson"></param>
        /// <returns></returns>
        public static Response<T> GetResponse(string key, string bodyJson)
        {
            return GetResponse(key, bodyJson, "", cacheExpired);
        }
        #endregion
    }
}
