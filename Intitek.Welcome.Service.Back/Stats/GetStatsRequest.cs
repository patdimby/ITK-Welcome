using Intitek.Welcome.Domain;
using System;

namespace Intitek.Welcome.Service.Back
{
    public enum StatsRequestType
    {
        Agency = 1,
        Departement = 2,
        Manager = 8
    }   
    
   

        public class GetStatsRequest
    {
        public int IdLang { get; set; }
        public int IdDefaultLang { get; set; }
        public int[] MultiDocSelect { get; set; }
        public int EmployeeSelect { get; set; }
        public string[] MultiEntitySelect { get; set; }
        public int IsSelectAllEntity { get; set; } //Sélectionner tout
        public string[] MultiDepartementSelect { get; set; }
        public int IsSelectAllDepartement { get; set; } //Sélectionner tout
        public string[] MultiManagerSelect { get; set; }
        public int IsSelectAllManager { get; set; } //Sélectionner tout
        public string EntitySelect { get; set; }
        public string SubmitButton { get; set; }
        public DateTime? Periode{ get; set; }
        public int NbDay { get; set; }
        public StatsRequestType StatType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public GetStatsRequest(GetStatsRequest req)
        {
            IdLang = req.IdLang;
            IdDefaultLang = req.IdDefaultLang;
            MultiDocSelect = req.MultiDocSelect;
            EmployeeSelect = req.EmployeeSelect;
            MultiEntitySelect = req.MultiEntitySelect;
            IsSelectAllEntity = req.IsSelectAllEntity;
            MultiDepartementSelect = req.MultiDepartementSelect;
            IsSelectAllDepartement = req.IsSelectAllDepartement;            
            IsSelectAllManager = req.IsSelectAllManager;
            EntitySelect = req.EntitySelect;
            SubmitButton = req.SubmitButton;
            Periode= req.Periode;
            NbDay = req.NbDay;
            StatType = req.StatType;
            StartDate = req.StartDate;
            EndDate = req.EndDate;
        }

        public GetStatsRequest(string[] chains)
        {
            StartDate = new DateTime(2000, 1, 1);
            EndDate = DateTime.Now;
            NbDay = 7;
            MultiDepartementSelect = chains;
        }

        public GetStatsRequest()
        {
            StartDate = new DateTime(2000, 1, 1);
            EndDate = DateTime.Now;
            NbDay = 7; 
        }
    }
}
