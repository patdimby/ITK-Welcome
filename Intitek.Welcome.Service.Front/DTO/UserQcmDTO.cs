using Intitek.Welcome.Domain;
using System;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Front
{
    public class UserQcmDTO
    {
        public int ID { get; set; }
        public int QcmID { get; set; }
        public bool QcmHasBadge { get; set; }
        public string QcmName { get; set; }
        public int UserID { get; set; }
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }

        public int NbQuestions { get; set; }
        public string DocumentVersion { get; set; }

        public int QcmScoreMinimal { get; set; }
        public int QcmScore { get; set; }
        public bool IsPassed { get; set; }
        public DateTime DateCre { get; set; }
        public DateTime DateFin { get; set; }
        public List<UserQcmReponse> Reponses { get; set; }
    }
}
