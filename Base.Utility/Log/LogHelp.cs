using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;

namespace Base.Utility
{
    public class LogHelp : ILogHelp
    {
        private readonly log4net.ILog _logger;

        public LogHelp()
            : this("DefaultLogger")
        {
        }

        public LogHelp(string loggerName)
            : this(loggerName, string.Empty)
        {
        }

        public LogHelp(string loggerName, string configPath)
        {
            if (configPath.IsNullOrEmpty())
            {
                log4net.Config.XmlConfigurator.Configure();
            }
            else
            {
                log4net.Config.XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configPath)));
            }

            _logger = LogManager.GetLogger(loggerName);
        }

        #region ILog 成员

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Warning(string message)
        {
            _logger.Warn(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Exception(Exception exception)
        {
            _logger.Error("出现异常:", exception);
        }

        #endregion
    }
}
