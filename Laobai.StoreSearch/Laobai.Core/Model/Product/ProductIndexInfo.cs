using System;
using System.Runtime.Serialization;

namespace Laobai.Model
{
    /// <summary>
    /// ProductIndexInfo:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [DataContract]
    public class ProductIndexInfo
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
        /*private int _shop_type = 1;
        /// <summary>
        /// 商城类型1-老白，2-硬件商城，3-健之安，默认为1
        /// </summary>
        [DataMember]
        public int shop_type
        {
            get { return _shop_type; }
            set { _shop_type = value; }
        }
        private string _product_code = "";
        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string product_code
        {
            get { return _product_code; }
            set { _product_code = value; }
        }*/
        /*private string _product_name = "";
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string product_name
        {
            get { return _product_name; }
            set { _product_name = value; }
        }*/

        private int _shop_id = 0;
        /// <summary>
        /// 商家ID
        /// </summary>
        [DataMember]
        public int shop_id
        {
            get { return _shop_id; }
            set { _shop_id = value; }
        }

        private int _product_type_id = 0;
        /// <summary>
        /// 分类ID
        /// </summary>
        [DataMember]
        public int product_type_id
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
        /*
        private decimal _sale_price = 0m;
        /// <summary>
        /// 售价
        /// </summary>
        [DataMember]
        public decimal sale_price
        {
            get { return _sale_price; }
            set { _sale_price = value; }
        }
        */
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
        /*
        private long _first_up_time = 0;
        /// <summary>
        /// 首次上架时间(时间戳)
        /// </summary>
        [DataMember]
        public long first_up_time
        {
            get { return _first_up_time; }
            set { _first_up_time = value; }
        }

        private int _is_on_sale = 0;
        /// <summary>
        /// 是否上架0=否;1=是
        /// </summary>
        [DataMember]
        public int is_on_sale
        {
            get { return _is_on_sale; }
            set { _is_on_sale = value; }
        }
        private int _is_drug = 0;
        /// <summary>
        /// 是否处方药0=否;1=是
        /// </summary>
        [DataMember]
        public int is_drug
        {
            get { return _is_drug; }
            set { _is_drug = value; }
        }
        private int _allow_ebaolife = -1;
        /// <summary>
        /// 是否允许医卡通付款0=否;1=是
        /// </summary>
        [DataMember]
        public int allow_ebaolife
        {
            get { return _allow_ebaolife; }
            set { _allow_ebaolife = value; }
        }
        private int _is_free_fare = -1;
        /// <summary>
        /// 是否免邮
        /// </summary>
        [DataMember]
        public int is_free_fare
        {
            get { return _is_free_fare; }
            set { _is_free_fare = value; }
        }

        private int _is_promotion = -1;
        /// <summary>
        /// 是否促销
        /// </summary>
        [DataMember]
        public int is_promotion
        {
            get { return _is_promotion; }
            set { _is_promotion = value; }
        }


        private int _is_cross_border = 0;
        /// <summary>
        /// 是否跨境商品
        /// </summary>
        [DataMember]
        public int is_cross_border
        {
            set { _is_cross_border = value; }
            get { return _is_cross_border; }
        }

        private int _total_sale_count = 0;
        /// <summary>
        /// 总销量
        /// </summary>
        [DataMember]
        public int total_sale_count
        {
            get { return _total_sale_count; }
            set { _total_sale_count = value; }
        }*/
        /*
        private int _month_sale_count = 0;
        /// <summary>
        /// 月销量
        /// </summary>
        [DataMember]
        public int month_sale_count
        {
            get { return _month_sale_count; }
            set { _month_sale_count = value; }
        }
        */
        private float _score_index = 0;
        /// <summary>
        /// 综合评分指数
        /// </summary>
        [DataMember]
        public float score_index
        {
            get { return _score_index; }
            set { _score_index = value; }
        }
        /* private float _sales_index = 0;
         /// <summary>
         /// 销售指数
         /// </summary>
         [DataMember]
         public float sales_index
         {
             get { return _sales_index; }
             set { _sales_index = value; }
         }*/
        private float _comment_index = 0;
        /// <summary>
        /// 评论指数
        /// </summary>
        [DataMember]
        public float comment_index
        {
            get { return _comment_index; }
            set { _comment_index = value; }
        }
        private float _score = 0;
        /// <summary>
        /// 评分评分
        /// </summary>
        [DataMember]
        public float score
        {
            get { return _score; }
            set { _score = value; }
        }
        #endregion Model
    }
}
