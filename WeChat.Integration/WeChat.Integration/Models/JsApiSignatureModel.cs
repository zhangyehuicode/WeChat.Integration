using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.Integration.Models
{
    /// <summary>
    /// JsApiSignature
    /// </summary>
    public class JsApiSignatureModel
    {
        /// <summary>
        /// JsApiSignature
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="nonceStr">随机字符串</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="signature">签名</param>
        public JsApiSignatureModel(string appId, string nonceStr, string timestamp, string signature)
        {
            this.appId = appId;
            this.nonceStr = nonceStr;
            this.timestamp = timestamp;
            this.signature = signature;
        }

        /// <summary>
        /// appId
        /// </summary>
        public string appId { get; set; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        public string nonceStr { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string signature { get; set; }

        public override string ToString()
        {
            return $"appId[{appId}],nonceStr[{nonceStr}],timestamp[{timestamp}],signature[{signature}]";
        }
    }
}