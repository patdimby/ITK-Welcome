using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Service.Front
{
    public class HistoUserQcmDocVersionDTO
    {
        public long ID { get; set; }
        public int UserID { get; set; }
        public int QcmID { get; set; }
        public int DocumentID { get; set; }
        public string DocumentVersion { get; set; }
        public int QcmScore { get; set; }
        public int QcmScoreMinimal { get; set; }
        public DateTime DateCre { get; set; }
        public DateTime DateFin { get; set; }

        public string DocumentName { get; set; }
        public string QcmName { get; set; }
    }
}
