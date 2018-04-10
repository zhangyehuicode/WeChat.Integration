using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.Integration.Models
{
    /// <summary>
    /// 微信api_ticket
    /// </summary>
    public partial class WeChatApiTicket
    {
        /// <summary>
        /// 错误码，0表示正常
        /// </summary>
        public int errcode { get; set; }

        /// <summary>
        /// 错误信息，ok 表示正常
        /// </summary>
        public string errmsg { get; set; }

        /// <summary>
        /// api_ticket，签名所需凭证
        /// </summary>
        public string ticket { get; set; }

        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public int expires_in { get; set; }
    }

    public partial class WeChatApiTicket
    {
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireDate { get; set; }
    }
}