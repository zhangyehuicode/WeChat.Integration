using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.Integration.Models
{
    /// <summary>
    /// 商户微信数据
    /// </summary>
    public class MchWeChatData
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// AccessToken
        /// </summary>
        public WeChatAccessToken AccessToken { get; set; }

        /// <summary>
        /// JsApiTicket
        /// </summary>
        public WeChatApiTicket JsApiTicket { get; set; }
    }
}