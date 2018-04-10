using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WeChat.Integration
{
    /// <summary>
    /// 应用设置
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// 站点域名
        /// </summary>
        public static string SiteDomain
        {
            get { return GetAppSettingValue("SiteDomain"); }
        }

        static string GetAppSettingValue(string key)
        {
            var settingValue = ConfigurationManager.AppSettings[key];
            return string.IsNullOrEmpty(settingValue) ? string.Empty : settingValue;
        }
    }
}