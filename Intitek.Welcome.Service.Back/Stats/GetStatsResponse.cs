using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Log;
using System.Collections.Generic;
using System.Linq;

namespace Intitek.Welcome.Service.Back
{

   

    public class GetStatsResponse
    {
        private List<StatistiquesDTO> _Lststat;
        public List<List<Statistiques>> DicoAgencyStats { get; private set; }
        public List<string> EntityNameList { get; private set; }
        public List<string>DepartementList { get; private set; }
        public List<string> DivisionList { get; private set; }
        public int NotRead { get; private set; }
        public int NotApproved { get; private set; }
        public int NotTested { get; private set; }
        public int Total { get; private set; }
        public int ToTested { get; private set; }
        public int ToApproved { get; private set; }
        private readonly IStatsService _statsService;
        public GetStatsResponse()
        {
            this.DicoAgencyStats = new List<List<Statistiques>>();
            this.EntityNameList = new List<string>();
            this.DepartementList = new List<string>();
            _statsService = new StatsService(new FileLogger());
        }

       
        public void SetEntityStats(List<StatistiquesDTO> lststat)
        {
            this._Lststat = lststat;
            this.EntityNameList = this._Lststat.OrderBy(x => x.EntityName).Select(x => x.EntityName).Distinct().ToList();
            if (this._Lststat != null && this._Lststat.Any())
            {
                //Vision globale
                List<int> userIds = this._Lststat.Select(x => x.IdUser).Distinct().ToList();
                this.NotRead = 0;
                this.NotApproved = 0;
                this.NotTested = 0;
                foreach (var userID in userIds)
                {
                    List<StatistiquesDTO> statsUser = this._Lststat.Where(x => x.IdUser == userID).ToList();
                    var notRead = statsUser.Where(x => !x.IsRead.HasValue).Count();
                    var notApproved = statsUser.Where(x => (x.Approbation.HasValue && x.Approbation == 1) && !x.IsApproved.HasValue).Count();
                    var notTested = statsUser.Where(x => (x.Test.HasValue && x.Test == 1) && !x.IsTested.HasValue).Count();
                    if (notRead > 0) this.NotRead += 1;
                    if (notApproved > 0) this.NotApproved += 1;
                    if (notTested > 0) this.NotTested += 1;
                    var toApproved = statsUser.Where(x => x.Approbation.HasValue && x.Approbation == 1).Count();
                    var toTested = statsUser.Where(x => x.Test.HasValue && x.Test == 1).Count();
                    if (toApproved > 0) this.ToApproved += 1;
                    if (toTested > 0) this.ToTested += 1;
                }
                this.Total = this._Lststat.Select(x => x.IdUser).Distinct().Count();

                foreach (var entityName in this.EntityNameList)
                {

                    var agenceNameList = this._Lststat.Where(x => x.EntityName.Equals(entityName)).OrderBy(x => x.AgencyName).Select(x => x.AgencyName).Distinct().ToList();
                    var agencyStats = new List<Statistiques>();
                    foreach (var agenceName in agenceNameList)
                    {
                        Statistiques stat = new Statistiques();
                        stat.Name = agenceName;
                        stat.EntityName = entityName;
                        stat.AgencyName = agenceName;
                        stat.NotRead = 0;
                        stat.NotApproved = 0;
                        stat.NotTested = 0;
                        List<StatistiquesDTO> LststatByAgence = this._Lststat.Where(x => x.EntityName.Equals(entityName) && x.AgencyName.Equals(agenceName)).ToList();
                        List<int> userAgIds = LststatByAgence.Select(x => x.IdUser).Distinct().ToList();
                        foreach (var userID in userAgIds)
                        {
                            List<StatistiquesDTO> statsUser = LststatByAgence.Where(x => x.IdUser == userID).ToList();
                            var notRead = statsUser.Where(x => !x.IsRead.HasValue).Count();
                            var notApproved = statsUser.Where(x => (x.Approbation.HasValue && x.Approbation == 1) && !x.IsApproved.HasValue).Count();
                            var notTested = statsUser.Where(x => (x.Test.HasValue && x.Test == 1) && !x.IsTested.HasValue).Count();
                            if (notRead > 0) stat.NotRead += 1;
                            if (notApproved > 0) stat.NotApproved += 1;
                            if (notTested > 0) stat.NotTested += 1;

                            var toApproved = statsUser.Where(x => x.Approbation.HasValue && x.Approbation == 1).Count();
                            var toTested = statsUser.Where(x => x.Test.HasValue && x.Test == 1).Count();
                            if (toApproved > 0) stat.ToApproved += 1;
                            if (toTested > 0) stat.ToTested += 1;
                        }
                        stat.Total = userAgIds.Count();
                        agencyStats.Add(stat);
                    }
                    this.DicoAgencyStats.Add(agencyStats);
                }

            }
        }
        public void SetDepartementStats(List<StatistiquesDTO> lststat)
        {
            this._Lststat = lststat;
            this.DepartementList = this._Lststat.OrderBy(x => x.Departement).Select(x => string.IsNullOrEmpty(x.Departement)? Constante.NO_DIRECTION_ID :x.Departement).Distinct().ToList();
            if (this._Lststat != null && this._Lststat.Any())
            {
                //Vision globale
                List<int> userIds = this._Lststat.Select(x => x.IdUser).Distinct().ToList();
                this.NotRead = 0;
                this.NotApproved = 0;
                this.NotTested = 0;
                this.ToApproved = 0;
                this.ToTested = 0;
                foreach (var userID in userIds)
                {
                    List<StatistiquesDTO> statsUser = this._Lststat.Where(x => x.IdUser == userID).ToList();
                    var notRead = statsUser.Where(x => !x.IsRead.HasValue).Count();
                    var notApproved = statsUser.Where(x => (x.Approbation.HasValue && x.Approbation == 1) && !x.IsApproved.HasValue).Count();
                    var notTested = statsUser.Where(x => (x.Test.HasValue && x.Test == 1) && !x.IsTested.HasValue).Count();
                    if (notRead > 0) this.NotRead += 1;
                    if (notApproved > 0) this.NotApproved += 1;
                    if (notTested > 0) this.NotTested += 1;
                    var toApproved = statsUser.Where(x => x.Approbation.HasValue && x.Approbation == 1).Count();
                    var toTested = statsUser.Where(x => x.Test.HasValue && x.Test == 1).Count();
                    if (toApproved > 0) this.ToApproved += 1;
                    if (toTested > 0) this.ToTested += 1;
                }
                this.Total = this._Lststat.Select(x => x.IdUser).Distinct().Count();

                var departementStats = new List<Statistiques>();
                foreach (var departement in this.DepartementList)
                {
                    Statistiques stat = new Statistiques();
                    stat.Name = departement;
                    stat.NotRead = 0;
                    stat.NotApproved = 0;
                    stat.NotTested = 0;
                    stat.ToApproved = 0;
                    stat.ToTested = 0;
                    List<StatistiquesDTO> LststatByDir = null;
                    if (departement.Equals(Constante.NO_DIRECTION_ID))
                    {
                        LststatByDir = this._Lststat.Where(x => string.IsNullOrEmpty(x.Departement)).ToList();
                    }
                    else
                    {
                        LststatByDir = this._Lststat.Where(x => !string.IsNullOrEmpty(x.Departement) && x.Departement.Equals(departement)).ToList();
                    }
                        
                    List<int> userAgIds = LststatByDir.Select(x => x.IdUser).Distinct().ToList();
                    foreach (var userID in userAgIds)
                    {
                        List<StatistiquesDTO> statsUser = LststatByDir.Where(x => x.IdUser == userID).ToList();
                        var notRead = statsUser.Where(x => !x.IsRead.HasValue).Count();
                        var notApproved = statsUser.Where(x => (x.Approbation.HasValue && x.Approbation == 1) && !x.IsApproved.HasValue).Count();
                        var notTested = statsUser.Where(x => (x.Test.HasValue && x.Test == 1) && !x.IsTested.HasValue).Count();
                        if (notRead > 0) stat.NotRead += 1;
                        if (notApproved > 0) stat.NotApproved += 1;
                        if (notTested > 0) stat.NotTested += 1;
                        
                        var toApproved = statsUser.Where(x => x.Approbation.HasValue && x.Approbation == 1).Count();
                        var toTested = statsUser.Where(x => x.Test.HasValue && x.Test == 1).Count();
                        if (toApproved > 0) stat.ToApproved += 1;
                        if (toTested > 0) stat.ToTested += 1;
                    }
                    stat.Total = userAgIds.Count();
                    departementStats.Add(stat);
                }
                this.DicoAgencyStats.Add(departementStats);

            }            
        }
        public void SetManagerStats(List<StatistiquesDTO> lststat, int idManager = 0)
        {
            if (idManager > 0)
            {
                lststat = lststat.Where(x => x.ID_Manager == idManager).ToList();
            }
            this._Lststat = lststat;
            var divisions = this._Lststat.OrderBy(x => x.Departement).ThenBy(x => x.Division).GroupBy(o => new { o.ID_Manager, o.Division, o.Departement }).Select(m => new { ID = m.Key.ID_Manager, Departement=m.Key.Departement, Division=m.Key.Division, Name = string.Format("{0} ({1})",m.Key.Division, m.Key.Departement)}).ToList();
            this.DivisionList = new List<string>();
           
            if (this._Lststat != null && this._Lststat.Any())
            {
                //Vision globale
                List<int> userIds = new List<int>();              
                userIds = this._Lststat.Select(x => x.IdUser).Distinct().ToList();
               
                var users = _statsService.ListUsersForStat(userIds, true, false, StatsRequestType.Manager);
                this.NotRead = 0;
                this.NotApproved = 0;
                this.NotTested = 0;
                this.ToApproved = 0;
                this.ToTested = 0;
                foreach (var userID in userIds)
                {
                    //var teamStat = new StatistiquesTeam(lststat, userID);
                    List<StatistiquesDTO> statsUser = this._Lststat.Where(x => x.IdUser == userID).ToList();
                    var notRead = statsUser.Where(x => !x.IsRead.HasValue).Count();
                    var read = statsUser.Where(x => x.IsRead.HasValue).Count();
                    var notApproved = statsUser.Where(x => (x.Approbation.HasValue && x.Approbation == 1) && !x.IsApproved.HasValue).Count();
                    var notTested = statsUser.Where(x => (x.Test.HasValue && x.Test == 1) && !x.IsTested.HasValue).Count();                    
                    if (notRead > 0) this.NotRead += 1;
                    if (notApproved > 0) this.NotApproved += 1;
                    if (notTested > 0) this.NotTested += 1;
                    var toApproved = statsUser.Where(x => x.Approbation.HasValue && x.Approbation == 1).Count();
                    var toTested = statsUser.Where(x => x.Test.HasValue && x.Test == 1).Count();
                    if (toApproved > 0) this.ToApproved += 1;
                    if (toTested > 0) this.ToTested += 1;                   
                }
                this.Total = this._Lststat.Select(x => x.IdUser).Distinct().Count();
                foreach (var division in divisions)
                {
                    this.DivisionList.Add(division.Name);
                    var collabsList = this._Lststat.Where(x => x.ID_Manager.HasValue && x.Departement.Equals(division.Departement) && x.Division.Equals(division.Division)).Distinct().ToList();
                    var collabStats = new List<Statistiques>();
                    List<int> userAgIds = collabsList.Select(x => x.IdUser).Distinct().ToList();
                    var userAgs = users.Where(x => userAgIds.Contains(x.ID)).ToList();
                    foreach (var user in userAgs)
                    {
                        //Nombre de documents par collaborateur
                        Statistiques stat = new Statistiques()
                        {
                            UserId = user.ID,
                           Name = user.FullName,
                        };
                      
                        
                        List<StatistiquesDTO> statsUser = collabsList.Where(x => x.IdUser == user.ID).ToList();
                        stat.NotRead = statsUser.Where(x => !x.IsRead.HasValue).Count();
                        
                        stat.NotApproved = statsUser.Where(x => (x.Approbation.HasValue && x.Approbation == 1) && !x.IsApproved.HasValue).Count();
                        stat.NotTested = statsUser.Where(x => (x.Test.HasValue && x.Test == 1) && !x.IsTested.HasValue).Count();

                        stat.ToApproved = statsUser.Where(x => x.Approbation.HasValue && x.Approbation == 1).Count();
                        stat.ToTested = statsUser.Where(x => x.Test.HasValue && x.Test == 1).Count();
                        
                        stat.Total = statsUser.Count();

                        if((stat.NotRead == 0)&&(stat.ToRead > 0))
                        {
                            stat.Sensibilisant = false;
                        }
                        else
                        {
                            if((stat.ToApproved == 0) &&(stat.ToTested == 0)){
                                stat.Sensibilisant = true;
                            }
                            else
                            {
                                if ((stat.NotApproved == 0)&& (stat.ToApproved > 0))
                                {
                                    if (stat.ToTested == 0)
                                    {
                                        stat.Sensibilisant = true;
                                    }
                                   if(stat.NotTested == 0)
                                    {
                                        stat.Sensibilisant = true;
                                    }
                                }

                            }
                        }
                        collabStats.Add(stat);
                    }
                    this.DicoAgencyStats.Add(collabStats);
                }
 
            }
        }
    }
}
