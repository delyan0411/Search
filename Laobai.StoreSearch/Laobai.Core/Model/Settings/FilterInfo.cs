using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laobai.Model
{
    /// <summary>
    /// 过滤
    /// </summary>
    public class FilterInfo
    {
        private List<int> _filter_products = new List<int>();
        /// <summary>
        /// 过滤商品
        /// </summary>
        public List<int> filter_products
        {
            get { return _filter_products; }
            set { _filter_products = value; }
        }
        private List<int> _filter_types = new List<int>();
        /// <summary>
        /// 过滤分类
        /// </summary>
        public List<int> filter_types
        {
            get { return _filter_types; }
            set { _filter_types = value; }
        }
        private List<int> _filter_stores = new List<int>();
        /// <summary>
        /// 过滤商家
        /// </summary>
        public List<int> filter_stores
        {
            get { return _filter_stores; }
            set { _filter_stores = value; }
        }
        private List<string> _filter_keys = new List<string>();
        /// <summary>
        /// 过滤包含的关键词
        /// </summary>
        public List<string> filter_keys
        {
            get { return _filter_keys; }
            set { _filter_keys = value; }
        }
    }
}
