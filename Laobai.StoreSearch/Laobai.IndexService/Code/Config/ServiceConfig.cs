using System;

namespace Laobai.IndexService
{
    public class ServiceConfig
    {
        /// <summary>
        /// 词库每页读取的数据
        /// </summary>
        public static int PageSize_Word
        {
            get { return 1000; }
        }
        /// <summary>
        /// 商品每页读取的数据
        /// </summary>
        public static int PageSize_Product
        {
            get { return 800; }
        }
        /// <summary>
        /// 更新商品索引的间隔时间(单位分钟)
        /// </summary>
        public static int IntervalMinute
        {
            get { return 30; }
        }
    }
}
