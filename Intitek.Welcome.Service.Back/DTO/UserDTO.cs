using Intitek.Welcome.Domain;
using System;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public class UserDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }

        public Nullable<int> Status { get; set; }
        public string EntityName { get; set; }
        public string AgencyName { get; set; }
        public bool IsOnBoarding { get; set; }
        public string Email { get; set; }
        public string EmailOnBoarding { get; set; }
        public bool Active { get; set; }
        public string Type { get; set; }
        public DateTime? InactivityStart { get; set; }
        public DateTime? InactivityEnd { get; set; }
        public string InactivityReason { get; set; }
        public string Division { get; set; }
        public string Departement { get; set; }
        public int? ID_Manager { get; set; }
        public bool Activity { get; set; }
        public List<Profile> ProfilList { get; set; }
        public List<int> ProfilListFilter { get; set; }

        public int DocumentRead { get; set; }
        public int DocumentApproved { get; set; }
        public int DocumentTested { get; set; }

        /// <summary>
        /// statistiques des collaborateurs d'un manager
        /// </summary>
        public List<Statistiques> Statistiques { get; set; }
        /// <summary>
        /// Ensemble des documents qui ont déclenché la relance d'un collaborateur
        /// </summary>
        public List<StatistiqueDocs> StatistiqueDocs { get; set; }
        public DateTime? ExitDate { get; set; }
        public DateTime? EntryDate { get; set; }
        public IntitekUser Manager { get; set; }
        public ImportManager ImportManager { get; set; }
        public bool isReader { get; set; }

    }
}
