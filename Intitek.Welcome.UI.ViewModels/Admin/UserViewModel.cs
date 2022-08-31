using Intitek.Welcome.Domain;
using Intitek.Welcome.UI.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.UI.ViewModels.Admin
{
    public enum Statuts
    {
        None = -1,
        BACKOFFICE = 0,
        FRONTOFFICE = 10
    };
    public class UserViewModel
    {
        public int ID { get; set; }
        public int? ID_Manager { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "user_login")]
        public string Name { get; set; }
        public int? Status { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "Entity")]
        public string EntityName { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "user_agency")]
        public string AgencyName { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "user_mail")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "user_mail")]
        public string EmailOnBoarding { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "user_isOnboarding")]
        public bool IsOnBoarding { get; set; }

        public bool IsRoot { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "profile")]
        public List<Profile> ProfilList { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "RemainingReading")]
        public int DocumentRead { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "RemainingApproval")]
        public int DocumentApproved { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "RemainingTest")]
        public int DocumentTested { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "profile")]
        public List<int> ProfilListFilter { get; set; }


        /// <summary>
        /// Ticket 398 - modifer Active par la Présent/Pas present en active directory
        /// </summary>
        [Display(ResourceType = typeof(Resource), Name = "Present")]
        public bool Active { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Actif")]
        public bool Activity { get; set; }


        [Display(ResourceType = typeof(Resource), Name = "fieldType")]
        public string Type { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Reason")]
        public string InactivityReason { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "inact_debut")]
        public DateTime? InactivityStart { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "inact_fin")]
        public DateTime? InactivityEnd { get; set; }
        public string Division { get; set; }
        public string Departement { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "DateExit")]
        public DateTime? ExitDate { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "DateEntry")]
        public DateTime? EntryDate { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "user_manager")]
        public IntitekUser Manager { get; set; }
        [Display(ResourceType = typeof(Resource), Name = "user_dop")]
        public ImportManager ImportManager { get; set; }

        public bool isReader { get; set; }
       
        //[IgnoreDataMember]
        //[Display(ResourceType = typeof(Resource), Name = "user_fullname")]        
        //public string FullName
        //{
        //    get
        //    {
        //        string ret = "";
        //        if (!string.IsNullOrEmpty(this.FirstName))
        //        {
        //            ret = string.Concat(this.FirstName, " ", this.LastName);
        //        }
        //        return ret;
        //    }
        //}


        [Display(ResourceType = typeof(Resource), Name = "user_fullname")]
        public string FullName
        {
            get; set;
        }


        [IgnoreDataMember]
        public string Profiles
        {
            get
            {
                if (this.ProfilList != null)
                {
                    return string.Join(", ", this.ProfilList.Select(x=>x.Name));
                }
                return null;
            }
        }
        [IgnoreDataMember]
        public bool GetIsRoot {
            get {
                if (this.Status.Value == (int)Statuts.BACKOFFICE) return true;
                return false;
            }
        }

    }
}
