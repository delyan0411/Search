using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

using Laobai.Model;
using Laobai.Core.Common;
using Laobai.Lucene.Documents;

namespace Laobai.Core.Lucene
{
    public class WordWriter
    {
        private IndexUtils indexUtils;
        public WordWriter()
        {
            this.indexUtils = new IndexUtils("word");//初始化
        }

        #region 初始化Document内容
        private Document CreateDocument(WordInfo item)
        {
            Document doc = new Document();

            #region 初始化
            doc.Add(this.indexUtils.UnAnalyzedField("word_id", item.word_id.ToString()));
            doc.Add(this.indexUtils.UnAnalyzedField("word_name", item.word_name));
            doc.Add(this.indexUtils.AnalyzedField("text", item.word_name));
            doc.Add(this.indexUtils.UnAnalyzedField("user_freq", item.use_freq.ToString()));
            //排序指数
            doc.Add(this.indexUtils.UnAnalyzedField("score_index", item.use_freq.ToString()));//综合指数
            //
            doc.Add(this.indexUtils.AnalyzedField("all", "all"));
            #endregion

            return doc;
        }
        #endregion

        #region 清除已被删除的商品索引
        /// <summary>
        /// 清除已被删除的商品索引
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<WordInfo> ResetList(List<WordInfo> list)
        {
            List<WordInfo> newList = new List<WordInfo>();
            List<string> idList = new List<string>();//需要删除的索引内容
            foreach (WordInfo p in list)
            {
                if (p.word_id > 0 && !idList.Contains(p.word_id.ToString()))
                    idList.Add(p.word_id.ToString());
                if (p.allow_suggest == 1)
                    newList.Add(p);
            }
            this.indexUtils.RemoveDocuments("word_id", idList);
            return newList;
        }
        #endregion

        #region 创建索引
        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="list"></param>
        public void CreateIndex(List<WordInfo> list)
        {
            list = this.ResetList(list);
            if (list.Count < 1)
                return;
            List<SetLuceneDocument> docs = new List<SetLuceneDocument>();
            foreach (WordInfo item in list)
            {
                docs.Add(new SetLuceneDocument(item.word_id.ToString(), this.CreateDocument(item)));
            }
            this.indexUtils.CreateIndex("word_id", docs);
        }
        #endregion
    }
}
