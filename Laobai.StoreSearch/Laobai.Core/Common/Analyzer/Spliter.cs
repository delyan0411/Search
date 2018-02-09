using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Laobai.Analysis
{
    /// <summary>
    /// 分词,单个汉字当做一个词,连续的英文或连续的数字当做一个词
    /// </summary>
    public class Spliter
    {
        #region 属性
        private string _source = "";
        private List<string> _keys = new List<string>();
        /// <summary>
        /// 返回所有词
        /// </summary>
        public List<string> Keys
        {
            get { return _keys; }
        }
        public string KeyString
        {
            get
            {
                System.Text.StringBuilder sb = new StringBuilder();
                foreach (string s in _keys)
                {
                    sb.Append(s).Append(" ");
                }
                return sb.ToString().Trim();
            }
        }
        #endregion
        /// <summary>
        /// 分词,单个汉字当做一个词,连续的英文或连续的数字当做一个词
        /// </summary>
        /// <param name="source"></param>
        public Spliter(string source)
        {
            this._source = source;
            this.Parse();
        }

        #region 正则检测
        private bool IsMatch(string val, string reg)
        {
            return new Regex(reg).IsMatch(val);
        }
        #endregion

        #region Parse
        private List<string> Parse()
        {
            if (string.IsNullOrEmpty(this._source))
                return this._keys;
            char[] arr = this._source.ToCharArray();
            int type = 0; //0-空白或符号 1-字母 2-中文 3-数字
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                #region 分析
                string s = arr[i].ToString();
                if (!this.IsMatch(s, "^[a-zA-Z\\d\u4e00-\u9fa5]+$"))
                {
                    type = 0;
                    continue;
                }
                if (this.IsMatch(s, "^[\u4e00-\u9fa5]+$"))
                {
                    sb.Append(" ");
                    type = 2;
                }
                else if (this.IsMatch(s, "^\\d+$"))
                {
                    if (type != 3)
                        sb.Append(" ");
                    type = 3;
                }
                else if (this.IsMatch(s, "^[a-zA-Z]+$"))
                {
                    if (type != 1)
                        sb.Append(" ");
                    type = 1;
                }
                #endregion

                sb.Append(s);
            }
            if (string.IsNullOrEmpty(sb.ToString().Trim()))
            {
                return this._keys;
            }
            string[] _tem = sb.ToString().Trim().Split(' ');
            foreach (string item in _tem)
            {
                if (!this._keys.Contains(item))
                    this._keys.Add(item);
            }
            return this._keys;
        }
        #endregion
    }
}
