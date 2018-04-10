using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.Integration.Models
{
    /// <summary>
    /// 微信AccessToken
    /// </summary>
    public partial class WeChatAccessToken
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public int expires_in { get; set; }
    }

    public partial class WeChatAccessToken
    {
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireDate { get; set; }
    }
}