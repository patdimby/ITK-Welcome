using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Infrastructure.Histo
{
    public class EmailBO
    {
        public int ID { get; set; }
        public Nullable<int> Id_IntitekUser { get; set; }
        public Nullable<int> Id_MailTemplate { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Message { get; set; }
        public Nullable<int> ID_Batch { get; set; }
    }
}
