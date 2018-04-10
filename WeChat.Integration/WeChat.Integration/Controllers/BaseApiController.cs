using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeChat.Integration.Helpers;
using WeChat.Integration.Models;

namespace WeChat.Integration.Controllers
{
    /// <summary>
    /// ApiController 基类
    /// </summary>
    public abstract class BaseApiController : ApiController
    {
        /// <summary>
        /// 为 基础操作 提供统一的返回格式，包含try / catch操作。
        /// </summary>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="execAction"></param>
        /// <returns></returns>
        protected JsonActionResult<TResult> SafeExecute<TResult>(Func<TResult> execAction)
        {
            var result = new JsonActionResult<TResult>();
            try
            {
                result.Data = execAction();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                result.IsSuccessful = false;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
    }
}
