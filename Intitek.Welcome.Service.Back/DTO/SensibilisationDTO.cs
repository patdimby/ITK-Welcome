using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Service.Back
{
    public class SensibilisationDTO
    {
        public string Mois { get; set; }
        public string Entité { get; set; }
        public string Agence { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Actif { get; set; }
        public int Present { get; set; }
        public string Nom_Document { get; set; }
        public string Version { get; set; }
        public string Resultat { get; set; }
        public int ID_User { get; set; }
        public int ID_Document { get; set; }
        public Nullable<System.DateTime> Test_Terminé { get; set; }
        public Nullable<int> Score { get; set; }
        public Nullable<int> ScoreMinimal { get; set; }
    }
}
