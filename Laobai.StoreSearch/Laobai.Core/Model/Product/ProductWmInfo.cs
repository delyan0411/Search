using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Laobai.Model
{
    /// <summary>
    /// ProductWmInfo:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [DataContract]
    public partial class ProductWmInfo : ProductInfo
    {
        #region Model
        private string _departments = "";

        private string _symptom = "";

        /// <summary>
        /// 科室
        /// </summary>
        [DataMember]
        public string departments
        {
            get
            {
                return this._departments;
            }
            set
            {
                this._departments = value;
            }
        }

        /// <summary>
        /// 病症
        /// </summary>
        [DataMember]
        public string symptom
        {
            get
            {
                return this._symptom;
            }
            set
            {
                this._symptom = value;
            }
        }
        #endregion Model
    }

    #region RequestWmProductBody
    [DataContract]
    public class RequestWmProductBody
    {
        [DataMember]
        public string page_no { set; get; }

        [DataMember]
        public string page_size { set; get; }
    }
    #endregion

    #region RequestWmModifyProductBody
    [DataContract]
    public class RequestWmModifyProductBody
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

    #region ResponseWmProduct
    [Serializable]
    [DataContract]
    public class ResponseWmProduct : ResponseListBody
    {
        private List<ProductWmInfo> _product_list = new List<ProductWmInfo>();

        [DataMember]
        public List<ProductWmInfo> product_list
        {
            get { return _product_list; }
            set { _product_list = value; }
        }
    }
    #endregion

    #region 响应搜索请求的参数
    [DataContract]
    public class SearchRequestWMProductBody
    {
        [DataMember]
        public string search_word
        {
            get;
            set;
        }
        [DataMember]
        public string departmentsname
        {
            get;
            set;
        }
        [DataMember]
        public string symptomname
        {
            get;
            set;
        }
        //[DataMember]
        //public List<SymptomInfo> symptom_list
        //{
        //    get;
        //    set;
        //}
        [DataMember]
        public int page_no
        {
            get;
            set;
        }

        [DataMember]
        public int page_size
        {
            get;
            set;
        }
    }
    #endregion


    #region 输出搜索结果的参数
    [DataContract]
    public class SearchResponseWMProductBody
    {
        [DataMember]
        public string rec_num
        {
            get;
            set;
        }

        [DataMember]
        public List<SearchProduct> product_list
        {
            get;
            set;
        }
    }
    #endregion
}