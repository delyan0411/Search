using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laobai.Model;
using Laobai.Core;
using Laobai.Core.Common.Json;
using Laobai.Core.Common;

namespace Laobai.Core.Data
{
    public class ProductWmDB
    {
        #region 获取所有有效微脉商品信息
        /// <summary>
        /// 获取所有有效微脉商品信息
        /// </summary>
        /// <param name="size"></param>
        /// <param name="pageIndex"></param>
        /// <param name="dataCount"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public static List<ProductWmInfo> GetList(int size, int pageIndex, ref int dataCount, ref int pageCount)
        {
            RequestWmProductBody body = new RequestWmProductBody() { page_size = size.ToString(), page_no = pageIndex.ToString() };

            var response = DBHelper<ResponseWmProduct>.GetResponse("GetWMValidProduct", JsonHelper.ObjectToJson(body));

            if (DBHelper<ResponseWmProduct>.IsOK(response) && response.body!=null)
            {
                var responseBody = response.body;
                dataCount = responseBody.rec_num;
                pageCount = Utils.GetPageCount(dataCount, size);
                return responseBody.product_list;
            }
            return new List<ProductWmInfo>();
        }
        #endregion

        #region 获取所有更新的微脉商品信息
        /// <summary>
        /// 获取所有更新的微脉商品信息
        /// </summary>
        /// <param name="size"></param>
        /// <param name="pageIndex"></param>
        /// <param name="dataCount"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public static List<ProductWmInfo> GetModifyList(int size, int pageIndex
            , DateTime startDate
            , DateTime endDate
            , ref int dataCount, ref int pageCount)
        {
            List<ProductWmInfo> list = new List<ProductWmInfo>();

            DateTime date = startDate;
            if (startDate > endDate)
            {
                startDate = endDate;
                endDate = date;
            }
            RequestWmModifyProductBody body = new RequestWmModifyProductBody()
            {
                page_size = size.ToString(),
                page_no = pageIndex.ToString(),
                start_time = startDate.ToString("yyyy-MM-dd") + " 00:00:00",
                end_time = endDate.ToString("yyyy-MM-dd") + " 23:59:59"
            };

            var response = DBHelper<ResponseWmProduct>.GetResponse("GetWMModifyProduct", JsonHelper.ObjectToJson(body));

            if (DBHelper<ResponseWmProduct>.IsOK(response) && response.body != null)
            {
                var responseBody = response.body;
                dataCount = responseBody.rec_num;
                pageCount = Utils.GetPageCount(dataCount, size);
                return responseBody.product_list;
            }

            return new List<ProductWmInfo>();
        }
        #endregion
    }
}