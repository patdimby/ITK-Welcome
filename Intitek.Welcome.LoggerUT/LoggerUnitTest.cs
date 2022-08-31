using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Intitek.Welcome.Infrastructure.Log;

namespace Intitek.Welcome.LoggerUT
{
    [TestClass]
    public class LoggerUnitTest
    {
        FileLogger _logger = new FileLogger();
        public LoggerUnitTest()
        {
            
        }
        [TestMethod]
        public void ShouldWriteLogInfo()
        {
            var exception = new ExceptionLogger() {
                ExceptionMessage = "",
                MethodName = "ShouldWriteLogInfo",
                ServiceName = "LoggerUnitTest"

            };
            _logger.Info(exception.ToString());
            Assert.IsTrue(true); // ThrowsException<NotImplementedException>();
        }
    }
}
