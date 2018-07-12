using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

using Laobai.Model;
using Laobai.Core.Common;
using Laobai.Lucene.Documents;
using Laobai.Core.Common.Json;

namespace Laobai.Core.Lucene
{
    public class ProductWriter
    {
        
        private IndexUtils indexUtils;
        public ProductWriter()
        {
            this.indexUtils = new IndexUtils("product");//初始化
        }

        private List<string> NormsTypePath()
        {
            List<string> list = new List<string>();
            list.Add("0,1,");//OTC药品
            list.Add("0,2,");//营养保健
            list.Add("0,3,");//医疗器械
            //list.Add("0,4,");//成人用品
            //list.Add("0,5,");//母婴护理
            list.Add("0,257,");//隐形眼镜
            list.Add("0,613,");//中药参茸
            return list;
        }
        /// <summary>
        /// 是否合规类目
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsNormsType(string path)
        {
            var list = this.NormsTypePath();
            foreach (string s in list)
            {
                if (path.IndexOf(s) >= 0)
                    return true;
            }
            return false;
        }

        #region 初始化搜索字段
        /// <summary>
        /// 初始化搜索字段
        /// </summary>
        /// <param name="sname"></param>
        /// <param name="scommon"></param>
        /// <param name="sbrand"></param>
        /// <param name="skey"></param>
        /// <param name="strait"></param>
        /// <param name="stype"></param>
        /// <param name="stext"></param>
        /// <param name="slicense"></param>
        /// <param name="sstore"></param>
        /// <returns></returns>
        private List<Field> FormatFields(string sname, string scommon, string sbrand
            , string skey, string strait, string stype, string stext, string slicense, string sstore)
        {
            List<Field> list = new List<Field>();

            #region 设置搜索字段
            list.Add(this.indexUtils.AnalyzedField("sname", sname, 1.00f));//商品名
            list.Add(this.indexUtils.AnalyzedField("sbrand", sbrand, 0.99f));//品牌名
            list.Add(this.indexUtils.AnalyzedField("skey", skey, 0.90f));//关键词
            list.Add(this.indexUtils.AnalyzedField("stype", stype, 0.10f));//商品分类

            //list.Add(this.indexUtils.AnalyzedField("scommon", scommon, 0.8f));//通用名
            //list.Add(this.indexUtils.AnalyzedField("strait", strait, 0.5f));//卖点
            //list.Add(this.indexUtils.AnalyzedField("stext", stext, 0.1f));//商品说明
            //list.Add(this.indexUtils.AnalyzedField("slicense", slicense, 0.1f));//批准文号
            //list.Add(this.indexUtils.AnalyzedField("sstore", sstore, 0.05f));//商家名称

            StringBuilder text = new StringBuilder();
            text.Append(sname);//商品名
            if (!string.IsNullOrEmpty(scommon))
                text.Append(" ").Append(scommon);//通用名
            text.Append(" ").Append(sbrand);//品牌
            text.Append(" ").Append(skey);//关键词
            text.Append(" ").Append(strait);//卖点
            text.Append(" ").Append(stype);//分类名称路径
            text.Append(" ").Append(stext);//商品说明
            text.Append(" ").Append(slicense);//批准文号
            text.Append(" ").Append(sstore);//商家名称

            list.Add(this.indexUtils.AnalyzedField("maxtext", text.ToString(), 0.05f));//关键词
            #endregion

            return list;
        }
        #endregion

        #region 初始化Document内容
        private Document CreateDocument(ProductInfo item)
        {
            Document doc = new Document();

            #region 初始化
            doc.Add(this.indexUtils.UnAnalyzedField("product_id", item.product_id.ToString()));
            doc.Add(this.indexUtils.UnAnalyzedField("product_code", item.product_code));
            doc.Add(this.indexUtils.UnAnalyzedField("shop_id", item.shop_id));
            //doc.Add(this.indexUtils.UnAnalyzedField("shop_type", item.shop_type));
            //doc.Add(this.indexUtils.UnAnalyzedField("product_name", item.product_name));
            doc.Add(this.indexUtils.UnAnalyzedField("product_brand", item.product_brand));

            #region 设置搜索字段
            var list = this.FormatFields(item.product_name, item.common_name, item.product_brand
                , item.search_key, item.sales_promotion, item.product_type_name_path
                , item.product_func, item.product_license, item.shop_name);
            foreach (Field field in list)
            {
                doc.Add(field);
            }
            #endregion
            doc.Add(this.indexUtils.UnAnalyzedField("product_type_id", item.product_type_id));
            doc.Add(this.indexUtils.UnAnalyzedField("product_type_path", item.product_type_path));

            doc.Add(this.indexUtils.BooleanField("is_on_sale", item.is_on_sale));
            doc.Add(this.indexUtils.BooleanField("is_drug", item.is_drug));
            doc.Add(this.indexUtils.BooleanField("allow_ebaolife", item.allow_ebaolife));
            doc.Add(this.indexUtils.BooleanField("allow_qj", (item.allow_qj.IndexOf("1") == 0) ? "1" : "0"));
            doc.Add(this.indexUtils.BooleanField("allow_klf", (item.allow_qj.IndexOf("3") >= 0) ? "1" : "0"));
            //doc.Add(this.indexUtils.BooleanField("is_cross", item.is_cross_border));
            doc.Add(this.indexUtils.BooleanField("is_free_fare", item.is_free_fare));
            doc.Add(this.indexUtils.BooleanField("is_promotion", item.is_promotion));

            //doc.Add(this.indexUtils.BooleanField("has_stock", item.stock_num));//是否有库存
            
            //排序用的字段
            doc.Add(this.indexUtils.LongField("first_up_time"
                , Utils.GetTimeStamp(Utils.StrToDateTime(item.up_time))));//上架时间(存储的是时间戳,方便搜索)
            doc.Add(this.indexUtils.FloatField("sale_price", item.sale_price));//售价
            doc.Add(this.indexUtils.IntField("total_sale_count", this.IsBadDocument(item) ? "0" : item.total_sale_count));//总销量
            doc.Add(this.indexUtils.IntField("month_sale_count", this.IsBadDocument(item) ? "0" : item.month_sale_count));//月销量
            //排序指数if (this.IsBadDocument(item
            doc.Add(this.indexUtils.FloatField("score_index", this.GetTotalScore(item)));//综合指数
            //doc.Add(this.indexUtils.FloatField("sales_index", this.GetSalesIndex(item)));//销售指数
            doc.Add(this.indexUtils.FloatField("comment_index", this.GetCommentIndex(item)));//评论指数
            //
            doc.Add(this.indexUtils.AnalyzedField("norms", this.IsNormsType(item.product_type_path) ? "1" : "0"));//合规商品
            doc.Add(this.indexUtils.AnalyzedField("all", "all"));
            #endregion

            return doc;
        }
        #endregion

        #region 判断是否需要过滤的商品(不放入索引)
        FilterInfo filter = Settings.Filter;//过滤规则
        /// <summary>
        /// 判断是否需要过滤的商品(不放入索引)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsFilter(ProductInfo item)
        {
            if (this.filter.filter_products.Contains(item.product_id))
                return true;//过滤的商品

            if (this.filter.filter_stores.Contains(Utils.StrToInt(item.shop_id)))
                return true;//过滤的商家

            if (this.filter.filter_types.Contains(Utils.StrToInt(item.product_type_id)))
                return true;//过滤的类目

            string val = item.product_brand + " " + item.product_name;
            foreach (var s in this.filter.filter_keys)
            {
                if (!string.IsNullOrEmpty(s) && val.IndexOf(s) >= 0)
                    return true;//品牌/标题中包含过滤词
            }
            return false;
        }
        #endregion

        #region 计算评分指数
        private const int badScore = -99999999;
        List<RuleInfo> rules = Settings.Priority;//降权规则
        /// <summary>
        /// 是否需要除权(评分为-99999999分)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsBadDocument(ProductInfo item)
        {
            //|| Utils.StrToInt(item.stock_num) < 1
            if (Utils.StrToInt(item.is_on_sale) != 1
                || Utils.StrToInt(item.is_visible) != 1
                )
                return true;
            return this.MatchRule(item);
        }
        /// <summary>
        /// 是否匹配降权规则
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool MatchRule(ProductInfo item)
        {
            DateTime now = DateTime.Now;
            string name = item.product_brand + " " + item.product_name;
            foreach (RuleInfo rule in this.rules)
            {
                if (now < Utils.StrToDateTime(rule.time_start) || now > Utils.StrToDateTime(rule.time_end))
                    continue;
                if (rule.store_id == Utils.StrToInt(item.shop_id))
                    return true;//商家匹配
                if (rule.type_path.StartsWith(item.product_type_path))
                    return true;//分类匹配
                foreach (string s in rule.keys)
                {
                    if (!string.IsNullOrEmpty(s) && name.IndexOf(s) >= 0)
                        return true;//匹配关键词
                }
            }
            return false;
        }
        private Random _random = new Random();
        /// <summary>
        /// 获取综合排序的评分
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private float GetTotalScore(ProductInfo item)
        {
            //float score = 0f;
            if (this.IsBadDocument(item))
                return badScore;//如果是下架或无库存的商品评分为-10
            float score = this.GetSalesIndex(item)//销售指数
                        + Utils.StrToFloat(item.sort_no);//人工排序
            DateTime first_up_time = Utils.StrToDateTime(item.up_time);
            int diffDays = Utils.DateDiffDays(first_up_time, DateTime.Now);//上架时间
            //加权
            //if (item.shop_id.Equals("206"))//京东商品
            //{
            //    score += 0.1f;//加权
            //}
            //加权
            if (item.is_promotion.Equals("1"))//促销商品
            {
                score += 0.1f;//加权
            }
            if (diffDays <= 30)//新上架商品
            {
                if (diffDays > 0)//上架超过1天
                {
                    float daysIndex = 30.0f / diffDays;
                    score += Utils.StrToFloat(item.month_sale_count) * daysIndex * Utils.StrToFloat(item.sale_price) / 150000.0f;
                }
                else
                {
                    score += 0.1f;//新上架未满1天的商品
                }
            }
            return score;
        }
        /// <summary>
        /// 获取好评指数
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private float GetGoodRate(ProductInfo item)
        {
            if (this.IsBadDocument(item))
                return -1;//如果是下架或无库存的商品评分为-10
            float _rate = 1;//好评率
            if (Utils.StrToInt(item.comment_count) > 10)
            {
                //总评论数大于10的情况
                _rate = (Utils.StrToFloat(item.good_count) + Utils.StrToFloat(item.normal_count)) / Utils.StrToFloat(item.comment_count);
            }
            return _rate;
        }
        /// <summary>
        /// 获取销售指数
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private float GetSalesIndex(ProductInfo item)
        {
            if (this.IsBadDocument(item))
                return badScore;//如果是下架或无库存的商品评分为-10
            float score = 0f;
            float _rate = this.GetGoodRate(item);//好评率          
            //月销量*单价*非差评率/15万
            float _score_01 = Utils.StrToFloat(item.month_sale_count) * Utils.StrToFloat(item.sale_price) / 150000.0f;            
            //总销量*非差评率
            float _score_02 = Utils.StrToFloat(item.total_sale_count) / 50000.0f;       
            //月点击
            float _score_03 = Utils.StrToFloat(item.month_click_count) / 10000.0f;    
            score = (_score_01 > 1 ? 1 : _score_01) //月销量*单价*非差评率/15万
                        + (_score_02 > 0.2f ? 0.2f : _score_02) ////总销量*非差评率
                        + (_score_03 > 0.1f ? 0.1f : _score_03);//月点击
            score *= _rate;
            return score;
        }
        /// <summary>
        /// 获取评论指数
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private float GetCommentIndex(ProductInfo item)
        {
            if (this.IsBadDocument(item))
                return badScore;//如果是下架或无库存的商品评分为-10
            //float score = 0f;
            float _rate = this.GetGoodRate(item);//好评率
            int comment_count = Utils.StrToInt(item.comment_count);
            if (comment_count < 10)
            {
                return 0.001f * comment_count;//少于10的评论
            }
            return _rate * comment_count;
        }
        #endregion

        #region 清除已被删除的商品索引
        /// <summary>
        /// 清除已被删除的商品索引
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<ProductInfo> ResetList(List<ProductInfo> list)
        {
            List<ProductInfo> newList = new List<ProductInfo>();
            List<string> idList = new List<string>();//需要删除的索引内容
            foreach (ProductInfo p in list)
            {
               
                if (p.product_id > 0 && !idList.Contains(p.product_id.ToString()))
                    idList.Add(p.product_id.ToString());
                if (Utils.StrToInt(p.is_visible) == 1 && !this.IsFilter(p))
                    newList.Add(p);           
            }            
            this.indexUtils.RemoveDocuments("product_id", idList);
            return newList;
        }
        #endregion

        #region 创建索引
        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="list"></param>
        public void CreateIndex(List<ProductInfo> list)
        {
            list = this.ResetList(list);
            if (list.Count < 1)
                return;
            List<SetLuceneDocument> docs = new List<SetLuceneDocument>();
            foreach (ProductInfo item in list)
            {
                docs.Add(new SetLuceneDocument(item.product_id.ToString(), this.CreateDocument(item)));
            }
            this.indexUtils.CreateIndex("product_id", docs);
        }
        #endregion
    }
}
