using Intitek.Welcome.Domain;
using Intitek.Welcome.UI.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Intitek.Welcome.UI.ViewModels
{
    public class DocumentViewModel
    {
        public int ID { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_Version")]
        public string Version { get; set; }

        public DateTime? Date { get; set; }

        public int? Approbation { get; set; }

        public int? Test { get; set; }

        public string Commentaire { get; set; }

        public string Couleur { get; set; }
        public bool IsReadOnly { get; set; }

        public string ContentType { get; set; }

        public string Extension { get; set; }

        public byte[] Data { get; set; }

        public bool Inactif { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_Categorie")]
        public Nullable<int> ID_Category { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_SubCategories")]
        public Nullable<int> ID_SubCategory { get; set; }

        public string NameCategory { get; set; }
        public string NameSubCategory { get; set; }

        public int? OrdreCategory { get; set; }
        public int? OrdreSubCategory { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_Informatif")]
        public bool IsNoActionRequired { get; set; }

        public string TypeAffectation { get; set; }

        public string NomOrigineFichier { get; set; }

        public int? IdQcm { get; set; }

        public Nullable<System.DateTime> IsRead { get; set; }
        public Nullable<System.DateTime> IsApproved { get; set; }
        public Nullable<System.DateTime> IsTested { get; set; }

        public int Num { get; set; }
        public int IdUser { get; set; }

        public bool IsBoolRead { get; set; }
        public bool IsBoolApproved { get; set; }

        public bool IsBoolTested { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "doc_Link")]
        public string Link { get; set; }
        /*[Display(ResourceType = typeof(Resource), Name = "doc_Sharable_Link")]
        public string SharableLink { get; set; }*/

        [Display(ResourceType = typeof(Resource), Name = "doc_Lu")]
        public Nullable<int> FiltreRead { get; set; }
        public bool IsMagazine { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_Approbation")]
        public Nullable<int> FiltreApproved { get; set; }
        public bool ReadBrowser { get; set; }

        public bool ReadDownload { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "doc_Test")]
        public Nullable<int> FiltreTested { get; set; }
        public UserQcm UserQcm { get; set; }

        [IgnoreDataMember]
        public string InfoBulles {
            get {
                if (this.UserQcm != null)
                {
                    string ret = "";
                    if (this.UserQcm.DateFin.HasValue)
                    {
                        ret= string.Format(Resource.qcm_Infobulle_DateQuizz, this.UserQcm.DateFin)+ "<br/>";
                    }                   
                    ret += string.Format(Resource.qcm_Infobulle_score_obtenu, UserQcm.Score, UserQcm.NbQuestions);
                    ret+= "<br/>" + string.Format(Resource.qcm_Infobulle_score_minimum, UserQcm.ScoreMinimal, UserQcm.NbQuestions);
                    return ret;
                }
                return null;               
            }
        }
        [IgnoreDataMember]
        public int Mark
        {
            get
            {
                if (this.UserQcm != null)
                {
                    if (this.UserQcm.Score >= this.UserQcm.ScoreMinimal)
                        return 1;
                    else
                        return 2;
                }
                return -1;
            }
        }
    }
}
