using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.Integration.Models
{
    /// <summary>
    /// 微信用户信息（snsapi_userinfo）
    /// </summary>
    public class WeChatUserSnsInfo
    {
        /// <summary>
        /// 用户是否订阅该公众号标识，值为0时，代表此用户没有关注该公众号，拉取不到其余信息。
        /// </summary>
        public virtual string subscribe
        {
            get { return string.IsNullOrEmpty(unionid) ? "0" : "1"; }
            set { }
        }

        /// <summary>
        /// 用户的标识，对当前公众号唯一
        /// </summary>
        public virtual string openid { get; set; }

        /// <summary>
        /// 用户的昵称
        /// </summary>
        public virtual string nickname { get; set; }

        /// <summary>
        /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        /// </summary>
        public virtual string sex { get; set; }

        /// <summary>
        /// 用户所在城市
        /// </summary>
        public virtual string city { get; set; }

        /// <summary>
        /// 用户所在省份
        /// </summary>
        public virtual string province { get; set; }

        /// <summary>
        /// 用户所在国家
        /// </summary>
        public virtual string country { get; set; }

        /// <summary>
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空。若用户更换头像，原有头像URL将失效
        /// </summary>
        public virtual string headimgurl { get; set; }

        /// <summary>
        /// 只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段。
        /// </summary>
        public virtual string unionid { get; set; }

        public override string ToString()
        {
            return $"subscribe={subscribe}&openid={openid}&nickname={nickname}&sex={sex}&city={city}&province={province}&country={country}&headimgurl={headimgurl}";
        }

        /// <summary>
        /// 获取基础信息字符串
        /// </summary>
        /// <returns></returns>
        public virtual string GetBasicInfoString()
        {
            return $"subscribe={subscribe}&openid={openid}&nickname={nickname}&headimgurl={headimgurl}";
        }
    }
}