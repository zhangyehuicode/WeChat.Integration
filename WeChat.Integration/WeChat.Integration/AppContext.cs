using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeChat.Integration.Models;

namespace WeChat.Integration
{
    /// <summary>
    /// AppContext
    /// </summary>
    public static class AppContext
    {
        static readonly Dictionary<string, AuthRedirectModel> _authRedirectDictionary = new Dictionary<string, AuthRedirectModel>();

        /// <summary>
        /// 授权回调管理字典
        /// </summary>
        public static Dictionary<string, AuthRedirectModel> AuthRedirectDictionary
        {
            get { return _authRedirectDictionary; }
        }
    }
}