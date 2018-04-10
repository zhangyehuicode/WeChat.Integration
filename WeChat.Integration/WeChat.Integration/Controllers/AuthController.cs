using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeChat.Integration.Helpers;
using WeChat.Integration.Models;
    
namespace WeChat.Integration.Controllers
{
    /// <summary>
    /// 调用微信授权接口获取授权信息
    /// </summary>
    public class AuthController : Controller
    {
        // GET: Auth
        public ActionResult Index()
        {
            LogHelper.Debug(string.Format("Call Auth/Index. RawUrl:[{0}]", Request.RawUrl));
            var companyCode = Request["CompanyCode"];
            var redirectUrl = Request["RedirectUrl"];
            var scope = Request["scope"];
            if (string.IsNullOrEmpty(companyCode))
            {
                ViewBag.Message = "参数错误";
                return View();
            }
            if (string.IsNullOrEmpty(redirectUrl))
            {
                ViewBag.Message = "缺少 RedirectUrl 参数";
                return View();
            }
            var needUserInfo = "true".Equals(Request["UserInfo"], StringComparison.OrdinalIgnoreCase);
            var redirectKey = Guid.NewGuid().ToString().Replace("-", string.Empty).ToLower();
            var authRedirectData = new AuthRedirectModel
            {
                CompanyCode = companyCode,
                RedirectUrl = redirectUrl,
                NeedUserInfo = needUserInfo
            };
            var weChatAuthUrl = string.Empty;
            var callbackUrl = string.Empty;
            var weChatHelper = WeChatHelper.GetInstance(companyCode);
            if ("snsapi_userinfo".Equals(scope))//Scope为snsapi_userinfo
            {
                authRedirectData.IsUserInfoScope = true;
                authRedirectData.NeedUserInfo = true;
                callbackUrl = string.Concat(AppSettings.SiteDomain, "/Auth/WxCallback?rid=", redirectKey);
                weChatAuthUrl = weChatHelper.GetAuthorizeUrl(callbackUrl);
            }
            else//Scope为snsapi_base
            {
                callbackUrl = string.Concat(AppSettings.SiteDomain, "/Auth/WxCallback?rid=", redirectKey);
                weChatAuthUrl = weChatHelper.GetBaseAuthorizeUrl(callbackUrl);
            }
            AppContext.AuthRedirectDictionary[redirectKey] = authRedirectData;
            return Redirect(weChatAuthUrl);
        }

        // GET: Auth/WxCallback
        public ActionResult WxCallback()
        {
            LogHelper.Debug(string.Format("Call Auth/WxCallback. RawUrl:[{0}]", Request.RawUrl));
            var code = Request["code"];
            if (string.IsNullOrEmpty(code))
            {
                ViewBag.Message = "没有返回code参数";
                return View();
            }
            var redirectKey = Request["rid"];
            if (redirectKey != null)
                redirectKey = redirectKey.ToLower();
            if (!AppContext.AuthRedirectDictionary.ContainsKey(redirectKey))
            {
                ViewBag.Message = "回调参数错误";
                return View();
            }
            var authRedirect = AppContext.AuthRedirectDictionary[redirectKey];
            var weChatHelper = WeChatHelper.GetInstance(authRedirect.CompanyCode);
            var accessToken = weChatHelper.GetAuthAccessToken(code);
            var paramString = string.Empty;
            if (authRedirect.NeedUserInfo && accessToken != null)
            {
                WeChatUserSnsInfo userInfo = null;
                if (authRedirect.IsUserInfoScope)
                {
                    userInfo = weChatHelper.GetUserSnsInfo(accessToken.access_token, accessToken.openid);
                }
                else
                {
                    userInfo = weChatHelper.GetUserBasicInfo(accessToken.openid);
                }
                if (userInfo != null)
                {
                    paramString = userInfo.GetBasicInfoString();
                }
            }
            else
            {
                var openId = accessToken == null ? string.Empty : accessToken.openid;
                paramString = "openid=" + openId;
            }
            var redirectUrl = HttpUtility.UrlDecode(authRedirect.RedirectUrl);
            if (redirectUrl.Contains("?"))
                redirectUrl = string.Concat(redirectUrl, "&", paramString);
            else
                redirectUrl = string.Concat(redirectUrl, "?", paramString);
            LogHelper.Debug(string.Format("Call Auth/WxCallback. RedirectUrl:[{0}]", redirectUrl));
            return Redirect(redirectUrl);
        }
    }
}