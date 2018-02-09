using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;

using Laobai.Model;
using Laobai.Core.Common;
using Laobai.Core.Data;
using Laobai.Core.Common.Json;
using Laobai.Core.Lucene;
using Laobai.Core.Common.Http;

namespace Laobai.WebSearch.Controllers
{
    public class HomeController : Controller
    {
        //public ConfigInfo _config = Config.GetConfig;//网站配置
        private string _token_id = "";
        private string token_id
        {
            get { return this._token_id; }
        }
        /*
        public void test()
        {
            var indexUtils = new IndexUtils("");
            string val = "飞利浦新安怡9安士自然啜饮杯260ml12个月以上SCF782/00（红色）";
            //Response.Write(indexUtils.FormatString(val));
            Response.Write("<br/>" + new Laobai.Core.Analysis.Analyzer().GetSplitString(val, ' '));
        }*/

        //
        // GET: /Home/

        #region Index
        [ValidateInput(false)]
        public ActionResult Index()
        {
            DateTime time1 = DateTime.Now;
            string json = "";
            #region 获取参数
            if (Request.Form.Keys.Count > 0)
            {
                json = Request.Form[0];
            }
            else
            {
                return Json(this.JsonError("Query", 0x00002, "JSON解析错误"), JsonRequestBehavior.AllowGet);
            }
            #endregion
            Logger.Log(json);
            var val = this.Query(json);
            Logger.Log(JsonHelper.ObjectToJson(val));
            Logger.Log(">>>总的执行时间>>>" + (DateTime.Now - time1).Milliseconds.ToString() + "毫秒");

            return val;
        }
        #endregion

        #region Query 查询网关
        private ActionResult Query(string json)
        {
            ParseJson parseJson = new ParseJson();
            SortedDictionary<string, string> mJson = parseJson.Parse(json);
            string oKey = parseJson.GetJsonValue(mJson, "key");
            string headerJson = parseJson.GetJsonValue(mJson, "header");
            var bodyJson = parseJson.GetJsonValue(mJson, "body");
            SortedDictionary<string, string> headerList = parseJson.Parse(headerJson);
            int shop_type = Utils.StrToInt(parseJson.GetJsonValue(headerList, "shop_type"), 1);

            var _cache = new DoCache();//缓存
            var cacheKey = (oKey + "$" + bodyJson + "$" + shop_type).ToLower();
            var cacheData = _cache.GetCache(cacheKey);
            if (cacheData != null)
            {
            //    return (ActionResult)cacheData;
            }
            ActionResult returnJson = null;

            string sKey = oKey.ToLower();
            string _tokenId = parseJson.GetJsonValue(headerList, "token_id");
            if (!string.IsNullOrEmpty(_tokenId))
                this._token_id = _tokenId;
            
            switch (sKey)
            {
                case "searchproduct"://商品搜索
                    {
                        returnJson = Json(this.QueryProduct(bodyJson, headerJson, shop_type, json), JsonRequestBehavior.AllowGet);
                        break;
                    }
                case "getsuggest"://搜索提示
                    {
                        returnJson = Json(this.QuerySuggest(bodyJson), JsonRequestBehavior.AllowGet);
                        //Logger.Log(JsonHelper.ObjectToJson(returnJson));
                        break;
                    }
                case "guessyoulike"://猜您喜欢
                    {
                        returnJson = Json(this.QueryGuessYouLike(bodyJson, shop_type), JsonRequestBehavior.AllowGet);
                        break;//QueryGuessYouLike
                    }
                case "searchsameproduct"://同品牌或同分类
                    {
                        returnJson = Json(this.QueryProductAll(bodyJson, shop_type), JsonRequestBehavior.AllowGet);
                        break;
                    }
                case "searchwmproduct"://微脉商品查询
                    {
                        returnJson = Json(this.QueryWmProduct(bodyJson, headerJson,shop_type, json), JsonRequestBehavior.AllowGet);
                        break;
                    }
                case "searchrecommendproduct"://同品牌/同分类/猜您喜欢三合一
                    {
                        returnJson = Json(this.QuerySearchRecommendProduct(bodyJson, shop_type), JsonRequestBehavior.AllowGet);
                        break;
                    }
                default:
                    {
                        returnJson = Json(this.JsonError(oKey, 0x00004, "协议未定义"), JsonRequestBehavior.AllowGet);
                        break;
                    }
            }
            var jsonString = JsonHelper.ObjectToJson(returnJson);
            if(jsonString.IndexOf("\"ret_code\":0")>0)
                _cache.SetCache(cacheKey, returnJson);//缓存60分钟

            return returnJson;
        }
        #endregion

        #region JSON解析错误
        private Response<SearchResponseProductBody> JsonError(string jsonKey, int code = 0x00002, string msg = "JSON解析错误")
        {
            Response<SearchResponseProductBody> request = new Response<SearchResponseProductBody>();
            request.key = jsonKey;
            request.header = new ResponseHeader()
            {
                token_id = this.token_id,
                ret_result = new ResponseResult() { ret_code = code, ret_msg = msg }
            };
            return request;
        }
        private Response<SearchResponseWMProductBody> JsonWmError(string jsonKey, int code = 0x00002, string msg = "JSON解析错误")
        {
            Response<SearchResponseWMProductBody> request = new Response<SearchResponseWMProductBody>();
            request.key = jsonKey;
            request.header = new ResponseHeader()
            {
                token_id = this.token_id,
                ret_result = new ResponseResult() { ret_code = code, ret_msg = msg }
            };
            return request;
        }
        #endregion

        #region 解析价格
        private void ParsePrice(string range, ref int min, ref int max)
        {
            if (!string.IsNullOrEmpty(range)
                    && Utils.IsMatch(range, @"^\d+\-\d+$"))
            {
                string[] arr = range.Split('-');
                min = Utils.StrToInt(arr[0]);
                max = Utils.StrToInt(arr[1]);
            }
        }
        #endregion

        #region 获取数据
        private List<ProductTypeInfo> _resetTypeList = new List<ProductTypeInfo>();
        private List<string> _resetBrandList = new List<string>();
        private List<ProductIndexInfo> GetData(SearchRequestProductBody sBody, int shop_type
            , ProductReader reader
            , EOrderType ot
            , ref int totalCount
            , ref int dataCount, ref int pageCount)
        {
            if (string.IsNullOrEmpty(sBody.sort_column))
                sBody.sort_column = "default";
            int pagesize = sBody.page_size < 1 ? 1 : sBody.page_size;
            int pageindex = sBody.page_no < 1 ? 1 : sBody.page_no;
            string sKey = sBody.search_word;
            if (string.IsNullOrEmpty(sKey))
                sKey = "";
            if (sKey.Length > 200)
                sKey = sKey.Substring(0, 200);//关键词长度最长200个字
            int startPrice = -1;
            int endPrice = -1;
            this.ParsePrice(sBody.price_range, ref startPrice, ref endPrice);
            //int dataCount = 0;
            //int pageCount = 0;
            List<ProductIndexInfo> list = reader.ChangePage(pagesize, pageindex//读取数据
                , sKey
                , Utils.StrToInt(sBody.product_type_id, -1)
                , sBody.product_brand
                , startPrice
                , endPrice
                , Utils.StrToInt(sBody.is_on_sale, 1)
                , Utils.StrToInt(sBody.is_drug, -1)
                , Utils.StrToInt(sBody.is_cross_border, -1)
                , Utils.StrToInt(sBody.allow_ebaolife, -1)
                , Utils.StrToInt(sBody.is_free_fare, -1)
                , Utils.StrToInt(sBody.is_promotion, -1)
                , Utils.StrToInt(sBody.is_new, -1)
                , Utils.StrToInt(sBody.is_norms, -1)
                , Utils.StrToInt(sBody.shop_id, -1)
                , shop_type
                , sBody.sort_column
                , ot
                , ref totalCount
                , ref dataCount, ref pageCount);
            this._resetTypeList = reader.HasTypeList;
            this._resetBrandList = reader.HasBrandList;
            /*if (list.Count > 0)
            {
                _cache.SetCache(cacheKey, list);
                _cache.SetCache("reader$" + cacheKey, reader);
            }*/
            return list;
        }
        #endregion

        #region 微脉获取数据
        private List<ProductIndexWmInfo> WmGetData(SearchRequestWMProductBody sBody, int shop_type
           , WmProductReader reader
           , EOrderType ot
           , ref int totalCount
           , ref int dataCount, ref int pageCount)
        {
            int pagesize = sBody.page_size < 1 ? 1 : sBody.page_size;
            int pageindex = sBody.page_no < 1 ? 1 : sBody.page_no;
            string sKey = sBody.search_word;
            if (string.IsNullOrEmpty(sKey))
                sKey = "";
            if (sKey.Length > 200)
                sKey = sKey.Substring(0, 200);//关键词长度最长200个字
            List<ProductIndexWmInfo> list = reader.ChangePage(pagesize, pageindex//读取数据
                , sKey
                , -1
                , ""
                , -1
                , -1
                , 1
                , -1
                , -1
                , -1
                , -1
                , -1
                , -1
                , -1
                , -1
                , shop_type
                , "default"
                , ot
                , ref totalCount
                , ref dataCount, ref pageCount);
            /*if (list.Count > 0)
            {
                _cache.SetCache(cacheKey, list);
                _cache.SetCache("reader$" + cacheKey, reader);
            }*/
            return list;
        }
        #endregion

        #region 绑定数据
        private List<SearchProduct> BindSearchList(List<ProductIndexInfo> list)
        {
            List<SearchProduct> sList = new List<SearchProduct>();
            foreach (ProductIndexInfo p in list)
            {
                SearchProduct spro = new SearchProduct();
                spro.product_id = p.product_id.ToString();
                spro.hot_count = p.score_index.ToString();
                spro.score = p.score.ToString();//关键词匹配度
                sList.Add(spro);
            }
            return sList;
        }
        private Response<SearchResponseProductBody> BindData(List<ProductIndexInfo> list, ProductReader reader
            , string jsonKey, int totalCount, int dataCount)
        {
            SearchResponseProductBody searchResult = new SearchResponseProductBody();
            #region 查询结果
            searchResult.product_list = this.BindSearchList(list);
            searchResult.total_count = totalCount.ToString();
            searchResult.rec_num = dataCount.ToString();
            searchResult.type_list = this._resetTypeList;
            var _brandArray = new List<ResponseBrand>();
            var _brandList = this._resetBrandList;
            foreach (string s in _brandList)
            {
                ResponseBrand v = new ResponseBrand();
                v.brand_name = s;
                _brandArray.Add(v);
            }
            searchResult.brand_list = _brandArray;
            searchResult.has_drug = reader.HasDrug.ToString().ToLower();
            searchResult.has_cross_border = reader.HasCross.ToString().ToLower();
            searchResult.has_new = reader.HasNew.ToString().ToLower();
            searchResult.spliter_words = reader.AnalyzerResult;
            searchResult.analyzer_words = reader.AnalyzerResult;
            #endregion

            Response<SearchResponseProductBody> response = new Response<SearchResponseProductBody>();
            response.key = jsonKey;
            response.body = searchResult;
            response.header = new ResponseHeader()
            {
                token_id = this.token_id,
                ret_result = new ResponseResult() { ret_code = 0, ret_msg = "OK" }
            };
            return response;
        }
        #endregion

        #region 微脉绑定数据
        private List<SearchProduct> BindWmSearchList(List<ProductIndexWmInfo> list)
        {
            List<SearchProduct> sList = new List<SearchProduct>();
            foreach (ProductIndexWmInfo p in list)
            {
                SearchProduct spro = new SearchProduct();
                spro.product_id = p.product_id.ToString();
                spro.hot_count = p.score_index.ToString();
                spro.score = p.score.ToString();//关键词匹配度
                sList.Add(spro);
            }
            return sList;
        }
        private Response<SearchResponseWMProductBody> BindWmData(List<ProductIndexWmInfo> list, WmProductReader reader
            , string jsonKey, int totalCount, int dataCount)
        {
            SearchResponseWMProductBody searchResult = new SearchResponseWMProductBody();
            #region 查询结果
            searchResult.product_list = this.BindWmSearchList(list);
            searchResult.rec_num = dataCount.ToString();
            #endregion

            Response<SearchResponseWMProductBody> response = new Response<SearchResponseWMProductBody>();
            response.key = jsonKey;
            response.body = searchResult;
            response.header = new ResponseHeader()
            {
                token_id = this.token_id,
                ret_result = new ResponseResult() { ret_code = 0, ret_msg = "OK" }
            };
            return response;
        }
        #endregion

        #region QuerySuggest 搜索提示
        private Response<ResponseWord> QuerySuggest(string json)
        {
            //if (Request.Form.Keys.Count>0)
            //json = Request.Form[0];
            string jsonKey = "GetSuggest";
            try
            {
                SearchRequestWordBody sBody = JsonHelper.JsonToObject<SearchRequestWordBody>(json);
                int pagesize = sBody.page_size < 1 ? 1 : sBody.page_size;
                string sKey = sBody.search_word;
                if (string.IsNullOrEmpty(sKey))
                    sKey = sBody.seach_word;
                WordReader reader = new WordReader();

                List<WordInfo> list = reader.Query(sKey, pagesize);
                ResponseWord repWord = new ResponseWord();
                repWord.word_list = list;
                repWord.rec_num = list.Count;

                Response<ResponseWord> request = new Response<ResponseWord>();
                request.body = repWord;
                request.key = jsonKey;
                request.header = new ResponseHeader()
                {
                    token_id = this.token_id,
                    ret_result = new ResponseResult() { ret_code = 0, ret_msg = "OK" }
                };

                return request;
            }
            catch (Exception e)
            {
                this.WriteLog(e, jsonKey, json);
                Response<ResponseWord> request = new Response<ResponseWord>();
                request.key = jsonKey;
                request.header = new ResponseHeader()
                {
                    token_id = this.token_id,
                    ret_result = new ResponseResult() { ret_code = 0x00002, ret_msg = "JSON解析错误" }
                };
                return request;
            }
        }
        #endregion

        #region QueryProduct 商品搜索
        private Response<SearchResponseProductBody> QueryProduct(string json, string header, int shop_type = 1, string logJson="")
        {
            string jsonKey = "SearchProduct";
            try
            {
                //ParseJson parse = new ParseJson();
                //SortedDictionary<string, string> mJson = parse.Parse(json);
                SearchRequestProductBody sBody = JsonHelper.JsonToObject<SearchRequestProductBody>(json);
                int totalCount = 0;//搜索命中的记录数
                int dataCount = 0;//返回的记录数
                int pageCount = 0;
                EOrderType ot = EOrderType.DESC;
                if (!string.IsNullOrEmpty(sBody.sort_type))
                {
                    ot = sBody.sort_type.Trim().ToLower().Equals("asc") ? EOrderType.ASC : EOrderType.DESC;
                }
                
                ProductReader reader = new ProductReader();                
                List<ProductIndexInfo> list = this.GetData(sBody, shop_type
                    , reader
                    , ot
                    , ref totalCount
                    , ref dataCount, ref pageCount);

                var request = this.BindData(list, reader, jsonKey, totalCount, dataCount);
                
                if (reader.IsSearchByKey)
                {
                    //关键词搜索的情况
                    request.body.type_list = this.ResetTypeList(sBody, shop_type, ot);
                }

                if (!string.IsNullOrEmpty(sBody.product_brand))
                {
                    //品牌不为空的情况
                    request.body.brand_list = this.ResetBrandList(sBody, shop_type, ot);
                }
                /**/
                #region 记录关键词使用频率
                /*if (sBody.page_no == 1)
                {
                    try
                    {//记录关键词使用频率
                        RequestHeader sHeader = JsonHelper.JsonToObject<RequestHeader>(header);
                        string userKey = string.IsNullOrEmpty(sHeader.dev_no) ? "" : sHeader.dev_no;//设备号
                        userKey += "$";
                        userKey += (string.IsNullOrEmpty(sHeader.ip_addr) ? "" : sHeader.ip_addr);//IP
                        WordDB.ModifyWordUseFreq(reader.AnalyzerResult, userKey);
                    }
                    catch (Exception)
                    {

                    }
                }*/
                #endregion

                return request;
            }
            catch (Exception e)
            {
                this.WriteLog(e, jsonKey, json, header, logJson);
                return this.JsonError(jsonKey, 0x00002, "Json解析失败");
            }
        }
        /// <summary>
        /// 重置分类列表
        /// </summary>
        /// <param name="sBody"></param>
        /// <param name="shop_type"></param>
        /// <param name="ot"></param>
        /// <returns></returns>
        private List<ProductTypeInfo> ResetTypeList(SearchRequestProductBody sBody, int shop_type, EOrderType ot)
        {
            int totalCount = 0;
            int dataCount = 0;
            int pageCount = 0;
            
            sBody.product_type_id = "-1";
            /*
            sBody.product_brand = ""; 
            sBody.is_new = "-1";
            sBody.is_drug = "-1";
            sBody.is_cross_border = "-1";
            sBody.is_free_fare = "-1";
            sBody.is_promotion = "-1";
            sBody.shop_id = "-1";*/
            ProductReader reader = new ProductReader();
            List<ProductIndexInfo> list = this.GetData(sBody, shop_type
                , reader
                , ot
                , ref totalCount
                , ref dataCount, ref pageCount);
            return this._resetTypeList;
        }
        /// <summary>
        /// 重置品牌列表
        /// </summary>
        /// <param name="sBody"></param>
        /// <param name="shop_type"></param>
        /// <param name="ot"></param>
        /// <returns></returns>
        private List<ResponseBrand> ResetBrandList(SearchRequestProductBody sBody, int shop_type, EOrderType ot)
        {
            int totalCount = 0;
            int dataCount = 0;
            int pageCount = 0;
            sBody.product_brand = "";
            ProductReader reader = new ProductReader();
            List<ProductIndexInfo> list = this.GetData(sBody, shop_type
                , reader
                , ot
                , ref totalCount
                , ref dataCount, ref pageCount);
            var brandList = new List<ResponseBrand>();
            foreach (string s in this._resetBrandList)
            {
                brandList.Add(new ResponseBrand() {  brand_name = s});
            }
            return brandList;
        }
        #endregion

        #region WmQueryProduct 微脉商品搜索
        private Response<SearchResponseWMProductBody> QueryWmProduct(string json, string header, int shop_type = 1, string logJson = "")
        {
            string jsonKey = "SearchWMProduct";
            try
            {
                //ParseJson parse = new ParseJson();
                //SortedDictionary<string, string> mJson = parse.Parse(json);
                SearchRequestWMProductBody sBody = JsonHelper.JsonToObject<SearchRequestWMProductBody>(json);
                int totalCount = 0;//搜索命中的记录数
                int dataCount = 0;//返回的记录数
                int pageCount = 0;
                EOrderType ot = EOrderType.DESC;
                //if (!string.IsNullOrEmpty(sBody.sort_type))
                //{
                //    ot = sBody.sort_type.Trim().ToLower().Equals("asc") ? EOrderType.ASC : EOrderType.DESC;
                //}

                WmProductReader reader = new WmProductReader();
                List<ProductIndexWmInfo> list = this.WmGetData(sBody, shop_type
                    , reader
                    , ot
                    , ref totalCount
                    , ref dataCount, ref pageCount);

                var request = this.BindWmData(list, reader, jsonKey, totalCount, dataCount);

                return request;
            }
            catch (Exception e)
            {
                this.WriteLog(e, jsonKey, json, header, logJson);
                return this.JsonWmError(jsonKey, 0x00002, "Json解析失败");
            }
        }
        #endregion

        #region QueryGuessYouLike 猜您喜欢
        private Response<SearchResponseProductBody> QueryGuessYouLike(string json, int shop_type = 1, string jsonKey = "GuessYouLike")
        {
            try
            {
                SearchRequestProductBody sBody = JsonHelper.JsonToObject<SearchRequestProductBody>(json);
                int totalCount = 0;//搜索命中的数量
                int dataCount = 0;//搜索返回的记录数
                int pageCount = 0;
                EOrderType ot = EOrderType.DESC;
                if (!string.IsNullOrEmpty(sBody.sort_type))
                {
                    ot = sBody.sort_type.Trim().ToLower() == "asc" ? EOrderType.ASC : EOrderType.DESC;
                }
                List<string> removeList = sBody.remove_products;
                if (removeList == null) removeList = new List<string>();
                int pagesize = sBody.page_size;
                sBody.page_size += removeList.Count;
                if (string.IsNullOrEmpty(sBody.search_word))
                    sBody.search_word = "@all";

                ProductReader reader = new ProductReader();
                List<ProductIndexInfo> list = this.GetData(sBody, shop_type
                    , reader
                    , ot
                    , ref totalCount
                    , ref dataCount, ref pageCount);
                list.RemoveAll(delegate(ProductIndexInfo item)
                            {
                                return removeList.Contains(item.product_id.ToString());
                            });
                if (list.Count > pagesize && pagesize > 0)
                {
                    list = list.GetRange(0, pagesize);
                }
                var response = this.BindData(list, reader, jsonKey, totalCount, dataCount);
                response.body.brand_list = new List<ResponseBrand>();//清空品牌
                response.body.type_list = new List<ProductTypeInfo>();//清空分类
                return response;
            }
            catch (Exception e)
            {
                this.WriteLog(e, jsonKey, json);
                return this.JsonError(jsonKey, 0x00002);
            }
        }
        #endregion

        #region QueryProductAll 同品牌或同分类商品
        private Response<SearchResponseProductBody> QueryProductAll(string json, int shop_type = 1)
        {
            return this.QueryGuessYouLike(json, shop_type, "SearchSameProduct");
        }
        #endregion

        #region QuerySearchRecommendProduct 同品牌/同分类/猜您喜欢三合一
        private Response<ResponseSearchRecommendProductBody> QuerySearchRecommendProduct(string json, int shop_type = 1)
        {
            string jsonKey = "SearchRecommendProduct";
            try
            {
                RequestSearchRecommendProductBody sBody = JsonHelper.JsonToObject<RequestSearchRecommendProductBody>(json);

                Response<ResponseSearchRecommendProductBody> request = new Response<ResponseSearchRecommendProductBody>();
                request.body = new ResponseSearchRecommendProductBody();
                if (sBody.youlike != null)
                {
                    var yLike = this.QueryGuessYouLike(JsonHelper.ObjectToJson(sBody.youlike), shop_type);
                    request.body.youlike_rec_num = yLike.body.rec_num;
                    request.body.youlike_product_list = yLike.body.product_list;
                }
                if (sBody.sametype != null)
                {
                    var yLike = this.QueryGuessYouLike(JsonHelper.ObjectToJson(sBody.sametype), shop_type);
                    request.body.sametype_rec_num = yLike.body.rec_num;
                    request.body.sametype_product_list = yLike.body.product_list;
                }
                if (sBody.samebrand != null)
                {
                    var yLike = this.QueryGuessYouLike(JsonHelper.ObjectToJson(sBody.samebrand), shop_type);
                    request.body.samebrand_rec_num = yLike.body.rec_num;
                    request.body.samebrand_product_list = yLike.body.product_list;
                }
                
                request.key = jsonKey;
                request.header = new ResponseHeader()
                {
                    token_id = this.token_id,
                    ret_result = new ResponseResult() { ret_code = 0, ret_msg = "OK" }
                };
                return request;
            }
            catch (Exception e)
            {
                this.WriteLog(e, jsonKey, json);
                Response<ResponseSearchRecommendProductBody> request = new Response<ResponseSearchRecommendProductBody>();
                request.key = jsonKey;
                request.header = new ResponseHeader()
                {
                    token_id = this.token_id,
                    ret_result = new ResponseResult() { ret_code = 0x00002, ret_msg = "JSON解析错误" }
                };
                return request;
            }
        }
        #endregion

        private void WriteLog(Exception e, string jsonKey, string json = "", string header = "", string logJson = "")
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("jsonKey: " + jsonKey);
            sb.AppendLine("jsonHeader: " + header);
            sb.AppendLine("jsonBody: " + json);
            sb.AppendLine("jsonData: " + logJson);
            sb.AppendLine(e.ToString());
            Logger.Error(sb.ToString());
        }
    }
}
