using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Laobai.Model
{
    /// <summary>
    /// ProductInfo:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [DataContract]
    public partial class ProductInfo
    {
        #region Model
            private int _product_id = 0;
            /// <summary>
            /// 商品ID
            /// </summary>
            [DataMember]
            public int product_id
            {
                get { return _product_id; }
                set { _product_id = value; }
            }
        //private string _shop_type = "1";
        ///// <summary>
        ///// 商城类型1-老白，2-硬件商城，3-健之安，默认为1
        ///// </summary>
        //[DataMember]
        //public string shop_type
        //{
        //    get { return _shop_type; }
        //    set { _shop_type = value; }
        //}
            private string _product_code = "";
            /// <summary>
            /// 商品编码
            /// </summary>
            [DataMember]
            public string product_code
            {
                get { return _product_code; }
                set { _product_code = value; }
            }
            private string _product_name = "";
            /// <summary>
            /// 商品名称
            /// </summary>
            [DataMember]
            public string product_name
            {
                get { return _product_name; }
                set { _product_name = value; }
            }
            private string _common_name = "";
            /// <summary>
            /// 通用名
            /// </summary>
            [DataMember]
            public string common_name
            {
                get { return _common_name; }
                set { _common_name = value; }
            }
            private string _shop_id = "0";
            /// <summary>
            /// 商家ID
            /// </summary>
            [DataMember]
            public string shop_id
            {
                get { return _shop_id; }
                set { _shop_id = value; }
            }
            private string _shop_name = "";
            [DataMember]
            public string shop_name
            {
                get { return _shop_name; }
                set { _shop_name = value; }
            }
            private string _product_type_id = "0";
            /// <summary>
            /// 分类ID
            /// </summary>
            [DataMember]
            public string product_type_id
            {
                get { return _product_type_id; }
                set { _product_type_id = value; }
            }
            private string _product_type_path = "";
            /// <summary>
            /// 分类路径
            /// </summary>
            [DataMember]
            public string product_type_path
            {
                get { return _product_type_path; }
                set { _product_type_path = value; }
            }
            private string _product_type_name_path = "";
            [DataMember]
            public string product_type_name_path
            {
                get { return _product_type_name_path; }
                set { _product_type_name_path = value; }
            }
            private string _sale_price = "0";
            /// <summary>
            /// 售价
            /// </summary>
            [DataMember]
            public string sale_price
            {
                get { return _sale_price; }
                set { _sale_price = value; }
            }
            private string _mobile_price = "0";
            /// <summary>
            /// 手机售价
            /// </summary>
            [DataMember]
            public string mobile_price
            {
                get { return _mobile_price; }
                set { _mobile_price = value; }
            }
            private string _product_brand = "";
            /// <summary>
            /// 品牌名
            /// </summary>
            [DataMember]
            public string product_brand
            {
                get { return _product_brand; }
                set { _product_brand = value; }
            }
        //private string _first_up_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        ///// <summary>
        ///// 首次上架时间
        ///// </summary>
        //[DataMember]
        //public string first_up_time
        //{
        //    get { return _first_up_time; }
        //    set { _first_up_time = value; }
        //}
            private string _up_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            /// <summary>
            /// 上架时间
            /// </summary>
            [DataMember]
            public string up_time
            {
                get { return _up_time; }
                set { _up_time = value; }
            }
            private string _down_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            /// <summary>
            /// 下架时间
            /// </summary>
            [DataMember]
            public string down_time
            {
                get { return _down_time; }
                set { _down_time = value; }
            }
            private string _search_key = "";
            /// <summary>
            /// 搜索关键词
            /// </summary>
            [DataMember]
            public string search_key
            {
                get { return _search_key; }
                set { _search_key = value; }
            }
            private string _product_func = "";
            /// <summary>
            /// 商品说明
            /// </summary>
            [DataMember]
            public string product_func
            {
                get { return _product_func; }
                set { _product_func = value; }
            }
            private string _product_spec = "";
            /// <summary>
            /// 规格
            /// </summary>
            [DataMember]
            public string product_spec
            {
                get { return _product_spec; }
                set { _product_spec = value; }
            }
            private string _sales_promotion = "";
            /// <summary>
            /// 卖点
            /// </summary>
            [DataMember]
            public string sales_promotion
            {
                get { return _sales_promotion; }
                set { _sales_promotion = value; }
            }
            private string _manu_facturer = "";
            /// <summary>
            /// 厂家
            /// </summary>
            [DataMember]
            public string manu_facturer
            {
                get { return _manu_facturer; }
                set { _manu_facturer = value; }
            }
            private string _product_license = "";
            /// <summary>
            /// 批准文号
            /// </summary>
            [DataMember]
            public string product_license
            {
                get { return _product_license; }
                set { _product_license = value; }
            }
            private string _is_visible = "1";
            /// <summary>
            /// 是否在回收站
            /// </summary>
            [DataMember]
            public string is_visible
            {
                get { return _is_visible; }
                set { _is_visible = value; }
            }
            private string _is_on_sale = "0";
            /// <summary>
            /// 是否上架0=否;1=是
            /// </summary>
            [DataMember]
            public string is_on_sale
            {
                get { return _is_on_sale; }
                set { _is_on_sale = value; }
            }
            private string _is_drug = "0";
            /// <summary>
            /// 是否处方药0=否;1=是
            /// </summary>
            [DataMember]
            public string is_drug
            {
                get { return _is_drug; }
                set { _is_drug = value; }
            }
            private string _allow_ebaolife = "-1";
            /// <summary>
            /// 是否允许医卡通付款0=否;1=是
            /// </summary>
            [DataMember]
            public string allow_ebaolife
            {
                get { return _allow_ebaolife; }
                set { _allow_ebaolife = value; }
            }
            private string _is_free_fare = "-1";
            /// <summary>
            /// 是否免邮
            /// </summary>
            [DataMember]
            public string is_free_fare
            {
                get { return _is_free_fare; }
                set { _is_free_fare = value; }
            }

            private string _is_promotion = "-1";
            /// <summary>
            /// 是否促销
            /// </summary>
            [DataMember]
            public string is_promotion
            {
                get { return _is_promotion; }
                set { _is_promotion = value; }
            }

            private string _modify_index_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            /// <summary>
            /// 索引更新时间
            /// </summary>
            [DataMember]
            public string modify_index_time
            {
                get { return _modify_index_time; }
                set { _modify_index_time = value; }
            }
            private string _modify_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            /// <summary>
            /// 商品更新时间
            /// </summary>
            [DataMember]
            public string modify_time
            {
                get { return _modify_time; }
                set { _modify_time = value; }
            }

            private string _total_sale_count = "0";
            /// <summary>
            /// 总销量
            /// </summary>
            [DataMember]
            public string total_sale_count
            {
                get { return _total_sale_count; }
                set { _total_sale_count = value; }
            }
            private string _month_sale_count = "0";
            /// <summary>
            /// 月销量
            /// </summary>
            [DataMember]
            public string month_sale_count
            {
                get { return _month_sale_count; }
                set { _month_sale_count = value; }
            }
            private string _month_click_count = "0";
            /// <summary>
            /// 月点击量
            /// </summary>
            [DataMember]
            public string month_click_count
            {
                get { return _month_click_count; }
                set { _month_click_count = value; }
            }
            private string _week_sale_count = "0";
            /// <summary>
            /// 周销量
            /// </summary>
            [DataMember]
            public string week_sale_count
            {
                get { return _week_sale_count; }
                set { _week_sale_count = value; }
            }
            private string _comment_count = "0";
            /// <summary>
            /// 总评论数
            /// </summary>
            [DataMember]
            public string comment_count
            {
                get { return _comment_count; }
                set { _comment_count = value; }
            }
            private string _good_count = "0";
            /// <summary>
            /// 好评数
            /// </summary>
            [DataMember]
            public string good_count
            {
                get { return _good_count; }
                set { _good_count = value; }
            }
            private string _normal_count = "0";
            /// <summary>
            /// 中评数
            /// </summary>
            [DataMember]
            public string normal_count
            {
                get { return _normal_count; }
                set { _normal_count = value; }
            }
            private string _bad_count = "0";
            /// <summary>
            /// 差评数
            /// </summary>
            [DataMember]
            public string bad_count
            {
                get { return _bad_count; }
                set { _bad_count = value; }
            }
            private string _click_count = "0";
            /// <summary>
            /// 总点击
            /// </summary>
            [DataMember]
            public string click_count
            {
                get { return _click_count; }
                set { _click_count = value; }
            }
            private string _sort = "0";
            /// <summary>
            /// 排序
            /// </summary>
            [DataMember]
            public string sort_no
            {
                set { _sort = value; }
                get { return _sort; }
            }

        //private string _stock_num = "0";

        ///// <summary>
        ///// 商品库存
        ///// </summary>
        //[DataMember]
        //public string stock_num
        //{
        //    set { _stock_num = value; }
        //    get { return _stock_num; }
        //}

        //private string _is_cross_border = "0";
        ///// <summary>
        ///// 是否跨境商品
        ///// </summary>
        //[DataMember]
        //public string is_cross_border
        //{
        //    set { _is_cross_border = value; }
        //    get { return _is_cross_border; }
        //}

        //private string _is_new = "-1";
        ///// <summary>
        ///// 是否新品
        ///// </summary>
        //[DataMember]
        //public string is_new
        //{
        //    set { _is_new = value; }
        //    get { return _is_new; }
        //}
        #endregion Model
    }

    #region RequestProductBody
    [DataContract]
    public class RequestProductBody
    {
        [DataMember]
        public string page_no { set; get; }

        [DataMember]
        public string page_size { set; get; }
    }
    #endregion

    #region RequestModifyProductBody
    [DataContract]
    public class RequestModifyProductBody
    {
        [DataMember]
        public string page_no { set; get; }

        [DataMember]
        public string page_size { set; get; }
        /// <summary>
        /// yyyy-MM-dd hh:mm:ss
        /// </summary>
        [DataMember]
        public string start_time { set; get; }
        /// <summary>
        /// yyyy-MM-dd hh:mm:ss
        /// </summary>
        [DataMember]
        public string end_time { set; get; }
    }
    #endregion

    [Serializable]
    [DataContract]
    public class ResponseProduct : ResponseListBody
    {
        private List<ProductInfo> _product_list = new List<ProductInfo>();

        [DataMember]
        public List<ProductInfo> product_list
        {
            get { return _product_list; }
            set { _product_list = value; }
        }
    }

    #region ResponseBrand
    [DataContract]
    public class ResponseBrand
    {
        private string _brand_name = "";

        [DataMember]
        public string brand_name
        {
            get { return _brand_name; }
            set { _brand_name = value; }
        }
    }
    #endregion

    #region 响应搜索请求的参数
    [DataContract]
    public class SearchRequestProductBody
    {
        [DataMember]
        public string search_word { set; get; }
        [DataMember]
        public string product_type_id { set; get; }
        [DataMember]
        public string product_brand { set; get; }
        [DataMember]
        public string is_on_sale { get; set; }
        [DataMember]
        public string is_drug { set; get; }

        private string _is_cross_border = "-1";
        [DataMember]
        public string is_cross_border { set { _is_cross_border = value; } get { return _is_cross_border; } }

        private string _allow_ebaolife = "-1";
        [DataMember]
        public string allow_ebaolife { set; get; }

        private string _is_free_fare = "-1";
        [DataMember]
        public string is_free_fare { get; set; }

        private string _is_promotion = "-1";
        [DataMember]
        public string is_promotion { get; set; }
        [DataMember]
        public string price_range { set; get; }
        [DataMember]
        public string sort_column { set; get; }
        [DataMember]
        public string sort_type { set; get; }

        private string _is_new = "-1";
        [DataMember]
        public string is_new { set { _is_new = value; } get { return _is_new; } }

        private string _shop_id = "-1";
        [DataMember]
        public string shop_id { set { _shop_id = value; } get { return _shop_id; } }

        private List<string> _remove_products = new List<string>();
        [DataMember]
        public List<string> remove_products { set { _remove_products = value; } get { return _remove_products; } }

        [DataMember]
        public int page_no { set; get; }

        [DataMember]
        public int page_size { set; get; }

        private string _is_norms = "-1";
        [DataMember]
        public string is_norms { set { _is_norms = value; } get { return _is_norms; } }
    }
    #endregion

    #region 商品搜索结果的商品信息
    [DataContract]
    public class SearchProduct
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public string product_id { get; set; }
        /// <summary>
        /// 默认排序的数值
        /// </summary>
        [DataMember]
        public string hot_count { get; set; }


        /// <summary>
        /// 关键词匹配度
        /// </summary>
        [DataMember]
        public string score { get; set; }
        /*
        /// <summary>
        /// 好评率评分
        /// </summary>
        [DataMember]
        public string good_rate { get; set; }
        
        /// <summary>
        /// 月销量指数
        /// </summary>
        [DataMember]
        public string month_indexs { get; set; }

        /// <summary>
        /// 总销量指数
        /// </summary>
        [DataMember]
        public string total_indexs { get; set; }

        /// <summary>
        /// 月点击指数
        /// </summary>
        [DataMember]
        public string click_indexs { get; set; }
        
        /// <summary>
        /// 人工排序指数
        /// </summary>
        [DataMember]
        public string mark_indexs { get; set; }
         */
    }
    #endregion

    #region 输出搜索结果的参数
    [DataContract]
    public class SearchResponseProductBody
    {
        [DataMember]
        public List<ProductTypeInfo> type_list { set; get; }
        [DataMember]
        public List<ResponseBrand> brand_list { set; get; }
        [DataMember]
        public List<string> spliter_words { get; set; }
        [DataMember]
        public List<string> analyzer_words { get; set; }
        [DataMember]
        public string has_drug { set; get; }

        [DataMember]
        public string has_cross_border { set; get; }

        [DataMember]
        public string has_new { set; get; }

        [DataMember]
        public string total_count { set; get; }

        [DataMember]
        public string rec_num { set; get; }

        [DataMember]
        public List<SearchProduct> product_list { set; get; }
    }
    #endregion

    #region 猜您喜欢
    [DataContract]
    public class GuessResponseProductBody
    {
        [DataMember]
        public string rec_num { set; get; }
        [DataMember]
        public List<SearchProduct> product_list { set; get; }
    }
    #endregion

    #region 同品牌/同分类/猜您喜欢三合一
    [DataContract]
    public class RequestSearchRecommendProductBody
    {
        [DataMember]
        public SearchRequestProductBody youlike { set; get; }

        [DataMember]
        public SearchRequestProductBody sametype { set; get; }

        [DataMember]
        public SearchRequestProductBody samebrand { set; get; }
    }
    #endregion

    #region 同品牌/同分类/猜您喜欢三合一
    [DataContract]
    public class ResponseSearchRecommendProductBody
    {
        [DataMember]
        public string youlike_rec_num { set; get; }
        [DataMember]
        public List<SearchProduct> youlike_product_list { set; get; }

        [DataMember]
        public string sametype_rec_num { set; get; }
        [DataMember]
        public List<SearchProduct> sametype_product_list { set; get; }

        [DataMember]
        public string samebrand_rec_num { set; get; }
        [DataMember]
        public List<SearchProduct> samebrand_product_list { set; get; }
    }
    #endregion
}