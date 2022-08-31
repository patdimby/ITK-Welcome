using Intitek.Welcome.Domain;
using System;

namespace Intitek.Welcome.Service.Front
{
    public class DocumentDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsReadOnly { get; set; }
        public string Version { get; set; }
        public string Extension { get; set; }
        public string NomOrigineFichier { get; set; }
        public Nullable<int> Approbation { get; set; }
        public Nullable<int> Test { get; set; }
        public Nullable<int> IdQcm { get; set; }
        public Nullable<int> ID_Category { get; set; }
        public Nullable<int> ID_SubCategory { get; set; }
        public bool IsNoActionRequired { get; set; }
        public Nullable<System.DateTime> IsRead { get; set; }
        public Nullable<System.DateTime> IsApproved { get; set; }
        public Nullable<System.DateTime> IsTested { get; set; }
        public string NameCategory { get; set; }
        public string NameSubCategory { get; set; }
        
        public int? OrdreCategory { get; set; }
        public int? OrdreSubCategory { get; set; }

        public int Num { get; set; }
        public int IdUser { get; set; }
        public bool IsBoolRead { get; set; }
        public bool IsBoolApproved { get; set; }
        public bool IsBoolTested { get; set; }
        public bool? Inactif { get; set; }
        public bool ReadBrowser { get; set; }
        public bool ReadDownload { get; set; }
        public UserQcm UserQcm { get; set; }
        public int? Score { get; set; }
        public int? ScoreMinimal { get; set; }
        public bool isMetier { get; set; }
        public bool isStructure { get; set; }
        public bool isMagazine { get; set; }
        public string Couleur { get; set; }

        //public Nullable<int> FiltreRead { get; set; }
        //public Nullable<int> FiltreApproved { get; set; }
        //public Nullable<int> FiltreTested { get; set; }

    }
}
