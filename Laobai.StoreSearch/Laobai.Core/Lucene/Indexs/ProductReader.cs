using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laobai.Model;
using Laobai.Core.Common;
using Laobai.Lucene.Documents;
using Laobai.Lucene.Search;
using Laobai.Lucene.Index;
using Laobai.Core.Data;

namespace Laobai.Core.Lucene
{
    public class ProductReader
    {
        private IndexUtils indexUtils;
        public ProductReader()
        {
            this.indexUtils = new IndexUtils("product");//初始化
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
        /// 索引是否存在
        /// </summary>
        public bool IsBegin
        {
            get
            {
                return this.indexUtils.IsBegin;
            }
        }

        #region BindData
        private ProductIndexInfo BindData(LuceneDocument model)
        {
            var doc = model.Doc;
            ProductIndexInfo Info = new ProductIndexInfo();
            Info.product_id = int.Parse(doc.Get("product_id"));
            //Info.product_code = doc.Get("product_code");
            Info.shop_id = int.Parse(doc.Get("shop_id"));
            //Info.shop_type = int.Parse(doc.Get("shop_type"));
            Info.product_brand = doc.Get("product_brand");
            Info.product_type_id = int.Parse(doc.Get("product_type_id"));
            Info.product_type_path = doc.Get("product_type_path");

            //Info.is_on_sale = int.Parse(doc.Get("is_on_sale"));
            //Info.is_drug = int.Parse(doc.Get("is_drug"));
            //Info.allow_ebaolife = int.Parse(doc.Get("allow_ebaolife"));
            //Info.is_cross_border = int.Parse(doc.Get("is_cross"));
            //Info.is_free_fare = int.Parse(doc.Get("is_free_fare"));
            //Info.is_promotion = int.Parse(doc.Get("is_promotion"));
            //Info.first_up_time = long.Parse(doc.Get("first_up_time"));
            //Info.sale_price = decimal.Parse(doc.Get("sale_price"));

            //Info.total_sale_count = int.Parse(doc.Get("total_sale_count"));
            Info.score_index = float.Parse(doc.Get("score_index"));
            Info.score = model.Score;
            Info.comment_index = float.Parse(doc.Get("comment_index"));

            return Info;
        }
        private List<ProductIndexInfo> BindList(List<LuceneDocument> list)
        {
            var data = new List<ProductIndexInfo>();
            foreach (LuceneDocument doc in list)
            {
                data.Add(this.BindData(doc));
            }
            list.Clear();
            list = null;
            return data;
        }
        #endregion

        /// <summary>
        /// 格式化搜索模式
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        private ESearchMode FormatSearchMode(string keyword)
        {
            ESearchMode searchMode = ESearchMode.KEY;//搜索模式
            string sKey = keyword.Trim().ToLower();
            if (sKey.Length>2 && sKey.StartsWith("n:"))
            {
                searchMode = ESearchMode.CODE;//按商品编码搜索
            }
            else if (sKey.Equals("@all"))
            {
                searchMode = ESearchMode.ALL;//搜索所有
            }
            else if (sKey.Equals("@norms"))
            {
                searchMode = ESearchMode.NORMS;
            }
            return searchMode;
        }

        #region GetMaxTotal
        /// <summary>
        /// 最大允许查询的记录数
        /// </summary>
        private int maxRecords
        {
            get { return 1000000; }
        }
        /// <summary>
        /// 获取需查询的记录数(pagesize * 100倍)
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        //private int GetMaxTotal(int pagesize, int pageindex)
        //{
        //    if (pagesize < 100) pagesize = 100;//最低1万
        //    int maxTotal = pagesize * 30;//首次最多取pagesize的100倍
        //    int dataCount = pagesize * (pageindex + 10);
        //    if (dataCount > maxTotal)
        //    {
        //        maxTotal = maxTotal * Utils.GetPageCount(dataCount, maxTotal);
        //    }
        //    if (maxTotal > this.maxRecords)
        //        maxTotal = this.maxRecords;//最多显示2万商品
        //    return maxTotal;
        //}
        #endregion

        #region 搜索多个分类
        private List<Query> SearchByTypeList(List<ProductTypeInfo> typeList, int[] idList)
        {
            List<Query> list = new List<Query>();
            foreach (int id in idList)
            {
                var type = ProductTypeDB.GetInfo(typeList, id);
                Term term = new Term("product_type_path", type.product_type_path + "*");
                Query query = new WildcardQuery(term);
                list.Add(query);
            }
            return list;
        }
        #endregion

        #region FormatMustQuerys
        private List<Query> FormatMustQuerys(ESearchMode searchMode
            , string keys
            , string type_path
            , string brand
            , int startPrice
            , int endPrice
            , int ison, int isDrug, int is_cross
            , int allowYBLife
            , int is_free_fare
            , int is_promotion
            , int is_new
            , int shop_id
            , int shop_type
            )
        {
            List<Query> list = new List<Query>();
            if (!string.IsNullOrEmpty(keys))
            {
                Query query = null;
                switch (searchMode)
                {
                    case ESearchMode.ALL://搜合所有商品
                        {
                            query = this.indexUtils.CreateTermQuery("all", "all");//搜索
                            break;
                        }
                    case ESearchMode.NORMS://搜合规商品
                        {
                            query = this.indexUtils.CreateTermQuery("norms", "1");//搜索
                            break;
                        }

                    case ESearchMode.CODE:
                        {
                            string key = keys.Substring(2);
                            query = this.indexUtils.CreateTermQuery("product_code", key);//精确搜索
                            break;
                        }
                    case ESearchMode.KEY:
                        {
                            query = this.indexUtils.CreateQuery("maxtext", keys);//搜索
                            break;
                        }
                }
                list.Add(query);
            }
            if (!string.IsNullOrEmpty(type_path))
            {
                #region 分类
                Term term = new Term("product_type_path", type_path + "*");
                Query query = new WildcardQuery(term);
                list.Add(query);
                #endregion
            }
            if (!string.IsNullOrEmpty(brand))
            {
                Query query = this.indexUtils.CreateTermQuery("product_brand", brand);//搜索
                list.Add(query);
            }
            if (startPrice > 0 || endPrice > 0)
            {
                #region 价格
                int val = startPrice;
                if (endPrice < startPrice && endPrice>0)
                {
                    startPrice = endPrice;
                    endPrice = val;
                }
                if (startPrice != endPrice)
                {
                    if (startPrice <= 0)
                        startPrice = int.MinValue;
                    if (endPrice <= 0)
                        endPrice = int.MaxValue;
                    Query query = NumericRangeQuery.NewFloatRange("sale_price", startPrice, endPrice
                        , true
                        , true);//价格范围
                    list.Add(query);
                }
                #endregion
            }
            if (ison >= 0)
            {
                Query query = this.indexUtils.CreateTermQuery("is_on_sale", ison);//是否上架
                list.Add(query);
            }
            if (isDrug >= 0)
            {
                Query query = this.indexUtils.CreateTermQuery("is_drug", isDrug.ToString());//是否处方药
                list.Add(query);
            }
            //if (is_cross >= 0)
            //{
            //    Query query = this.indexUtils.CreateTermQuery("is_cross", is_cross.ToString());//是否跨境商品
            //    list.Add(query);
            //}
            if (allowYBLife >= 0)
            {
                Query query = this.indexUtils.CreateTermQuery("allow_ebaolife", allowYBLife.ToString());//是否支持医卡通付款
                list.Add(query);
            }
            if (is_free_fare >= 0)
            {
                Query query = this.indexUtils.CreateTermQuery("is_free_fare", is_free_fare.ToString());//是否包邮
                list.Add(query);
            }
            if (is_promotion >= 0)
            {
                Query query = this.indexUtils.CreateTermQuery("is_promotion", is_promotion.ToString());//是否促销
                list.Add(query);
            }
            if (is_new >= 0)
            {
                #region 是否新品
                long val = Utils.ConvertDateTimeLong(DateTime.Now.AddDays(-30));
                long max = Utils.ConvertDateTimeLong(DateTime.Now);
                if (is_new > 0)
                {
                    Query query = NumericRangeQuery.NewLongRange("first_up_time", val, max, true, false);//时间
                    list.Add(query);
                }
                else
                {
                    Query query = NumericRangeQuery.NewLongRange("first_up_time", 0, val, false, true);//时间
                    list.Add(query);
                }
                #endregion
            }
            if (shop_id > 0)
            {
                Query query = this.indexUtils.CreateTermQuery("shop_id", shop_id.ToString());//商家ID
                list.Add(query);
            }
            if (shop_type >= 0)
            {
                Query query = this.indexUtils.CreateTermQuery("shop_type", shop_type);//商家类别
                list.Add(query);
            }
            return list;
        }
        #endregion

        #region FormatShouldQuerys
        private List<Query> FormatShouldQuerys(string keys)
        {
            List<Query> list = new List<Query>();
            string[] fields = { "sname", "sbrand", "skey", "stype" };
            foreach (string f in fields)
            {
                Query query = this.indexUtils.CreateQuery(f, keys);
                list.Add(query);
            }
            return list;
        }
        #endregion

        #region 商品搜索的属性设置
        private List<ProductTypeInfo> _hasTypeList = new List<ProductTypeInfo>();
        /// <summary>
        /// 搜索结果包含的分类
        /// </summary>
        public List<ProductTypeInfo> HasTypeList
        {
            get
            {
                if (this._hasTypeList != null)
                {
                    this._hasTypeList.RemoveAll(item => item.product_type_id.Equals("1476"));//移除运费补拍的分类
                    /*
                    FilterInfo filter = Settings.Filter;
                    this._hasTypeList.RemoveAll(
                        delegate(ProductTypeInfo item){
                            bool isMatch = false;
                            foreach (var s in filter.filter_keys)
                            {
                                if (!string.IsNullOrEmpty(item.type_name)
                                    && !string.IsNullOrEmpty(s) && item.type_name.IndexOf(s) >= 0)
                                {
                                    isMatch = true; break;
                                }
                            }//过滤
                            return (filter.filter_types.Contains(int.Parse(item.product_type_id)) || isMatch);
                    });*/
                    this._hasTypeList.Sort(//先排序
                        delegate(ProductTypeInfo x, ProductTypeInfo y)
                        {
                            int xSort = Utils.StrToInt(x.sort_no);
                            int ySort = Utils.StrToInt(y.sort_no);
                            return xSort.CompareTo(ySort);
                        });
                }
                return this._hasTypeList;
            }
        }
        private List<string> _hasBrandList = new List<string>();
        /// <summary>
        /// 搜索结果包含的品牌
        /// </summary>
        public List<string> HasBrandList
        {
            get { return _hasBrandList; }
        }
        private bool _hasDrug = false;
        /// <summary>
        /// 搜索结果包含处方药
        /// </summary>
        public bool HasDrug
        {
            get { return _hasDrug; }
        }
        private bool _hasCross = false;
        /// <summary>
        /// 搜索结果是否包含全球购商品
        /// </summary>
        public bool HasCross
        {
            get { return _hasCross; }
        }
        private bool _hasNew = false;
        /// <summary>
        /// 搜索结果是否包含新品
        /// </summary>
        public bool HasNew
        {
            get { return _hasNew; }
        }
        private List<string> _analyzerResult = new List<string>();
        /// <summary>
        /// 根据词库分词的结果
        /// </summary>
        public List<string> AnalyzerResult
        {
            get { return _analyzerResult; }
        }
        private bool _isSearchByKey = false;
        /// <summary>
        /// 是否关键词搜索
        /// </summary>
        public bool IsSearchByKey
        {
            get { return _isSearchByKey; }
        }
        #endregion

        #region ParseTypeList
        List<int> tIdList = new List<int>();//分类ID
        /// <summary>
        /// 按分类路径，把ID放入List
        /// </summary>
        /// <param name="path"></param>
        private void ParseTypePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            string[] arr = path.Split(',');
            foreach (string s in arr)
            {
                if (string.IsNullOrEmpty(s))
                    continue;
                int v = Utils.StrToInt(s);
                if (v > 0 && !tIdList.Contains(v))
                    tIdList.Add(v);
            }
        }
        #endregion

        #region AddToTypeList
        /// <summary>
        /// 添加父类
        /// </summary>
        /// <param name="typeList"></param>
        /// <param name="search_type_id"></param>
        private void AddParentType(List<ProductTypeInfo> typeList, int search_type_id)
        {
            if (search_type_id < 1)
                return;
            var type = ProductTypeDB.GetInfo(typeList, search_type_id);
            if (Utils.StrToInt(type.product_type_id) > 0)
            {
                this._hasTypeList.Add(type);
            }
            int parentId = Utils.StrToInt(type.parent_type_id);
            if (parentId > 0)
            {
                this.AddParentType(typeList, parentId);
            }
        }
        /// <summary>
        /// 添加子类
        /// </summary>
        /// <param name="typeList"></param>
        /// <param name="search_type_id"></param>
        private void AddToTypeList(List<ProductTypeInfo> typeList, int search_type_id)
        {
            if (search_type_id < 0) search_type_id = 0;
            //if (search_type_id == id || type.parent_type_id.Equals(search_type_id.ToString()))
            this._hasTypeList = typeList.FindAll(
                        delegate(ProductTypeInfo em)
                        {
                            if (tIdList.Contains(Utils.StrToInt(em.product_type_id)))
                            {
                                if (em.parent_type_id.Equals(search_type_id.ToString()))
                                    return true;
                            }
                            return false;
                        });
            if (this._hasTypeList == null) this._hasTypeList = new List<ProductTypeInfo>();
            this.AddParentType(typeList, search_type_id);
        }
        #endregion

        #region ParseData
        /// <summary>
        /// 搜全部
        /// </summary>
        /// <param name="typeList"></param>
        /// <param name="search_type_id"></param>
        private void GetTypeForSearchAll(List<ProductTypeInfo> typeList, int search_type_id)
        {
            this._hasTypeList = ProductTypeDB.GetList(typeList, 0);//所有顶级分类
            if (search_type_id > 0)
            {
                string typePath = ProductTypeDB.GetInfo(typeList, search_type_id).product_type_path;
                string[] arr = typePath.Split(',');
                List<int> _test = new List<int>();
                foreach (string s in arr)
                {
                    int val = Utils.StrToInt(s);
                    if (val>0 && !_test.Contains(val))
                    {
                        var list = ProductTypeDB.GetList(typeList, val);
                        if (list.Count > 0)
                            this._hasTypeList.AddRange(list);
                    }
                    _test.Add(val);
                }
            }
        }
        private void ParseData(LuceneData data, List<ProductTypeInfo> typeList, int search_type_id, bool isSearchAll)
        {
            this._hasBrandList = data.BrandList;
            this._hasDrug = data.HasDrug;
            this._hasCross = data.HasCross;
            this._hasNew = data.HasNew;
            this.tIdList.Clear();
            this.tIdList.AddRange(data.TypeIds);

            if (isSearchAll)
            {
                //搜全部
                this._hasTypeList = typeList;
                //this.GetTypeForSearchAll(typeList, search_type_id);
                return;
            }
            
            foreach (int id in data.TypeIds)
            {
                ProductTypeInfo type = ProductTypeDB.GetInfo(typeList, id);
                this.ParseTypePath(type.product_type_path);
            }

            this._hasTypeList = typeList.FindAll(em => (
                this.tIdList.Contains(Utils.StrToInt(em.product_type_id))
            ));
           //var item = ProductTypeDB.GetInfo(typeList, search_type_id);
            /*if (search_type_id < 1)
            {
                this._hasTypeList = typeList.FindAll(em => (Utils.StrToInt(em.parent_type_id) == 0
                    && this.tIdList.Contains(Utils.StrToInt(em.product_type_id))
                    ));
            }
            else
            {
                var item = ProductTypeDB.GetInfo(typeList, search_type_id);
                this._hasTypeList = typeList.FindAll(em => (
                        Utils.StrToInt(em.parent_type_id) == 0//顶级分类
                        || (Utils.StrToInt(em.parent_type_id) == Utils.StrToInt(item.parent_type_id))//同级分类
                        || em.parent_type_id.Equals(item.product_type_id)//子分类
                    )
                        && this.tIdList.Contains(Utils.StrToInt(em.product_type_id))
                 );
            }*/
            //this.AddToTypeList(typeList, search_type_id);//分类数据
        }
        #endregion

        #region FilterType
        /*private List<LuceneDocument> FilterType(List<LuceneDocument> list, string path)
        {
            if (string.IsNullOrEmpty(path))
                return list;
            return list.FindAll(
                            delegate(LuceneDocument item)
                            {
                                return item.Doc.Get("product_type_path").StartsWith(path);
                            });
        }*/
        #endregion

        #region FilterBrand
        /*private List<LuceneDocument> FilterBrand(List<LuceneDocument> list, string brand)
        {
            if (string.IsNullOrEmpty(brand))
                return list;
            return list.FindAll(
                            delegate(LuceneDocument item)
                            {
                                return item.Doc.Get("product_brand").StartsWith(brand);
                            });
        }*/
        #endregion

        #region ParseBrandList
        /*/// <summary>
        /// 获取品牌
        /// </summary>
        /// <param name="list"></param>
        private void ParseBrandList(List<LuceneDocument> list)
        {
            foreach (LuceneDocument doc in list)
            {
                string brand = doc.Doc.Get("product_brand");
                if (this._hasBrandList.Count < 120 && brand.Length > 0 && !this._hasBrandList.Contains(brand))
                {
                    this._hasBrandList.Add(brand);
                }
            }
        }*/
        #endregion

        #region GetSortField
        private ESortField GetSortField(string ocol)
        {
            ESortField sortField = ESortField.score_index;
            if (string.IsNullOrEmpty(ocol))
                return sortField;
            ocol = ocol.Trim().ToUpper();
            switch (ocol)
            {
                case "SALES":
                case "TOTALSALES"://总销量
                    {
                        sortField = ESortField.total_sale_count;
                        break;
                    }
                case "WEEKSALES"://周销量
                case "MONTHSALES"://月销量
                    {
                        sortField = ESortField.month_sale_count;
                        break;
                    }
                case "PRICE":
                case "SALEPRICE"://售价
                    {
                        sortField = ESortField.sale_price;
                        break;
                    }
                case "UPTIME":
                case "ADDTIME"://上架时间
                    {
                        sortField = ESortField.first_up_time;
                        break;
                    }
                case "GOODRATE"://好评率
                case "COMMENTCOUNT"://评论数
                    {
                        sortField = ESortField.comment_index;
                        break;
                    }
                default:
                    {
                        sortField = ESortField.score_index;
                        break;
                    }
            }
            return sortField;
        }
        #endregion

        #region 分页查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagesize"></param>
        /// <param name="pageindex"></param>
        /// <param name="keyword"></param>
        /// <param name="product_type_id"></param>
        /// <param name="brand"></param>
        /// <param name="startPrice"></param>
        /// <param name="endPrice"></param>
        /// <param name="ison"></param>
        /// <param name="isDrug"></param>
        /// <param name="is_cross"></param>
        /// <param name="allowYBLife"></param>
        /// <param name="is_free_fare"></param>
        /// <param name="is_promotion"></param>
        /// <param name="is_new"></param>
        /// <param name="shop_id"></param>
        /// <param name="shop_type"></param>
        /// <param name="ocol"></param>
        /// <param name="ot"></param>
        /// <param name="totalCount"></param>
        /// <param name="dataCount"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<ProductIndexInfo> ChangePage(int pagesize, int pageindex
            , string keyword
            , int product_type_id
            , string brand
            , int startPrice
            , int endPrice
            , int ison, int isDrug, int is_cross
            , int allowYBLife
            , int is_free_fare
            , int is_promotion
            , int is_new
            , int is_norms
            , int shop_id
            , int shop_type
            , string ocol, EOrderType ot
            , ref int totalCount
            , ref int dataCount, ref int pageCount)
        {
            if (indexUtils.IsBadWords(keyword))
                return new List<ProductIndexInfo>();//全符号的搜索

            List<ProductTypeInfo> typeList = ProductTypeDB.GetList(shop_type == 1 ? 1 : -1);//获取所有分类
            string path = "";
            if (product_type_id > 0)
            {
                path = ProductTypeDB.GetInfo(typeList, product_type_id).product_type_path;//分类路径
            }
            #region 准备参数
            if (string.IsNullOrEmpty(keyword)) keyword = "";
            ESearchMode searchMode = this.FormatSearchMode(keyword);
            string keys = keyword.Trim();
            if (!string.IsNullOrEmpty(keys)
                && searchMode.Equals(ESearchMode.KEY))
            {
                var analy = new Analysis.Analyzer();
                keys = analy.GetSplitString(keyword, ' ');
                if (keys.Length > 0)
                {
                    this._analyzerResult = keys.Split(' ').ToList<string>();
                }
                this._isSearchByKey = true;
                if (string.IsNullOrEmpty(keys.Trim()))
                    return new List<ProductIndexInfo>();//空搜索
            }
            List<Query> mustQuerys = new List<Query>();
            int maxTotal = this.maxRecords;//需要取多少条记录
            mustQuerys = this.FormatMustQuerys(searchMode, keys
                            , path
                            , brand
                            , startPrice
                            , endPrice
                            , ison
                            , isDrug
                            , is_cross
                            , allowYBLife
                            , is_free_fare
                            , is_promotion
                            , is_new
                            , shop_id
                            ,-1);
            //, shop_type
            if (is_norms >= 0 && !searchMode.Equals(ESearchMode.NORMS))
            {
                //从合规商品中搜索
                mustQuerys.Add(this.indexUtils.CreateTermQuery("norms", is_norms.ToString()));
            }
            List<Query> shouldQuerys = null;
            if (searchMode.Equals(ESearchMode.KEY) && !string.IsNullOrEmpty(keys))
            {
                shouldQuerys = this.FormatShouldQuerys(keys);
            }
            #endregion

            //排序
            ESortField sortField = this.GetSortField(ocol);

            #region 取数据
            var data = indexUtils.QueryProduct(mustQuerys, shouldQuerys, maxTotal
                , pagesize, pageindex
                , sortField, ot);
            totalCount = data.DataCount;//总记录数
            dataCount = totalCount > maxRecords ? maxRecords : totalCount;//查询的记录数
            this.ParseData(data, typeList, product_type_id, searchMode.Equals(ESearchMode.ALL));//绑定数据
            #endregion

            return this.BindList(data.Documents);
        }
        
        #endregion
    }
}
