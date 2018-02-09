using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laobai.Model;
using Laobai.Core.Common;
using Laobai.Lucene.Documents;
using Laobai.Core.Analysis;

namespace Laobai.Core.Lucene
{
    public class WordReader
    {
        private IndexUtils indexUtils;
        public WordReader()
        {
            this.indexUtils = new IndexUtils("word");//初始化
        }
        /// <summary>
        /// 最近修改时间
        /// </summary>
        public DateTime LastModifyTime
        {
            get
            {
                return this.indexUtils.GetLastModifyTime;
            }
        }

        /// <summary>
        /// 最近修改时间
        /// </summary>
        public bool IsBegin
        {
            get
            {
                return this.indexUtils.IsBegin;
            }
        }
        #region BindData
        private WordInfo BindData(LuceneDocument model)
        {
            var doc = model.Doc;
            WordInfo Info = new WordInfo();
            Info.word_id = Utils.StrToInt(doc.Get("word_id"));
            Info.word_name = doc.Get("word_name");
            Info.use_freq = Utils.StrToLong(doc.Get("user_freq"));
            return Info;
        }
        private List<WordInfo> BindList(List<LuceneDocument> list)
        {
            var data = new List<WordInfo>();
            foreach (LuceneDocument doc in list)
            {
                data.Add(this.BindData(doc));
            }
            return data;
        }
        #endregion

        #region 查询
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sKey"></param>
        /// <param name="size">1≤size≤200</param>
        /// <returns></returns>
        public List<WordInfo> Query(string sKey, int size)
        {
            if (indexUtils.IsBadWords(sKey))
                return new List<WordInfo>();
            if (size < 1) size = 1;
            if (size > 200) size = 200;
            string query = new Analyzer().GetSplitString(sKey, ' ');
            if (indexUtils.IsBadWords(query))
                return new List<WordInfo>();
            List<Laobai.Lucene.Search.Query> mustQuerys = new List<Laobai.Lucene.Search.Query>();
            mustQuerys.Add(indexUtils.CreateQuery("text", query));
            var data = indexUtils.Query(indexUtils.CreateQuery("text", query), size);
            return this.BindList(data.Documents);
        }
        #endregion
    }
}
