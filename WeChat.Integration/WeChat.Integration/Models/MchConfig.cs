using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.Integration.Models
{
    /// <summary>
    /// 商户微信配置文件
    /// </summary>
    public class MchConfig
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// AppID(应用ID)
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// AppSecret(应用密钥)
        /// </summary>
        public string AppSecret { get; set; }
    }
}