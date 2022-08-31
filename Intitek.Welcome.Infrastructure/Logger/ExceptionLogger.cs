using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Infrastructure.Log
{
    public class ExceptionLogger
    {
        public int ID { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrack { get; set; }
        public string ServiceName { get; set; }
        public string MethodName { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public DateTime ExceptionDateTime { get; set; }
    }
}
