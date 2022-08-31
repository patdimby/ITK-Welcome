using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Infrastructure.Log
{
    public sealed class FileLogger: ILogger
    {
        private readonly ILog _log;
      
        public FileLogger()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var configFileDirectory = Path.Combine(baseDirectory, "log4net.config");

            FileInfo configFileInfo = new FileInfo(configFileDirectory);
            log4net.Config.XmlConfigurator.ConfigureAndWatch(configFileInfo);

            _log = log4net.LogManager.GetLogger("log4netFileLogger");
        }

        public void Error(ExceptionLogger exceptionLogger, Exception ex)
        {
            _log.Error(exceptionLogger, ex);
        }
        public void Error(string message)
        {
            _log.Error(message);
        }
        public void Info(string message)
        {
            _log.Info(message);
        }
    }
}
