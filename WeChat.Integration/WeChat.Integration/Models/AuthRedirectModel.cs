using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.Integration.Models
{
    /// <summary>
    /// AuthRedirect模型
    /// </summary>
    public class AuthRedirectModel
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 回调Url
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// 是否需要用户信息
        /// </summary>
        public bool NeedUserInfo { get; set; }

        /// <summary>
        /// 是否是snsapi_userinfo scope
        /// </summary>
        public bool IsUserInfoScope { get; set; }
    }
}