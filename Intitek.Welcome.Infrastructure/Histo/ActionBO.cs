using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Infrastructure.Histo
{
    public class ActionBO
    {
        public int ID_Object { get; set; }
        public int ID_User { get; set; }
        public string ObjectCode { get; set; }
        public string Action { get; set; }
        public DateTime DateAction { get; set; }
        public string LinkObjects { get; set; }
    }

}
