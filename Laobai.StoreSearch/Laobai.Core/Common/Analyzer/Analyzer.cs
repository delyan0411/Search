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

using System.Runtime.Serialization;
using Laobai.Core.Common;

namespace Laobai.Core.Analysis
{
    #region SplitWordInfo
    [DataContract]
    public class SplitWordInfo
    {
        private string _word = "";
        /// <summary>
        /// 词
        /// </summary>
        [DataMember]
        public string Word
        {
            get { return _word; }
            set { _word = value; }
        }

        private float _frequency = 0f;
        /// <summary>
        /// 词频
        /// </summary>
        [DataMember]
        public float Frequency
        {
            get { return _frequency; }
            set { _frequency = value; }
        }
    }
    #endregion

    public enum AnalysisMode
    {
        /// <summary>
        /// 简化版
        /// </summary>
        SIMPLE = 0,
        /// <summary>
        /// 全部
        /// </summary>
        ALL = 1
    }

    public class Analyzer
    {
        private AnalysisMode _mode = AnalysisMode.ALL;
        #region Analyzer
        public Analyzer()
        {

        }
        #endregion

        #region Analyzer
        public Analyzer(AnalysisMode mode)
        {
            _mode = mode;
        }
        #endregion

        #region 属性
        private string SplitChar = " ";//分隔符
        #endregion
        //
        #region 数据缓存函数
        /// <summary>
        /// 数据缓存函数
        /// </summary>
        /// <param name="key">索引键</param>
        /// <param name="val">缓存的数据</param>
        private void SetCache(string key, object val)
        {
            if (val == null)
                val = " ";
            System.Web.HttpContext.Current.Application.Lock();
            System.Web.HttpContext.Current.Application.Set(key, val);
            System.Web.HttpContext.Current.Application.UnLock();
        }

        //private static void SetCache(string key, object val)
        //{
        //    SetCache(key, val, 120);
        //}
        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <param name="mykey"></param>
        /// <returns></returns>
        private object GetCache(string key)
        {
            return System.Web.HttpContext.Current.Application.Get(key);
            //return System.Web.HttpContext.Current.Cache[key];
        }
        #endregion
        //
        #region 读取文本
        private SortedList GetDictList(string FilePath)
        {
            if (this.GetCache("dict") == null)
            {
                Encoding encoding = Encoding.GetEncoding("utf-8");
                SortedList list = new SortedList();
                //
                try
                {
                    if (!File.Exists(FilePath))
                    {
                        list.Add("0", "");
                    }
                    else
                    {
                        StreamReader objReader = new StreamReader(FilePath, encoding);
                        string sLine = "";
                        //ArrayList arrText = new ArrayList();

                        while (sLine != null)
                        {
                            sLine = objReader.ReadLine();
                            if (sLine != null)
                                list.Add(sLine, sLine);
                        }
                        //
                        objReader.Close();
                        objReader.Dispose();
                    }
                }
                catch (Exception) { }
                SetCache("dict", list);
                //return (string[])arrText.ToArray(typeof(string));
            }
            return (SortedList)this.GetCache("dict");
        }
        #endregion
        //
        #region 载入词典
        private SortedList LoadDict
        {
            get
            {
                string path = this._mode == AnalysisMode.ALL ? "words-all.dic" : "words.dic";
                path = Settings.DictionaryPath + path;
                return this.GetDictList(path);
            }
        }
        #endregion
        //
        #region 正则检测
        private bool IsMatch(string str, string reg)
        {
            return new Regex(reg).IsMatch(str);
        }
        #endregion
        //
        #region 首先格式化字符串(粗分)
        /// <summary>
        /// 首先格式化字符串(粗分)
        /// </summary>
        /// <param name="val">待分词的字符串</param>
        /// <param name="exReg">连续的字符规则(排除法)</param>
        /// <returns></returns>
        public string FormatString(string val, string joinReg = "[^0-9a-zA-Z@\\.%#:\\/\\&_-]")
        {
            string result = "";
            if (string.IsNullOrEmpty(val))
                return "";
            if (val.Length < 3)
                return val;//小于3个字符的,不分词
            //
            //if (val.Length < 20)
            //    result = val + SplitChar;
            //
            char[] CharList = val.ToCharArray();
            //
            string Spc = this.SplitChar;//分隔符
            int StrLen = CharList.Length;
            int CharType = 0; //0-空白 1-英文 2-中文 3-符号
            //
            for (int i = 0; i < StrLen; i++)
            {
                string StrList = CharList[i].ToString();
                if (StrList == null || StrList == "")
                    continue;
                //
                if (CharList[i] < 0x81)
                {
                    #region
                    if (CharList[i] < 33)
                    {
                        if (CharType != 0 && StrList != "\n" && StrList != "\r")
                        {
                            result += " ";
                            CharType = 0;
                        }
                        continue;
                    }
                    else if (IsMatch(StrList, joinReg))//排除这些字符
                    {
                        if (CharType == 0)
                            result += StrList;
                        else
                            result += Spc + StrList;
                        CharType = 3;
                    }
                    else
                    {
                        if (CharType == 2 || CharType == 3)
                        {
                            result += Spc + StrList;
                            CharType = 1;
                        }
                        else
                        {
                            if (IsMatch(StrList, "[%:\\/]"))
                            {
                                if (CharType != 3)
                                    result += Spc;
                                result += StrList;
                                CharType = 3;
                            }
                            else
                            {
                                if (CharType != 1)
                                    result += Spc;
                                result += StrList;
                                CharType = 1;
                            }//end if No.4
                        }//end if No.3
                    }//end if No.2
                    #endregion
                }//if No.1
                else
                {
                    //如果上一个字符为非中文和非空格，则加一个空格
                    if (CharType != 0 && CharType != 2)
                        result += Spc;
                    //如果是中文标点符号
                    if (!IsMatch(StrList, "^[\u4e00-\u9fa5]+$"))
                    {
                        if (CharType != 0)
                            result += Spc + StrList;
                        else
                            result += StrList;
                        CharType = 3;
                    }
                    else //中文
                    {
                        result += StrList;
                        CharType = 2;
                    }
                }
                //end if No.1

            }//exit for
            //Utils.cout(result);
            //
            return this.CheckFormatResult(result);
        }
        #endregion
        //
        #region 分词
        /// <summary>
        /// 分词
        /// </summary>
        /// <param name="key">关键词</param>
        /// <returns></returns>
        private List<string> StringSpliter(string[] key)
        {
            List<string> list = new List<string>();
            try
            {
                SortedList dict = LoadDict;//载入词典
                //
                for (int i = 0; i < key.Length; i++)
                {
                    var v = key[i].Trim();
                    if (string.IsNullOrEmpty(v))
                    {
                        continue;
                    }
                    if (dict.Contains(v))
                    {
                        list.Add(v); continue;
                    }
                    
                    if (IsMatch(v, @"^[a-zA-Z0-9\-\.\u4e00-\u9fa5@#&]+$")) //中文、英文、数字
                    {
                        if (IsMatch(v, "^[\u4e00-\u9fa5]+$"))//如果是纯中文
                        {
                            //if (!dict.Contains(key[i].GetHashCode()))
                            //    List.Add(key[i]);
                            //
                            int keyLen = v.Length;
                            if (keyLen < 1)
                                continue;
                            else if (keyLen <= 16 && !list.Contains(v))
                                list.Add(v);//小于十六个字符的当做一个关键词
                            //
                            //开始分词
                            for (int x = 0; x < keyLen; x++)
                            {
                                //x：起始位置//y：结束位置
                                for (int y = x; y < keyLen; y++)
                                {
                                    string val = v.Substring(x, keyLen - y);
                                    if (val == null || val.Length < 2)
                                        break;
                                    else if (val.Length > 10)
                                        continue;
                                    if (dict.Contains(val) && !list.Contains(val))
                                    {
                                        list.Add(val);
                                        break;
                                    }
                                }
                                //
                            }
                            //
                        }
                        else if (IsMatch(v, @"^(?:[a-zA-Z0-9_\-\.@#&]+)$"))//纯数字、纯英文
                        {
                            if (!list.Contains(v))
                                list.Add(v);
                        }
                        else //中文、英文、数字的混合
                        {
                            if (!list.Contains(v))
                                list.Add(v);
                            string input = Regex.Replace(v, @"\w+", "", RegexOptions.Compiled | RegexOptions.Singleline);
                            if (!list.Contains(input))
                                list.Add(input);
                            input = Regex.Replace(v, @"\d+", "", RegexOptions.Compiled | RegexOptions.Singleline);
                            if (!list.Contains(input))
                                list.Add(input);
                        }
                        //
                    }
                }
            }
            catch (Exception) { }
            //
            return list;
            //return (string[])List.ToArray(typeof(string));
        }
        #endregion
        //
        #region 计算某个词与指定字符串的匹配度
        public float CoutFrequency(string key, string content)
        {
            float result = 0.0f;
            int fcout = 0;//词频
            if (key.Equals("") || content.Equals(""))
                return 0.0f;//空字符串的情况
            if (key.ToLower().Trim() == content.ToLower().Trim())
                return 100.0f;//完全匹配的情况
            if (!content.Contains(key))
            {
                return 0.0f;//字符串中不存在该词
            }
            //计算词频
            string input = content;
            while (input.Contains(key))
            {
                input = input.Remove(input.IndexOf(key), key.Length);
                fcout++;
            }
            //fcout = Utils.splitString(content, key).Length;
            float keylen = (float)key.Length;
            float contentlen = (float)content.Length;
            //相似度：((关键词长度 * 词频) / 字符串长度) * 100
            if (contentlen > 0)
            {
                result = ((keylen * (float)fcout) / contentlen) * 100f;
            }
            return result;
        }
        #endregion
        //
        #region 得到分词结果
        /// <summary>
        /// 得到分词结果
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public List<SplitWordInfo> GetSplitList(string val)
        {
            List<SplitWordInfo> rList = new List<SplitWordInfo>();
            if (string.IsNullOrEmpty(val)) return rList;

            List<string> list = new List<string>();
            List<string> keyList = StringSpliter(FormatString(val).Split(SplitChar.ToCharArray()));

            #region 去权重低的词
            //List<string> _sList = new List<string>();
            //foreach (string s in keyList)
            //{
            //    bool _remove = false;
            //    foreach (string k in keyList)
            //    {
            //        if (k.IndexOf(s) >= 0 && k.Length != s.Length && s!=val)
            //        {
            //            _remove = true; break;
            //        }
            //    }
            //    if (!_remove)
            //        _sList.Add(s);
            //}
            //keyList = _sList;
            #endregion

            if (val.Length < 100 && IsMatch(val, "^[\u4e00-\u9fa5\\d\\w]+$"))
            {
                list.Add(val);
                SplitWordInfo _info = new SplitWordInfo();
                _info.Word = val;
                _info.Frequency = 100.0f;
                rList.Add(_info);
            }
            foreach (string item in keyList)
            {
                if (item.StartsWith("."))
                    continue;
                if (item.Length == 1)// && !IsMatch(item, "^[\u4e00-\u9fa5]+$")
                    continue;//去掉单字
                if (IsMatch(item, "^[\u4e00-\u9fa5]+$") && item.Length > 10)
                    continue;//去掉超过10个字的中文词
                if (IsMatch(item, @"^[^\u4e00-\u9fa5a-zA-Z0-9]+$"))
                    continue;//去掉纯符号
                if (item.Equals("OR"))
                {
                    var v = "or";
                    list.Add(v);
                    rList.Add(this.AppendWord(v, val));
                    continue;
                }
                if (item.StartsWith("-"))
                {
                    int last = item.LastIndexOf("-");//去掉-符号开头的
                    string v = item;
                    if(item.Length>last)
                        v = item.Substring(last + 1);
                    list.Add(v);
                    rList.Add(this.AppendWord(v, val));
                    continue;
                }
                if (!list.Contains(item))
                {
                    list.Add(item);
                    rList.Add(this.AppendWord(item, val));
                }
            }
            if (rList.Count < 1)
            {
                foreach (string item in keyList)
                {
                    if (IsMatch(item, "^[\u4e00-\u9fa5a-zA-Z0-9]+$"))
                    {
                        rList.Add(new SplitWordInfo() { Word = val });
                    }
                }
            }

            rList.Sort(delegate(SplitWordInfo x, SplitWordInfo y) { return y.Frequency.CompareTo(x.Frequency); });
            return rList;
        }
        #endregion

        #region
        private SplitWordInfo AppendWord(string item, string val)
        {
            SplitWordInfo _info = new SplitWordInfo();
            _info.Word = item;
            _info.Frequency = CoutFrequency(item, val);
            return _info;
        }
        #endregion

        //
        #region 得到分词结果
        /// <summary>
        /// 得到分词结果
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public List<string> GetList(string val)
        {
            List<SplitWordInfo> list = GetSplitList(val);
            List<string> rList = new List<string>();
            foreach (SplitWordInfo item in list)
            {
                rList.Add(item.Word);
            }
            return rList;
        }
        #endregion
        //
        #region 得到分词结果
        /// <summary>
        /// 得到分词结果
        /// </summary>
        /// <param name="val"></param>
        /// <param name="format">间隔符号</param>
        /// <returns></returns>
        public string GetSplitString(string val, char format)
        {
            List<SplitWordInfo> list = GetSplitList(val);
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (SplitWordInfo item in list)
            {
                if (count > 0)
                    sb.Append(format.ToString());
                sb.Append(item.Word);
                count++;
            }
            return sb.ToString();
        }
        #endregion

        #region 正则匹配模式
        private RegexOptions options
        {
            get
            {
                RegexOptions opt;
                if (Environment.Version.Major == 1)
                    opt = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline;
                else
                    opt = RegexOptions.IgnoreCase | RegexOptions.Multiline;
                return opt;
            }
        }
        #endregion

        #region 纠正粗分结果
        private string CheckFormatResult(string input)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(input);
            Regex r = new Regex("[0-9a-zA-Z@\\.%#:\\/\\&_\\-]+", this.options);
            Match m = null;
            for (m = r.Match(input); m.Success; m = m.NextMatch())
            {
                string mval = m.Groups[0].ToString();
                if ((!(Utils.IsMatch(mval, "[@%#:\\/\\&_\\-]") && Utils.IsMatch(mval, "[a-zA-Z0-9]")))
                    && !(Utils.IsMatch(mval, "([0-9]+[a-zA-Z]+)|([a-zA-Z]+[0-9]+)"))
                    )
                    continue;
                string str = this.MatchValue(mval, "[0-9]+(\\.[0-9]+)*");//数字单独索引
                if (!string.IsNullOrEmpty(str))
                    sb.Append(" ").Append(str);
                str = this.MatchValue(mval, "[a-zA-Z]+");
                if (!string.IsNullOrEmpty(str))
                    sb.Append(" ").Append(str);
            }
            //return new Analyzer().FormatString(input, "[^0-9a-zA-Z]");
            return sb.ToString();
        }
        private string MatchValue(string val, string reg)
        {
            StringBuilder sb = new StringBuilder();
            Regex r = new Regex(reg, this.options);
            Match m = null;
            for (m = r.Match(val); m.Success; m = m.NextMatch())
            {
                string mval = m.Groups[0].ToString();
                if (mval.Length < 2)
                    continue;
                if (!string.IsNullOrEmpty(mval))
                    sb.Append(" ").Append(mval);
            }
            return sb.ToString();
        }
        #endregion
    }
}
