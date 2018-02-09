using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Laobai.Model
{
    /// <summary>
    /// WordInfo:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [DataContract]
    public partial class WordInfo
    {
        #region Model
        private int _id = 0;
        private string _word = "";
        private long _frequency = 0;
        private int _allowsuggest = 1;
        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public int word_id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 关键词
        /// </summary>
        [DataMember]
        public string word_name
        {
            set { _word = value; }
            get { return _word; }
        }
        /// <summary>
        /// 热度
        /// </summary>
        [DataMember]
        public long use_freq
        {
            set { _frequency = value; }
            get { return _frequency; }
        }
        /// <summary>
        /// 是否允许在搜索提示中显示
        /// </summary>
        [DataMember]
        public int allow_suggest
        {
            set { _allowsuggest = value; }
            get { return _allowsuggest; }
        }
        #endregion Model

    }

    #region
    [DataContract]
    public class RequestWordBody
    {
        [DataMember]
        public string page_no { set; get; }

        [DataMember]
        public string page_size { set; get; }
    }
    #endregion

    [Serializable]
    [DataContract]
    public class ResponseWord : ResponseListBody
    {
        private List<WordInfo> _word_list = new List<WordInfo>();

        [DataMember]
        public List<WordInfo> word_list
        {
            get { return _word_list; }
            set { _word_list = value; }
        }
    }

    #region 相应搜索参数
    [DataContract]
    public class RequestModifyWordBody
    {
        [DataMember]
        public List<ModifyWord> word_list { set; get; }
    }
    #endregion

    #region 相应搜索参数
    [DataContract]
    public class ModifyWord
    {
        [DataMember]
        public string word_name { set; get; }

        public ModifyWord() { }

        public ModifyWord(string word_name)
        {
            this.word_name = word_name;
        }
    }
    #endregion

    #region 相应搜索参数
    [DataContract]
    public class SearchRequestWordBody
    {
        [DataMember]
        public string search_word { set; get; }

        /// <summary>
        /// 待废弃
        /// </summary>
        [DataMember]
        public string seach_word { set; get; }

        [DataMember]
        public int page_no { set; get; }

        [DataMember]
        public int page_size { set; get; }
    }
    #endregion
}