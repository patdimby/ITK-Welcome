using System;

namespace Intitek.Welcome.UI.ViewModels
{
    public enum Status
    {
        None = -1,
        BACKOFFICE = 0,
        FRONTOFFICE = 10,
        SPYOFFICE = 20
    };
    public class UserViewModel
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }

        public int Status { get; set; }
        public string EntityName { get; set; }
        public string AgencyName { get; set; }
        public bool IsOnBoarding { get; set; }
        public string EmailOnBoarding { get; set; }
        public string PasswordOnBoarding { get; set; }
        public Nullable<System.DateTime> DateLastVisit { get; set; }

        #region Données LDAP
        public string CompanyLogo { get; set; }
        public string NomPrenom { get; set; }
        #endregion
        public int ID_AD { get; set; }
        public bool isReader { get; set; }

    }
}
