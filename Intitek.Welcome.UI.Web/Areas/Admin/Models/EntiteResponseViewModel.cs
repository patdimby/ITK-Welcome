using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intitek.Welcome.UI.Web.Admin
{
    public class EntiteResponseViewModel
    {
        public string EntityName { get; set; }
        public string AgencyName { get; set; }
        public List<string> EntityNameList { get; set; }
        public GridBO<Statistiques> EntityGrid { get; set; }
        public GridBO<Statistiques> ListAgency { get; set; }
        public GridBO<DocumentViewModel> ListDocuments { get; set; }
        public GridBO<DocumentViewModel> ListDocumentAgences { get; set; }
    }
}