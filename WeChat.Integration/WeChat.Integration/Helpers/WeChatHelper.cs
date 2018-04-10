using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using WeChat.Integration.Models;

namespace WeChat.Integration.Helpers
{
    /// <summary>
    /// 微信操作帮助类
    /// </summary>
    public class WeChatHelper
    {
        #region fields
        static DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        const string URL_GetAccessToken = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
        const string URL_GetJsApiTicket = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi";
        const string URL_GetAuthorize = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state=yunmai#wechat_redirect";
        const string URL_GetAuthAccessToken = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
        const string URL_SendTemplate = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}";
        const string URL_GetUserBasicInfo = "https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN";
        const string URL_GetUserSnsInfo = "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN";
        #endregion

        #region ctor.
        private WeChatHelper(MchConfig mchConfig)
        {
            CurrentMchConfig = mchConfig;
        }

        static WeChatHelper()
        {
            InitData();
        }
        #endregion

        #region properties
        /// <summary>
        /// 商户配置信息
        /// </summary>
        public MchConfig CurrentMchConfig { get; set; }

        #region MchConfigList
        /// <summary>
        /// 商户微信配置文件
        /// </summary>
        static List<MchConfig> MchConfigList { get; set; }

        /// <summary>
        /// 商户微信数据
        /// </summary>
        static List<MchWeChatData> MchWeChatDataList { get; set; }
        #endregion
        #endregion

        #region static methods
        /// <summary>
        /// 初始化数据
        /// </summary>
        static void InitData()
        {
            try
            {
                var mchConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/MchConfig.json");
                MchConfigList = LoadFileData<List<MchConfig>>(mchConfigFile);
                if (MchConfigList == null)
                    MchConfigList = new List<MchConfig>();

                var mchDataFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/MchWeChatData.json");
                MchWeChatDataList = LoadFileData<List<MchWeChatData>>(mchDataFile);
                if (MchWeChatDataList == null)
                    MchWeChatDataList = new List<MchWeChatData>();
            }
            catch (Exception ex)
            {
                LogHelper.Error("初始化数据失败", ex);
            }
        }

        /// <summary>
        /// 加载文件数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        static T LoadFileData<T>(string fileName)
        {
            if (!File.Exists(fileName))
                return default(T);
            try
            {
                string json = File.ReadAllText(fileName);
                if (string.IsNullOrEmpty(json))
                    return default(T);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("加载{0}失败", fileName), ex);
            }
            return default(T);
        }

        /// <summary>
        /// 保存Mch数据
        /// </summary>
        static void SaveMchData()
        {
            var mchDataFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/MchWeChatData.json");
            try
            {
                var json = JsonConvert.SerializeObject(MchWeChatDataList, Formatting.Indented);
                File.WriteAllText(mchDataFile, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("保存{0}失败", mchDataFile), ex);
            }
        }

        /// <summary>
        /// 是否是微信浏览器访问
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static bool IsWeChatBrowser(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return false;
            return userAgent.Contains("micromessenger");
        }

        /// <summary>
        /// 根据商户号获取WeChat实例
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static WeChatHelper GetInstance(string companyCode)
        {
            var mchConfig = MchConfigList.FirstOrDefault(mch => mch.CompanyCode == companyCode);
            if (mchConfig == null)
            {
                throw new Exception("没有找到对应的配置，请检查配置");
            }
            return new WeChatHelper(mchConfig);
        }
        #endregion

        #region methods
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        string GetTimeStamp()
        {
            return ((long)(DateTime.UtcNow - Jan1st1970).TotalSeconds).ToString();
        }

        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <returns></returns>
        string GetNonceStr()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 使用SHA1 加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        string EncryptToSHA1(string str)
        {
            byte[] strRes = Encoding.UTF8.GetBytes(str);
            var sha1Provider = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            strRes = sha1Provider.ComputeHash(strRes);
            var EnText = new StringBuilder();
            foreach (byte iByte in strRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <returns></returns>
        public string GetAccessToken()
        {
            var wechatData = MchWeChatDataList.FirstOrDefault(mch => mch.CompanyCode == CurrentMchConfig.CompanyCode);
            if (wechatData == null)
            {
                wechatData = new MchWeChatData();
                wechatData.CompanyCode = CurrentMchConfig.CompanyCode;
                MchWeChatDataList.Add(wechatData);
            }
            WeChatAccessToken accessToken;
            var now = DateTime.Now;
            if (wechatData.AccessToken == null || wechatData.AccessToken.ExpireDate <= now)
            {
                accessToken = GetNewestAccessToken();
                if (accessToken == null)
                    return string.Empty;
                wechatData.AccessToken = accessToken;
                SaveMchData();
            }
            else
            {
                accessToken = wechatData.AccessToken;
                if (accessToken.ExpireDate < now.AddMinutes(10))//如果距离过期时间小于10分钟，就刷新AccessToken
                {
                    var refreshAction = new Action<string>((companyCode) =>
                    {
                        var mahData = MchWeChatDataList.FirstOrDefault(mch => mch.CompanyCode == companyCode);
                        mahData.AccessToken = GetNewestAccessToken();
                        SaveMchData();
                    });
                    refreshAction.BeginInvoke(CurrentMchConfig.CompanyCode, null, null);
                }
            }
            return accessToken.access_token;
        }

        /// <summary>
        /// 获取最新的 AccessToken
        /// </summary>
        /// <returns></returns>
        WeChatAccessToken GetNewestAccessToken()
        {
            //https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1421140183&token=&lang=zh_CN
            var url = string.Format(URL_GetAccessToken, CurrentMchConfig.AppID, CurrentMchConfig.AppSecret);
            try
            {
                var client = new HttpClient();
                var tokenStr = client.GetStringAsync(url).Result;
                //{"access_token":"ACCESS_TOKEN","expires_in":7200}
                LogHelper.Debug(string.Format("Call WechatHelper.GetAccessTokenData. tokenStr[{0}]", tokenStr));
                var accessToken = JsonConvert.DeserializeObject<WeChatAccessToken>(tokenStr);
                if (accessToken == null)
                    throw new Exception("获取 AccessToken 失败。");
                accessToken.ExpireDate = DateTime.Now.AddSeconds(accessToken.expires_in);
                return accessToken;
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetAccessToken 出现错误.", ex);
            }
            return null;
        }

        /// <summary>
        /// 获取 JsApiTicket
        /// </summary>
        /// <returns></returns>
        public string GetJsApiTicket()
        {
            var wechatData = MchWeChatDataList.FirstOrDefault(mch => mch.CompanyCode == CurrentMchConfig.CompanyCode);
            if (wechatData == null)
            {
                wechatData = new MchWeChatData();
                wechatData.CompanyCode = CurrentMchConfig.CompanyCode;
                MchWeChatDataList.Add(wechatData);
            }
            WeChatApiTicket jsApiTicket;
            var now = DateTime.Now;
            if (wechatData.JsApiTicket == null || wechatData.JsApiTicket.ExpireDate <= now)
            {
                jsApiTicket = GetNewestJsApiTicket();
                if (jsApiTicket == null)
                    return string.Empty;
                wechatData.JsApiTicket = jsApiTicket;
                SaveMchData();
            }
            else
            {
                jsApiTicket = wechatData.JsApiTicket;
                if (jsApiTicket.ExpireDate < now.AddMinutes(10))//如果距离过期时间小于10分钟，就刷新AccessToken
                {
                    var refreshAction = new Action<string>((companyCode) =>
                    {
                        var mahData = MchWeChatDataList.FirstOrDefault(mch => mch.CompanyCode == companyCode);
                        mahData.JsApiTicket = GetNewestJsApiTicket();
                        SaveMchData();
                    });
                    refreshAction.BeginInvoke(CurrentMchConfig.CompanyCode, null, null);
                }
            }
            return jsApiTicket.ticket;
        }

        /// <summary>
        /// 获取最新的 JsApiTicket
        /// </summary>
        /// <returns></returns>
        public WeChatApiTicket GetNewestJsApiTicket()
        {
            //https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1421141115&token=&lang=zh_CN
            var accessToken = GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
                return null;
            var url = string.Format(URL_GetJsApiTicket, accessToken);
            try
            {
                var client = new HttpClient();
                var jsApiTicketStr = client.GetStringAsync(url).Result;
                //{"errcode":"0","errmsg":"ok","ticket":"bxLdikRXVbTPdHSM05e5u5sUoXNKd8-41ZO3MhKoyN5OfkWITDGgnr2fwJ0m9E8NYzWKVZvdVtaUgWvsdshFKA","expires_in":"7200"}
                LogHelper.Debug(string.Format("Call WechatHelper.GetJsApiTicket. jsApiTicketStr[{0}]", jsApiTicketStr));
                var jsApiTicket = JsonConvert.DeserializeObject<WeChatApiTicket>(jsApiTicketStr);
                if (jsApiTicket == null)
                    throw new Exception("获取 JsApiTicket 失败。");
                jsApiTicket.ExpireDate = DateTime.Now.AddSeconds(jsApiTicket.expires_in);
                return jsApiTicket;
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetJsApiTicket 出现错误.", ex);
            }
            return null;
        }

        /// <summary>
        /// 获取基础授权Url
        /// </summary>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        public string GetBaseAuthorizeUrl(string redirectUrl)
        {
            var redirect_uri = HttpUtility.UrlEncode(redirectUrl);
            return string.Format(URL_GetAuthorize, CurrentMchConfig.AppID, redirect_uri, "snsapi_base");
        }

        /// <summary>
        /// 获取完整授权Url
        /// </summary>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        public string GetAuthorizeUrl(string redirectUrl)
        {
            var redirect_uri = HttpUtility.UrlEncode(redirectUrl);
            return string.Format(URL_GetAuthorize, CurrentMchConfig.AppID, redirect_uri, "snsapi_userinfo");
        }

        /// <summary>
        /// 根据AccessCode获取OpenID
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetOpenIDByAccessCode(string code)
        {
            LogHelper.Debug(string.Format("Call WechatHelper.GetOpenIDByAccessCode. authCode[{0}]", code));
            var accessToken = GetAuthAccessToken(code);
            return accessToken == null ? string.Empty : accessToken.openid;
        }

        /// <summary>
        /// 获取网页授权AccessToken
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public WeChatUserAuthAccessToken GetAuthAccessToken(string code)
        {
            var url = string.Format(URL_GetAuthAccessToken, CurrentMchConfig.AppID, CurrentMchConfig.AppSecret, code);
            try
            {
                var client = new HttpClient();
                var tokenStr = client.GetStringAsync(url).Result;
                //{"access_token":"OezXcEiiBSKSxW0eoylIeF7l1zoDrdnqMvNA2c0fxefljQI09aKhb5lzxlMUfdLqVmr8n6EI8SkAdXE1tYsxnx-e5VRQGj35rB7GvLsKUN05wUt6EDm2aJlY1Xoq2cdWolh6ArnMcAp8ZKtcZrzsUA","expires_in":7200,"refresh_token":"OezXcEiiBSKSxW0eoylIeF7l1zoDrdnqMvNA2c0fxefljQI09aKhb5lzxlMUfdLqa2kKyuAC9eyflwHK-XDhdMVaeB4oQJ2rj9ByltrfJLjepP-BWlHB8Wtdm7aEs69EIlg4zswhaWqn-A-weQif6A","openid":"ozhR3jpxd8d-0TxWMM_uubQYMLoo","scope":"snsapi_base"}
                LogHelper.Debug(string.Format("Call WechatHelper.GetAuthAccessToken. tokenStr[{0}]", tokenStr));
                var accessToken = JsonConvert.DeserializeObject<WeChatUserAuthAccessToken>(tokenStr);
                if (accessToken == null)
                    return null;
                return accessToken;
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetAuthAccessToken 出现错误.", ex);
            }
            return null;
        }

        /// <summary>
        /// 根据AccessCode获取OpenID，需要用户关注公众号
        /// </summary>
        /// <param name="openID"></param>
        /// <returns></returns>
        public WeChatUserInfo GetUserBasicInfo(string openID)
        {
            //http://mp.weixin.qq.com/wiki/14/bb5031008f1494a59c6f71fa0f319c66.html
            var accessToken = GetAccessToken();
            var url = string.Format(URL_GetUserBasicInfo, accessToken, openID);
            try
            {
                var client = new HttpClient();
                var userInfoResponse = client.GetStringAsync(url).Result;
                //{
                //    "subscribe": 1, 
                //    "openid": "odEDBjmoJKiQCZx3tM3GPDeBmLUI", 
                //    "nickname": "刘凯", 
                //    "sex": 1, 
                //    "language": "zh_CN", 
                //    "city": "厦门", 
                //    "province": "福建", 
                //    "country": "中国", 
                //    "headimgurl": "http://wx.qlogo.cn/mmopen/80h2Yx4qUfqwSgKw9THfeYxCvicVfwQyCdOxiawicibR3mg6o9lZeA4ibGrMPpf3UjZz0icgibeTzQZOG8VZvv8icC43PA/0", 
                //    "subscribe_time": 1465958412, 
                //    "remark": "", 
                //    "groupid": 0, 
                //    "tagid_list": [ ]
                //}
                LogHelper.Debug(string.Format("Call WechatHelper.GetUserBasicInfo. userInfoResponse[{0}]", userInfoResponse));
                var result = JsonConvert.DeserializeObject<WeChatUserInfo>(userInfoResponse);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetUserBasicInfo 出现错误.", ex);
            }
            return null;
        }

        /// <summary>
        /// 用户信息(需scope为snsapi_userinfo)
        /// </summary>
        /// <param name="openID"></param>
        /// <returns></returns>
        public WeChatUserSnsInfo GetUserSnsInfo(string accessToken, string openID)
        {
            //https://mp.weixin.qq.com/wiki?id=mp1421140842
            var url = string.Format(URL_GetUserSnsInfo, accessToken, openID);
            try
            {
                var client = new HttpClient();
                var userInfoResponse = client.GetStringAsync(url).Result;
                //{
                //    "openid": "odEDBjmoJKiQCZx3tM3GPDeBmLUI", 
                //    "nickname": "刘凯", 
                //    "sex": 1, 
                //    "city": "厦门", 
                //    "province": "福建", 
                //    "country": "中国", 
                //    "headimgurl": "http://wx.qlogo.cn/mmopen/80h2Yx4qUfqwSgKw9THfeYxCvicVfwQyCdOxiawicibR3mg6o9lZeA4ibGrMPpf3UjZz0icgibeTzQZOG8VZvv8icC43PA/0", 
                //    "privilege": [ ],
                //    "unionid": 0
                //}
                LogHelper.Debug(string.Format("Call WechatHelper.GetUserSnsInfo. userInfoResponse[{0}]", userInfoResponse));
                var result = JsonConvert.DeserializeObject<WeChatUserSnsInfo>(userInfoResponse);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetUserSnsInfo 出现错误.", ex);
            }
            return null;
        }

        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <param name="message"></param>
        public void SendTemplateMessage(TemplateMessageModel message)
        {
            //https://mp.weixin.qq.com/advanced/tmplmsg?action=faq&token=1376195260&lang=zh_CN
            var accessToken = GetAccessToken();
            var url = string.Format(URL_SendTemplate, accessToken);
            try
            {
                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(message);
                LogHelper.Debug($"Call WechatHelper.SendTemplateMessage. request[{json}]");
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync(url, content).Result;
                var responseData = response.Content.ReadAsStringAsync().Result;
                //{"errcode":0,"errmsg":"ok","msgid":200228332}
                LogHelper.Debug(string.Format("Call WechatHelper.SendTemplateMessage. result[{0}]", responseData));
                var sendResult = JsonConvert.DeserializeObject<dynamic>(responseData);
                if (sendResult == null)
                    throw new Exception("服务器返回了错误的结果");
                if (sendResult.errcode != 0)
                {
                    throw new Exception("失败, errorcode: " + sendResult.errcode);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SendTemplateMessage 出现错误.", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取JS API 签名
        /// </summary>
        /// <param name="url"></param>
        /// <returns>item1: nonceStr, item2: timeStamp, item3: signature</returns>
        public JsApiSignatureModel JsApiSignature(string url)
        {
            //http://mp.weixin.qq.com/wiki/11/74ad127cc054f6b80759c40f77ec03db.html#.E9.99.84.E5.BD.951-JS-SDK.E4.BD.BF.E7.94.A8.E6.9D.83.E9.99.90.E7.AD.BE.E5.90.8D.E7.AE.97.E6.B3.95
            LogHelper.Debug($"开始调用JsApiSignature方法，url[{url}]");
            var jsApiTicket = GetJsApiTicket();
            var nonceStr = Guid.NewGuid().ToString();
            var timeStamp = GetTimeStamp();
            var str = $"jsapi_ticket={jsApiTicket}&noncestr={nonceStr}&timestamp={timeStamp}&url={url}";
            var sha1Str = EncryptToSHA1(str);
            var jsApiSignature = new JsApiSignatureModel(CurrentMchConfig.AppID, nonceStr, timeStamp, sha1Str);
            LogHelper.Debug($"调用JsApiSignature生成数据[{jsApiSignature.ToString()}]");
            return jsApiSignature;
        }
        #endregion
    }
}