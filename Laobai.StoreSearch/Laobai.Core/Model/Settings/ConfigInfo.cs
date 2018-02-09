using System;

namespace Laobai.Model
{
    /// <summary>
    /// 站点配置
    /// </summary>
    public class ConfigInfo
    {
        private string _WebRoot = "";
        /// <summary>
        /// 站点根路径
        /// </summary>
        public string WebRoot
        {
            get { return _WebRoot; }
            set { _WebRoot = value; }
        }

        private string _DataUrl = "";
        /// <summary>
        /// 数据接口地址
        /// </summary>
        public string DataUrl
        {
            get { return _DataUrl; }
            set { _DataUrl = value; }
        }

        private bool _isDebug = false;

        /// <summary>
        /// 是否Debug模式
        /// </summary>
        public bool IsDebug
        {
            get { return _isDebug; }
            set { _isDebug = value; }
        }
    }
}