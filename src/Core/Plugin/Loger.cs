using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace X.Core.Plugin
{
    public class Loger
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        public static void Warn(Type t, Exception ex)
        {
            LogManager
                .GetLogger("Warn")
                .Warn(ex.Message, ex);
        }
        /// <summary>
        /// 一般错误
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        public static void Error(Exception ex)
        {
            LogManager
                .GetLogger("Error")
                .Error(ex.Message, ex);
        }
        /// <summary>
        /// 一般错误
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        public static void Error(string msg)
        {
            LogManager
                .GetLogger("Error")
                .Error(msg);
        }
        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        public static void Debug(Exception ex)
        {
            LogManager
                .GetLogger("Debug")
                .Debug(ex.Message, ex);
        }
        /// <summary>
        /// 致命错误
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        public static void Fatal(Type t, Exception ex)
        {
            LogManager
                .GetLogger(t)
                .Fatal("致命错误", ex);
        }
        /// <summary>
        /// 提示
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        public static void Info(string msg)
        {
            LogManager
                .GetLogger("info")
                .Info(msg);
        }
    }
}



