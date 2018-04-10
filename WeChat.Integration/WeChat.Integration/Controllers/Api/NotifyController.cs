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
    /// 微信通知类API
    /// </summary>
    [RoutePrefix("Notify")]
    public class NotifyController : BaseApiController
    {
        /// <summary>
        /// 发送模板类消息
        /// </summary>
        [HttpPost, Route("SendTemplateMessage")]
        public JsonActionResult<bool> SendTemplateMessage(string companyCode, TemplateMessageModel message)
        {
            return SafeExecute(() =>
            {
                if (!ModelState.IsValid)//表示没有过滤参数成功匹配，判定为错误请求。
                {
                    throw new Exception("参数错误。");
                }
                var weChatHelper = WeChatHelper.GetInstance(companyCode);
                weChatHelper.SendTemplateMessage(message);
                return true;
            });
        }
    }
}
