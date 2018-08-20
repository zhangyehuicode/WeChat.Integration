using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace WeChat.Integration.Helpers
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public static class LogHelper
    {
        private static readonly log4net.ILog s_logger = log4net.LogManager.GetLogger("Application");

        public static void Register()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public static void Debug(object message)
        {
            s_logger.Debug(message);
        }

        public static void Info(object message)
        {
            s_logger.Info(message);
        }

        public static void Warn(object message)
        {
            s_logger.Warn(message);
        }

        public static void Error(object message)
        {
            s_logger.Error(message);
        }

        public static void Error(object message, Exception exception)
        {
            s_logger.Error(message, exception);
        }
    }
}