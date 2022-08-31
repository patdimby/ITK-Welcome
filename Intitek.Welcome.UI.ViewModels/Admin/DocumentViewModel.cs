using Intitek.Welcome.Domain;
using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public class DocumentViewModel
    {
        public int ID { get; set; }
        public int IdLang { get; set; }
        public string DocumentTitle { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_Name")]
        [Required]
        public string Name { get; set; }
        public bool IsDefaultLangName { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "doc_Version")]
        public string Version { get; set; }

        public DateTime? Date { get; set; }

        public int? Approbation { get; set; }

        public int? Test { get; set; }

        public string Commentaire { get; set; }

        public string ContentType { get; set; }

        public string NomOrigineFichier { get; set; }

        public string Extension { get; set; }

        public byte[] Data { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_Inactif")]
        public bool Inactif { get; set; }

        public string CategorySubCategory { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "doc_Categorie")]
        public Nullable<int> ID_Category { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "doc_SubCategories")]
        public Nullable<int> ID_SubCategory { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_Informatif")]
        public bool IsNoActionRequired { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "assignmentType")]
        public string TypeAffectation { get; set; }
        public int? IdQcm { get; set; }
        
        public bool IsMajor { get; set; }

        public int IdUser { get; set; }
        public string NameCategory { get; set; }
        public string NameSubCategory { get; set; }
        public string DefaultNameCategory { get; set; }
        public string DefaultNameSubCategory { get; set; }
        public int? OrdreCategory { get; set; }
        public int? OrdreSubCategory { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "typeMetier")]
        public bool IsMetier { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "typeStructure")]
        public bool IsStructure { get; set; }

        public bool IsChecked { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "modeMagazine")]
        public bool IsMagazine { get; set; }

        public bool IsSessionChecked { get; set; }
        public bool IsDisabled { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_Lu")]
        public Nullable<System.DateTime> IsRead { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_Approbation")]
        public Nullable<System.DateTime> IsApproved { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_Test")]
        public Nullable<System.DateTime> IsTested { get; set; }

        public List<DocumentCategoryDTO> Categories { get; set; }
        public List<CategorySubcategoryViewModel> CategorySubcategories { get; set; }
        public List<DocumentVersionDTO> Versions { get; set; }

        public List<ProfilDTO> Profiles { get; set; }
        //public int SelectedIdProfiles { get; set; }
        public List<EntityAgencyDTO> Entites { get; set; }
        //public string SelectedEntites { get; set; }
        public string Affectation { get; set; }
        public List<QcmDTO> Qcms { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_Reading")]
        public int UserRead { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "approval")]
        public int UserApproved { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_Test")]
        public int UserTested { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "user_agency")]
        public string AgencyName { get; set; }

        public bool ReadBrowser { get; set; }
        public bool ReadDownload { get; set; }
        public bool PhaseOnboarding { get; set; }
        public bool PhaseEmployee { get; set; }

        public string CreatedBy { get; set; }
        public string CreationDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModificationDate { get; set; }
        public string DeletedBy { get; set; }
        public string DeletionDate { get; set; }
        [IgnoreDataMember]
        public string Actions { get; set; }

        public List<Lang> Langues { get; set; }
        public Statistiques Statistiques { get; set; }
    }
}
