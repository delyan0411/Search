using System;

namespace Laobai.Model
{
    public enum ESearchMode
    {
        /// <summary>
        /// 按关键词搜索
        /// </summary>
        KEY = 0,
        /// <summary>
        /// 按商品编码搜索
        /// </summary>
        CODE = 1,
        /// <summary>
        /// 搜所有商品
        /// </summary>
        ALL = 2,
        /// <summary>
        /// 合规商品(OTC药品 中药参茸 营养保健 医疗器械 成人用品 隐形眼镜 母婴护理)
        /// </summary>
        NORMS = 3
    }
}
