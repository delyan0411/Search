using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Laobai.Model
{
    [DataContract]
    public class Response<T>
    {
        [DataMember]
        public string key { set; get; }

        [DataMember]
        public T body { set; get; }

        [DataMember]
        public ResponseHeader header { set; get; }
    }

    [DataContract]
    public class ResponseBodyEmpty 
    {
    }
    [DataContract]
    public class ResponseListBody
    {
        /// <summary>
        /// 记录数
        /// </summary>
        [DataMember]
        public int rec_num { set; get; }
    }
    [DataContract]
    public class Response : Response<ResponseBodyEmpty>
    {
    }

    [DataContract]
    public class ResponseHeader
    {
        [DataMember]
        public string token_id { set; get; }

        [DataMember]
        public ResponseResult ret_result { set; get; }
    }

    [DataContract]
    public class ResponseResult
    {
        [DataMember]
        public int ret_code { set; get; }

        [DataMember]
        public string ret_msg { set; get; }

        public ResponseResult() { }
        public ResponseResult(int ret_code, string ret_msg)
        {
            this.ret_code = ret_code;
            this.ret_msg = ret_msg;
        }
    }
}
