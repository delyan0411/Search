using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Laobai.Model
{
    [DataContract]
    public partial class ProductTypeInfo
    {
        #region
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
        private string _type_name = "";
        /// <summary>
        /// 分类名称
        /// </summary>
        [DataMember]
        public string type_name
        {
            get { return _type_name; }
            set { _type_name = value; }
        }
        private string _parent_type_id = "0";
        /// <summary>
        /// 父分类ID
        /// </summary>
        [DataMember]
        public string parent_type_id
        {
            get { return _parent_type_id; }
            set { _parent_type_id = value; }
        }
        private string _is_visible = "1";
        /// <summary>
        /// 是否显示
        /// </summary>
        [DataMember]
        public string is_visible
        {
            get { return _is_visible; }
            set { _is_visible = value; }
        }
        private string _sort_no = "0";
        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public string sort_no
        {
            get { return _sort_no; }
            set { _sort_no = value; }
        }
        #endregion
    }

    #region RequestProductTypeBody
    [DataContract]
    public class RequestProductTypeBody
    {
        [DataMember]
        public string is_visible { set; get; }
    }
    #endregion

    [Serializable]
    [DataContract]
    public class ResponseProductType : ResponseListBody
    {
        private List<ProductTypeInfo> _type_list = new List<ProductTypeInfo>();

        [DataMember]
        public List<ProductTypeInfo> type_list
        {
            get { return _type_list; }
            set { _type_list = value; }
        }
    }
}
