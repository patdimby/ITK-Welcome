using Intitek.Welcome.Domain;
using System;

namespace Intitek.Welcome.Service.Back
{
    public class DocumentDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsDefaultLangName { get; set; }
        public string Version { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Commentaire { get; set; }
        public string ContentType { get; set; }
        public Qcm Qcm { get; set; }
        public Nullable<int> Approbation { get; set; }
        public Nullable<int> Test { get; set; }
        public Nullable<int> ID_Category { get; set; }
        public Nullable<int> ID_SubCategory { get; set; }
        public string NameCategory { get; set; }
        public string DefaultNameCategory { get; set; }
        public string NameSubCategory { get; set; }
        public string DefaultNameSubCategory { get; set; }
        public int? OrdreCategory { get; set; }
        public int? OrdreSubCategory { get; set; }
        public string TypeAffectation { get; set; }
        public bool? Inactif { get; set; }
        public bool IsNoActionRequired { get; set; }
        public string NomOrigineFichier { get; set; }
        public string Extension { get; set; }
        public bool Affecte { get; set; }
        public bool IsMetier { get; set; }
        public bool IsStructure { get; set; }
        public bool IsMetierCritere { get; set; }
        public bool IsStructureCritere { get; set; }
        public bool IsChecked { get; set; }
        public bool IsSessionChecked { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsMagazine { get; set; }
        public string Color { get; set; }

        public int Num { get; set; }
        public Nullable<System.DateTime> IsRead { get; set; }
        public Nullable<System.DateTime> IsApproved { get; set; }
        public Nullable<System.DateTime> IsTested { get; set; }
        public Nullable<System.DateTime> DateCre { get; set; }
        public Nullable<System.DateTime> DateUpd { get; set; }
        public bool ReadBrowser { get; set; }
        public bool ReadDownload { get; set; }
        public bool PhaseOnboarding { get; set; }
        public bool PhaseEmployee { get; set; }

        public int UserRead { get; set; }
        public int UserApproved { get; set; }
        public int UserTested { get; set; }
        
        public string EntityName { get; set; }
        public string AgencyName { get; set; }
        public DocumentCategoryLang DocumentCategoryLang { get; set; }
        public Statistiques Statistiques { get; set; }

    }
}
