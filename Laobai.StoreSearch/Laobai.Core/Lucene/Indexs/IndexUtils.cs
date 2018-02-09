using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laobai.Model;
using Laobai.Core.Common;
using Laobai.Core.Analysis;

using Laobai.Lucene.Store;
using IndexWriter = Laobai.Lucene.Index.IndexWriter;
using FSDirectory = Laobai.Lucene.Store.FSDirectory;
using Version = Laobai.Lucene.Util.Version;
using Laobai.Lucene.Analysis.Standard;
using Laobai.Lucene.Documents;
using Laobai.Lucene.Search;
using Laobai.Lucene.QueryParsers;
using Laobai.Lucene.Index;
using Laobai.Lucene.Search.Function;

namespace Laobai.Core.Lucene
{
    #region 写索引时的类
    public class SetLuceneDocument
    {
        private string _id = "0";
        /// <summary>
        /// 唯一键
        /// </summary>
        public string id
        {
            get { return _id; }
            set { _id = value; }
        }
        private Document _doc = new Document();
        /// <summary>
        /// 文档
        /// </summary>
        public Document Doc
        {
            get { return _doc; }
            set { _doc = value; }
        }
        public SetLuceneDocument()
        {

        }
        public SetLuceneDocument(string id, Document doc)
        {
            this._id = id;
            this._doc = doc;
        }
    }
    #endregion

    public class LuceneDocument
    {
        private Document _doc = new Document();
        /// <summary>
        /// 文档
        /// </summary>
        public Document Doc
        {
            get { return _doc; }
            set { _doc = value; }
        }

        private float _score = 0f;
        /// <summary>
        /// 评分
        /// </summary>
        public float Score
        {
            get { return _score; }
            set { _score = value; }
        }
    }

    public class LuceneData
    {
        private int _dataCount = 0;
        /// <summary>
        /// 数据总数
        /// </summary>
        public int DataCount
        {
            get { return _dataCount; }
            set { _dataCount = value; }
        }

        private List<LuceneDocument> _documents = new List<LuceneDocument>();
        /// <summary>
        /// LuceneDocument
        /// </summary>
        public List<LuceneDocument> Documents
        {
            get { return _documents; }
            set { _documents = value; }
        }
        #region 扩展字段
        private List<int> _typeIds = new List<int>();//分类ID
        /// <summary>
        /// 分类ID
        /// </summary>
        public List<int> TypeIds
        {
            get { return _typeIds; }
            set { _typeIds = value; }
        }

        private List<string> _brandList = new List<string>();
        /// <summary>
        /// 品牌(最多200个)
        /// </summary>
        public List<string> BrandList
        {
            get { return _brandList; }
            set { _brandList = value; }
        }

        private bool _hasDrug = false;
        /// <summary>
        /// 搜索结果包含处方药
        /// </summary>
        public bool HasDrug
        {
            get { return _hasDrug; }
            set { _hasDrug = value; }
        }
        private bool _hasCross = false;
        /// <summary>
        /// 搜索结果是否包含全球购商品
        /// </summary>
        public bool HasCross
        {
            get { return _hasCross; }
            set { _hasCross = value; }
        }
        private bool _hasNew = false;
        /// <summary>
        /// 搜索结果是否包含新品
        /// </summary>
        public bool HasNew
        {
            get { return _hasNew; }
            set { _hasNew = value; }
        }
        #endregion
    }

    public class IndexUtils
    {
        #region FormatString - 格式化需要索引的文本(主要处理连续数字、字母的索引)
        /// <summary>
        /// 格式化需要索引的文本(主要处理连续数字、字母的索引)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string FormatString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return new Analyzer().FormatString(input);
        }
        #endregion

        #region 去空格，以及处理null
        /// <summary>
        /// 去空格，以及处理null
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public string FormatValue(string val)
        {
            if (string.IsNullOrEmpty(val))
                return "";
            return val.Trim();
        }
        #endregion

        /// <summary>
        /// 索引路径
        /// </summary>
        private string indexPath = Settings.IndexsPath;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder">索引目录名称,如Product</param>
        public IndexUtils(string folder)
        {
            this.indexPath += folder;
            if (!this.indexPath.EndsWith("\\"))
                this.indexPath += "\\";//索引目录
        }
        /// <summary>
        /// segments.gen文件路径
        /// </summary>
        private string segmentsPath
        {
            get { return this.indexPath + "segments.gen"; }
        }
        /// <summary>
        /// 更新日志文件
        /// </summary>
        private string segupdateLogPath
        {
            get { return this.indexPath + "segupdate.date"; }
        }
        /// <summary>
        /// 是否已上锁
        /// </summary>
        public bool IsLock
        {
            get { return Utils.FileExists(this.indexPath + "write.lock"); }
        }
        /// <summary>
        /// 得到索引更新时间
        /// </summary>
        public DateTime GetLastModifyTime
        {
            get
            {
                if (!Utils.FileExists(segupdateLogPath))
                    return DateTime.Now.AddYears(-10);
                return System.IO.File.GetLastWriteTime(segupdateLogPath);
            }
        }
        /// <summary>
        /// 索引是否已存在
        /// </summary>
        public bool IsBegin
        {
            get { return Utils.FileExists(this.segmentsPath); }
        }

        #region ClearIndex
        /// <summary>
        /// 清空索引文件(文件夹)
        /// </summary>
        public void ClearIndex()
        {
            if (!this.IsBegin) return;
            Utils.ClearFolder(this.indexPath);//清空文件夹
        }
        #endregion

        #region 打开索引目录
        private Directory directory = null;
        /// <summary>
        /// 打开索引目录
        /// </summary>
        /// <returns></returns>
        private void OpenDirectory()
        {
            if (this.directory==null)
                this.directory = FSDirectory.Open(new System.IO.DirectoryInfo(this.indexPath), new SimpleFSLockFactory());
        }
        #endregion

        #region 创建analyzer
        private Laobai.Lucene.Analysis.Analyzer analyzer = null;
        private void CreateAnalyzer()
        {
            if (this.analyzer==null)
                this.analyzer = new StandardAnalyzer(Version.LUCENE_30, new HashSet<string>());
        }
        #endregion

        #region 创建Writer
        /// <summary>
        /// 创建Writer
        /// </summary>
        private IndexWriter writer = null;
        private void CreateWriter()
        {
            if (this.writer==null)
                this.writer = new IndexWriter(this.directory, this.analyzer, !this.IsBegin, IndexWriter.MaxFieldLength.UNLIMITED);
        }
        #endregion

        #region 创建文档的Field
        /// <summary>
        /// 创建文档的Field(只建索引,不存储内容)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <param name="boost">优先级，≤0表示不设置优先级</param>
        /// <returns></returns>
        public Field AnalyzedField(string name, string val, float boost=-1)
        {
            Field field = new Field(name, this.FormatString(this.FormatValue(val))
                , Field.Store.NO, Field.Index.ANALYZED);
            if (boost > 0) field.Boost = boost;
            return field;
        }
        #endregion

        #region 创建文档的Field
        /// <summary>
        /// 创建文档的Field(建索引并存储内容,但不做分词处理)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Field UnAnalyzedField(string name, string val)
        {
            return new Field(name, this.FormatValue(val)
                , Field.Store.YES, Field.Index.NOT_ANALYZED);
        }
        #endregion

        #region BooleanField
        /// <summary>
        /// BooleanField
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Field BooleanField(string name, string val)
        {
            int intVal = Utils.StrToInt(val);
            if (intVal < 1) intVal = 0;
            else intVal = 1;
            return new Field(name, intVal.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED);
        }
        #endregion

        #region  IntField
        public NumericField IntField(string name, int val)
        {
            return new NumericField(name, Field.Store.YES, true).SetIntValue(val);
        }
        public NumericField IntField(string name, string val)
        {
            return this.IntField(name, Utils.StrToInt(val));
        }
        #endregion

        #region  LongField
        public NumericField LongField(string name, long val)
        {
            return new NumericField(name, Field.Store.YES, true).SetLongValue(val);
        }
        public NumericField LongField(string name, string val)
        {
            return this.LongField(name, Utils.StrToLong(val));
        }
        #endregion

        #region  FloatField
        public NumericField FloatField(string name, float val)
        {
            return new NumericField(name, Field.Store.YES, true).SetFloatValue(val);
        }
        public NumericField FloatField(string name, string val)
        {
            return this.FloatField(name, Utils.StrToFloat(val));
        }
        #endregion

        #region 创建/更新索引
        /// <summary>
        /// 创建/更新索引
        /// </summary>
        /// <param name="list"></param>
        public void CreateIndex(string fieldId, List<SetLuceneDocument> list)
        {
            try
            {
                this.OpenDirectory();
                this.CreateAnalyzer();
                this.CreateWriter();
                foreach (SetLuceneDocument doc in list)
                {
                    writer.UpdateDocument(new Term(fieldId, doc.id), doc.Doc);
                }
                this.writer.Optimize();
            }
            catch (Exception)
            {

            }
            finally
            {
                Utils.WriteText(this.segupdateLogPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));//设置索引更新时间
                this.CloseWriter();//释放资源
            }
        }
        #endregion

        #region 移除文档
        /// <summary>
        /// 移除文档
        /// </summary>
        /// <param name="fieldName">根据哪个field移除文档</param>
        /// <param name="list"></param>
        public void RemoveDocuments(string fieldName, List<string> list)
        {
            if (!this.IsBegin || this.IsLock)
                return;//索引未建立或已上锁
            if (list.Count < 1)
                return;//空数组
            try
            {
                this.OpenDirectory();
                this.CreateWriter();
                Query[] querys = new Query[list.Count];
                /*QueryParser parser = new QueryParser(Version.LUCENE_30, fieldName
                    , new StandardAnalyzer(Version.LUCENE_30));*/
                for (int i = 0; i < list.Count; i++)
                {
                    querys[i] = this.CreateTermQuery(fieldName, list[i]);//parser.Parse(list[i]);
                }
                writer.DeleteDocuments(querys);
                this.writer.Optimize();
            }
            catch (Exception)
            {

            }
            finally
            {
                this.CloseWriter();//释放资源
            }
        }
        #endregion

        #region 释放analyzer
        /// <summary>
        /// 释放analyzer
        /// </summary>
        private void CloseAnalyzer()
        {
            if (this.analyzer != null)
            {
                this.analyzer.Dispose();
                this.analyzer = null;
            }
        }
        #endregion

        #region 释放directory
        /// <summary>
        /// 释放directory
        /// </summary>
        private void CloseDirectory()
        {
            if (this.directory != null)
            {
                this.directory.Dispose();
                this.directory = null;
            }
        }
        #endregion

        #region 释放写资源
        /// <summary>
        /// 释放写资源
        /// </summary>
        private void CloseWriter()
        {
            try
            {
                this.CloseAnalyzer();
                if (this.writer != null)
                {
                    this.writer.Dispose();
                    this.writer = null;
                }
                this.CloseDirectory();
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region 创建Reader
        private IndexReader reader = null;
        private void CreateReader()
        {
            if (this.reader == null)
                this.reader = IndexReader.Open(this.directory, true);
        }
        #endregion

        #region 创建Searcher
        private Searcher searcher = null;
        private void CreateSearcher()
        {
            this.CreateReader();
            if (this.searcher == null)
                this.searcher = new IndexSearcher(this.reader);
        }
        #endregion

        #region Query
        public Query CreateQuery(string field, string sKey)
        {
            this.CreateAnalyzer();
            return new QueryParser(Version.LUCENE_30, field, this.analyzer).Parse(sKey);
        }
        public Query CreateQuery(string field, string sKey, Laobai.Lucene.Analysis.Analyzer a)
        {
            return new QueryParser(Version.LUCENE_30, field, a).Parse(sKey);
        }
        #endregion

        #region CreateTermQuery
        public Query CreateTermQuery(string field, string val)
        {
            //return new QueryParser(Version.LUCENE_30, field, new Laobai.Lucene.Analysis.WhitespaceAnalyzer()).Parse(val);
            return new TermQuery(new Term(field, val));
        }
        #endregion

        #region CreateTermQuery
        public Query CreateTermQuery(string field, int val)
        {
            return CreateTermQuery(field, val.ToString());
        }
        #endregion

        #region AppendQuerys
        private BooleanQuery AppendQuerys(BooleanQuery query, List<Query> list, Occur occur)
        {
            if (list == null || list.Count<1)
            {
                return query;
            }
            foreach (Query item in list)
            {
                query.Add(item, occur);
            }
            return query;
        }
        #endregion

        #region 获取排序的type
        private int GetSortType(ESortField sortField)
        {
            int type = SortField.FLOAT;
            switch (sortField)
            {
                case ESortField.score_index:
                case ESortField.sale_price:
                case ESortField.comment_index:
                    type = SortField.FLOAT;
                    break;
                case ESortField.total_sale_count:
                case ESortField.month_sale_count:
                    type = SortField.INT;
                    break;
                case ESortField.first_up_time:
                    type = SortField.LONG;
                    break;
            }
            return type;
        }
        #endregion

        #region GetTopDocs(搜商品时)
        private TopDocs GetTopDocs(BooleanQuery bquery
            , int maxTotal
            , ESortField sortField
            , EOrderType ot = EOrderType.DESC)
        {
            TopDocs docs = null;
            bool reverse = ot.Equals(EOrderType.DESC);//排序方式(false是升序ASC)
            if (sortField.Equals(ESortField.score_index))
            {
                FieldScoreQuery fsq = new FieldScoreQuery(sortField.ToString(), FieldScoreQuery.Type.FLOAT);//创建评分域
                if (reverse)
                {
                    DefaultCustomScoreQuery squery = new DefaultCustomScoreQuery(bquery, fsq);//使用自定义评分(降序)
                    docs = this.searcher.Search(squery, null, maxTotal);
                }
                else
                {
                    DefaultCustomScoreQueryASC squery = new DefaultCustomScoreQueryASC(bquery, fsq);//使用自定义评分(升序)
                    docs = this.searcher.Search(squery, null, maxTotal);
                }
            }
            else
            {
                SortField sort1 = new SortField("is_on_sale", SortField.INT, true);//是否在售
                //SortField sort2 = new SortField("has_stock", SortField.INT, true);//是否有库存
                SortField sort3 = new SortField(sortField.ToString(), this.GetSortType(sortField), reverse);
                //sort2
                SortField[] sortArray = { sort1, sort3 };
                Sort sort = new Sort(sortArray);
                docs = this.searcher.Search(bquery, null, maxTotal, sort);
            }
            return docs;
        }
        #endregion

        #region 搜索(简单搜索)
        /// <summary>
        /// 处理查询结果
        /// </summary>
        /// <param name="docs"></param>
        /// <returns></returns>
        private LuceneData ParseQuery(TopDocs docs)
        {
            LuceneData data = new LuceneData();
            var hits = docs.ScoreDocs;
            data.DataCount = docs.TotalHits;
            var len = hits.Length;
            for (int i = 0; i < len; i++)
            {
                LuceneDocument doc = new LuceneDocument();
                doc.Doc = this.searcher.Doc(hits[i].Doc);
                doc.Score = hits[i].Score;
                data.Documents.Add(doc);
            }
            return data;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="mustQuerys"></param>
        /// <param name="shouldQuerys"></param>
        /// <param name="maxTotal"></param>
        /// <returns></returns>
        public LuceneData Query(List<Query> mustQuerys, List<Query> shouldQuerys, int maxTotal)
        {
            this.OpenDirectory();
            //this.CreateAnalyzer();
            this.CreateSearcher();
            if (maxTotal < 1) maxTotal = 1;
            if (maxTotal > 2000) maxTotal = 2000;
            //ScoreDoc[] hits
            BooleanQuery bquery = new BooleanQuery();
            bquery = this.AppendQuerys(bquery, mustQuerys, Occur.MUST);
            bquery = this.AppendQuerys(bquery, shouldQuerys, Occur.SHOULD);
            //获取TopDocs
            TopDocs docs = this.searcher.Search(bquery, null, maxTotal);
            var data = this.ParseQuery(docs);
            this.CloseReader();

            return data;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="query"></param>
        /// <param name="maxTotal"></param>
        /// <returns></returns>
        public LuceneData Query(Query query, int maxTotal)
        {
            List<Query> mustQuerys = new List<Query>();
            mustQuerys.Add(query);
            return Query(mustQuerys, new List<Query>(), maxTotal);
        }
        #endregion

        #region 搜索(商品)
        public LuceneData QueryProduct(
            List<Query> mustQuerys
            , List<Query> shouldQuerys
            , int maxTotal
            , int pagesize
            , int pageindex
            , ESortField sortField
            , EOrderType ot = EOrderType.DESC)
        {
            this.OpenDirectory();
            this.CreateSearcher();

            if (maxTotal < 1) maxTotal = 1;
            BooleanQuery bquery = new BooleanQuery();
            bquery = this.AppendQuerys(bquery, mustQuerys, Occur.MUST);
            bquery = this.AppendQuerys(bquery, shouldQuerys, Occur.SHOULD);

            //获取TopDocs
            TopDocs docs = this.GetTopDocs(bquery, maxTotal, sortField, ot);

            LuceneData data = new LuceneData();
            var hits = docs.ScoreDocs;
            data.DataCount = docs.TotalHits;
            data = this.ParseQueryProduct(hits, data, pagesize, pageindex, sortField);
            this.CloseReader();

            return data;
        }
        /// <summary>
        /// 处理查询结果
        /// </summary>
        /// <param name="docs"></param>
        /// <returns></returns>
        private LuceneData ParseQueryProduct(ScoreDoc[] hits, LuceneData data
            , int pagesize, int pageindex
            , ESortField sortField)
        {
            bool scoreIndex = sortField.Equals(ESortField.score_index);
            var len = hits.Length;
            if (pageindex < 1) pageindex = 1;
            int start = pagesize * (pageindex - 1);
            int endIdx = Math.Min(start + pagesize, len);
            for (int i = start; i < endIdx; i++)
            {
                LuceneDocument doc = new LuceneDocument();
                doc.Doc = this.searcher.Doc(hits[i].Doc);
                if (scoreIndex)
                    doc.Score = hits[i].Score;
                else
                    doc.Score = Utils.StrToFloat(doc.Doc.Get(sortField.ToString()));
                data.Documents.Add(doc);
            }
            data = this.ParseQueryProduct(hits, data, pagesize);
            return data;
        }
        /// <summary>
        /// 处理品牌等
        /// </summary>
        /// <param name="hits"></param>
        /// <param name="data"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        private LuceneData ParseQueryProduct(ScoreDoc[] hits, LuceneData data, int pagesize)
        {
            long timeStamp = Utils.GetTimeStamp(DateTime.Now.AddDays(-30));
            int max = Math.Min(pagesize * 10, hits.Length);
            for (int i = 0; i < max; i++)
            {
                var doc = this.searcher.Doc(hits[i].Doc);
                data = this.BindProduct(data, doc, timeStamp);
            }
            return data;
        }
        #endregion

        #region 绑定商品信息(分类/是否处方药/新品/全球购等)
        private LuceneData BindProduct(LuceneData data, Document doc, long timeStamp)
        {
            int type_id = int.Parse(doc.Get("product_type_id"));
            if (type_id>0 && !data.TypeIds.Contains(type_id))
                data.TypeIds.Add(type_id);//添加
            //品牌
            string brand = doc.Get("product_brand");
            if (data.BrandList.Count < 60 && brand.Length > 0 && !data.BrandList.Contains(brand))
            {
                if(!string.IsNullOrEmpty(brand.Trim()))
                    data.BrandList.Add(brand.Trim());
            }
            if (!data.HasDrug && int.Parse(doc.Get("is_drug")) == 1)
            {
                data.HasDrug = true;
            }
            //if (!data.HasCross && int.Parse(doc.Get("is_cross")) == 1)
            //{
            //    data.HasCross = true;
            //}
            //if (!data.HasNew && Utils.StrToLong(doc.Get("first_up_time")) > timeStamp)
            //{
            //    data.HasNew = true;
            //}
            return data;
        }
        #endregion

        #region 释放读资源
        /// <summary>
        /// 释放读资源
        /// </summary>
        private void CloseReader()
        {
            try
            {
                this.CloseAnalyzer();
                if (this.reader != null)
                {
                    this.reader.Dispose();
                    this.reader = null;
                }
                if (this.searcher != null)
                {
                    this.searcher.Dispose();
                    this.searcher = null;
                }
                this.CloseDirectory();
            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region 屏蔽特殊词的搜索
        /// <summary>
        /// 屏蔽特殊词的搜索
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool IsBadWords(string val)
        {
            if (string.IsNullOrEmpty(val))
                return true;
            val = val.Replace("\n", " ").Replace("\r", " ").Trim().ToLower();
            if (val.Contains("n:"))
                return true;
            if (string.IsNullOrEmpty(val) || Utils.IsMatch(val, @"^[^\u4e00-\u9fa5a-zA-Z0-9]+$"))//全符号
                return true;
            return false;
        }
        #endregion
    }
}
