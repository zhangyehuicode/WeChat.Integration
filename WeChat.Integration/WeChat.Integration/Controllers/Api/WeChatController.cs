using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeChat.Integration.Helpers;
using WeChat.Integration.Models;

namespace WeChat.Integration.Controllers.Api
{
    /// <summary>
    /// 微信api
    /// </summary>
    [RoutePrefix("api/WeChat")]
    public class WeChatController : BaseApiController
    {
        /// <summary>
        /// 获取 AccessToken
        /// </summary>
        /// <param name="url">调用JSApi的完整URL</param>
        /// <returns></returns>
        [HttpGet, Route("AccessToken")]
        public JsonActionResult<string> AccessToken(string companyCode)
        {
            return SafeExecute(() =>
            {
                var weChatHelper = WeChatHelper.GetInstance(companyCode);
                return weChatHelper.GetAccessToken();
            });
        }

        /// <summary>
        /// 获取JsApi 签名
        /// </summary>
        /// <param name="url">调用JSApi的完整URL</param>
        /// <returns></returns>
        [HttpPost, Route("JsApiSignature")]
        public JsonActionResult<JsApiSignatureModel> JsApiSignature(string companyCode, string url)
        {
            return SafeExecute(() =>
            {
                var weChatHelper = WeChatHelper.GetInstance(companyCode);
                return weChatHelper.JsApiSignature(url);
            });
        }

        /// <summary>
        /// 获取 用户基本信息
        /// </summary>
        /// <param name="openID">openID</param>
        /// <returns></returns>
        [HttpGet, Route("UserBasicInfo")]
        public JsonActionResult<WeChatUserInfo> GetUserBasicInfo(string companyCode, string openID)
        {
            return SafeExecute(() =>
            {
                var weChatHelper = WeChatHelper.GetInstance(companyCode);
                return weChatHelper.GetUserBasicInfo(openID);
            });
        }
    }
}
