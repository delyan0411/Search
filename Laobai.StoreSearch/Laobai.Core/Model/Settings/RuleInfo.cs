using System;
using System.Collections.Generic;

namespace Laobai.Model
{
    public enum RuleMode
    {
        /// <summary>
        /// 权重设定为指定的值
        /// </summary>
        LOCK=0,
        /// <summary>
        /// 加权
        /// </summary>
        ADD=1,
        /// <summary>
        /// 降权
        /// </summary>
        CUT=2,
        /// <summary>
        /// 直接屏蔽
        /// </summary>
        HIDE=4
    }

    public class RuleInfo
    {
        private string _time_start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        /// <summary>
        /// 生效时间
        /// </summary>
        public string time_start
        {
            get { return _time_start; }
            set { _time_start = value; }
        }
        private string _time_end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        /// <summary>
        /// 失效时间
        /// </summary>
        public string time_end
        {
            get { return _time_end; }
            set { _time_end = value; }
        }
        /*
        private string _mode = "lock";
        /// <summary>
        /// 模式
        /// </summary>
        public string mode
        {
            get { return _mode; }
            set { _mode = value; }
        }
        */
        private int _store_id = 0;
        /// <summary>
        /// 商家ID
        /// </summary>
        public int store_id
        {
            get { return _store_id; }
            set { _store_id = value; }
        }
        private string _type_path = "";
        /// <summary>
        /// 分类路径
        /// </summary>
        public string type_path
        {
            get { return _type_path; }
            set { _type_path = value; }
        }
        private List<int> _products = new List<int>();
        /// <summary>
        /// 商品ID
        /// </summary>
        public List<int> products
        {
            get { return _products; }
            set { _products = value; }
        }
        private List<string> _keys = new List<string>();
        /// <summary>
        /// 关键词
        /// </summary>
        public List<string> keys
        {
            get { return _keys; }
            set { _keys = value; }
        }
        /*
        private float _val = 0;
        /// <summary>
        /// 值
        /// </summary>
        public float val
        {
            get { return _val; }
            set { _val = value; }
        }
        /// <summary>
        /// 格式化模式
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static RuleMode FormatMode(string val)
        {
            RuleMode mode = RuleMode.LOCK;
            if (string.IsNullOrEmpty(val))
                return mode;

            val = val.ToLower().Trim();
            switch (val)
            {
                case "lock":
                    mode = RuleMode.LOCK;
                    break;
                case "add":
                    mode = RuleMode.ADD;
                    break;
                case "cut":
                    mode = RuleMode.CUT;
                    break;
                case "hide":
                    mode = RuleMode.HIDE;
                    break;
            }

            return mode;
        }*/
    }
}
