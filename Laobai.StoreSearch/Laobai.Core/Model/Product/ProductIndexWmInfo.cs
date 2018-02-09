using System;
using System.Runtime.Serialization;

namespace Laobai.Model
{
    /// <summary>
    /// ProductIndexWmInfo:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [DataContract]
    public class ProductIndexWmInfo
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
