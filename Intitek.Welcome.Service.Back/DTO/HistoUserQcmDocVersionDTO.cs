using System;

namespace Intitek.Welcome.Service.Back
{
    public class HistoUserQcmDocVersionDTO
    {
        public long ID { get; set; }
        public int ID_Document { get; set; }
        public int ID_Qcm { get; set; }
        public DateTime DateCreation { get; set; }
        public string Username { get; set; }
        public string DocName { get; set; }
        public string QcmName { get; set; }
        public string Version { get; set; }
        public int Score { get; set; }
        public int ScoreMinimal { get; set; }
        public DateTime DateAction { get; set; }
    }
}
