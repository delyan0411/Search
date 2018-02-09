using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Laobai.Model
{
    [DataContract]
    public class Request<T>
    {
        [DataMember]
        public T body { set; get; }

        [DataMember]
        public string key { set; get; }

        [DataMember]
        public RequestHeader header { set; get; }
    }

    [DataContract]
    public class RequestHeader
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        [DataMember]
        public string token_id { set; get; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public string user_id { set; get; }

        /// <summary>
        /// 设备平台
        /// </summary>
        [DataMember]
        public string dev_plat { set; get; }

        /// <summary>
        /// 设备版本
        /// </summary>
        [DataMember]
        public string dev_ver { set; get; }

        /// <summary>
        /// 手机端-设备号，浏览器端-自动生成的cookie
        /// </summary>
        [DataMember]
        public string dev_no { set; get; }

        /// <summary>
        /// 设备型号
        /// </summary>
        [DataMember]
        public string dev_model { set; get; }

        /// <summary>
        /// 软件版本
        /// </summary>
        [DataMember]
        public string soft_ver { set; get; }

        /// <summary>
        /// IOS推送ID
        /// </summary>
        [DataMember]
        public string push_id { set; get; }

        /// <summary>
        /// 请求端IP
        /// </summary>
        [DataMember]
        public string ip_addr { set; get; }

    }

    [DataContract]
    public class RequestBodyEmpty
    {
    }

    [DataContract]
    public class Request : Request<RequestBodyEmpty>
    {
        //private RequestBodyEmpty _body = new RequestBodyEmpty();

        //[DataMember]
        //public RequestBodyEmpty body 
        //{
        //    set { this._body = value; }
        //    get { return this._body; } 
        //}
    }

}
