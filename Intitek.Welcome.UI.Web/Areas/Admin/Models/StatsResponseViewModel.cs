using Intitek.Welcome.Service.Back;
using Intitek.Welcome.UI.ViewModels.Admin;
using Intitek.Welcome.UI.Web.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Intitek.Welcome.UI.Web.Admin
{
    public class StatsResponseViewModel
    {
        [DisplayName("Uniquement les relances")]
        public bool OnlyLatePeople { get; set; }
        public StatsResponseViewModel()
        {
            GridStats = new List<GridBO<Statistiques>>();
            EntityNameList = new List<OptionStatDTO>();
            ListDocument = new List<DocumentViewModel>();
            ReponseEntityNames = new List<string>();
            AgenceNameDic = new Dictionary<string, List<string>>();
            EmployeesList = new List<IntOption>();
            Datasets = new List<ChartDataset>();
        }


        public StatsResponseViewModel(GetStatsRequest statsRequest)
        {
            GridStats = new List<GridBO<Statistiques>>();
            EntityNameList = new List<OptionStatDTO>();
            ListDocument = new List<DocumentViewModel>();
            ReponseEntityNames = new List<string>();
            AgenceNameDic = new Dictionary<string, List<string>>();
            EmployeesList = new List<IntOption>();
            Datasets = new List<ChartDataset>();
        }

        public void SetResponse(GetStatsResponse reponse)
        {
            ReponseEntityNames = reponse.DivisionList;
            Total = reponse.Total;
            NotRead = reponse.NotRead;
            NotApproved = reponse.NotApproved;
            NotTested = reponse.NotTested;
            ToApproved = reponse.ToApproved;
            ToTested = reponse.ToTested;
        }

        public List<OptionStatDTO> DepartementNameList { get; set; }
        public List<OptionStatDTO> EntityNameList { get; set; }
        public List<OptionStatDTO> AncestorManagerList { get; set; } = new List<OptionStatDTO>();
        public List<OptionStatDTO> ManagerList { get; set; }
        public List<IntOption> EmployeesList { get; set; }
        public Dictionary<string, List<string>> AgenceNameDic { get; set; }
        public List<DocumentViewModel> ListDocument { get; set; }
        public int[] MultiDocSelect { get; set; }

        public string[] MultiEntitySelect { get; set; }
        public string[] MultiDepartementSelect { get; set; }
        public string[] MultiManagerSelect { get; set; }
        public int EmployeeSelect { get; set; }
        public List<string> ReponseEntityNames { get; set; }
        public List<GridBO<Statistiques>> GridStats { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int NotRead { get; set; }
        public int NotApproved { get; set; }
        public int NotTested { get; set; }
        public int Total { get; set; }
        public int ToTested { get; set; }
        public int ToApproved { get; set; }

        public List<ChartDataset> Datasets { get; set; }
        public DateTime Periode { get; set; }
        public int NbDay { get; set; }
        public string ActiveLink(int value)
        {
            var activeLink = "";
            if (value == NbDay)
            {
                activeLink = "active";
            }
            return activeLink;
        }
        private List<DateTime> Dates
        {
            get
            {
                List<DateTime> lst = new List<DateTime>();
                DateTime dtDebut = Periode.AddDays(-NbDay + 1);
                lst.Add(dtDebut);
                for (int i = 1; i <= NbDay; i++)
                {
                    lst.Add(dtDebut.AddDays(i));
                }
                return lst;
            }
        }
        private int LineWidth(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Monday)
                return 2;
            return 1;

        }
        public string LineWidths
        {
            get
            {
                var width = "1";
                if (NbDay > 20)
                {
                    return "[" + string.Join(",", Dates.Select(x => LineWidth(x))) + "]";
                }
                return width;
            }
        }
        public string JsonDatasets
        {
            get
            {
                var dataset = Datasets.Select(x => x.Json);
                return "[" + string.Join(",", dataset) + "]";
            }
        }
    }
}