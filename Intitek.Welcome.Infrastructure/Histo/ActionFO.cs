using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Infrastructure.Histo
{
    public class ActionFO
    {
        public long ID { get; set; }
        public int ID_User { get; set; }
        public int ID_Qcm { get; set; }
        public int ID_Document { get; set; }
        public string Version { get; set; }
        public int Score { get; set; }
        public int ScoreMinimal { get; set; }
        public System.DateTime DateCre { get; set; }
        public System.DateTime DateFin { get; set; }
    }
}
