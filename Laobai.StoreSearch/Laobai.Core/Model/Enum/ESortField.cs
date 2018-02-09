using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laobai.Model
{
    public enum ESortField
    {
        /// <summary>
        /// 综合评分
        /// </summary>
        score_index=0,
        /// <summary>
        /// 售价
        /// </summary>
        sale_price=1,
        /// <summary>
        /// 总销量
        /// </summary>
        total_sale_count=2,
        /// <summary>
        /// 月销量
        /// </summary>
        month_sale_count=3,
        /// <summary>
        /// 评论指数
        /// </summary>
        comment_index=4,
        /// <summary>
        /// 首次上架时间
        /// </summary>
        first_up_time=5
    }
}
