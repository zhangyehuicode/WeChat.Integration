using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WeChat.Integration.Models
{
    /// <summary>
    /// 模板消息
    /// </summary>
    public class TemplateMessageModel
    {
        /// <summary>
        /// 发送到的OpenId
        /// </summary>
        [Description("接收者OPENID")]
        [Required]
        public string touser { get; set; }

        /// <summary>
        /// 微信消息模板ID
        /// </summary>
        [Description("微信消息模板ID")]
        [Required]
        public string template_id { get; set; }

        /// <summary>
        /// 点击模板消息后跳转到的URL
        /// </summary>
        [Description("跳转Url")]
        public string url { get; set; }

        /// <summary>
        /// 消息数据
        /// </summary>
        [Description("消息数据")]
        [Required]
        public dynamic data { get; set; }
    }
}