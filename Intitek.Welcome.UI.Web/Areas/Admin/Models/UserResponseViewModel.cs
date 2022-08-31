using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels.Admin;
using System;
using System.Collections.Generic;

namespace Intitek.Welcome.UI.Web.Admin.Models
{
    public class UserResponseViewModel
    {
        public UserViewModel User { get; set; }
        public string EntityName { get; set; }
        public string AgencyName { get; set; }
        public List<string> EntityNameList { get; set; }
        public List<string> AgencyNameList { get; set; }
        public GridBO<UserViewModel> ListUser { get; set; }
        public GridBO<ProfilViewModel> ListProfil { get; set; }
        public GridBO<DocumentViewModel> ListDocument { get; set; }
        public List<StringOption> ActifList { get; set; }
        public string Actif { get; set; }
        public string Activity { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? ExitDate { get; set; }

    }
}