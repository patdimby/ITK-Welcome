using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using System.Collections.Generic;
using System.Linq;

namespace Intitek.Welcome.UI.Web.Areas.Admin.Controllers
{
    public class CommonStatController : CommunController
    {
        private readonly IUserService _userService;
        private readonly IEntiteService _entiteService;
        private readonly IStatsService _statService;
        private List<StatistiquesDTO> _Lststat;
        public Statistiques EntityStat { get; private set; }
        public List<Statistiques> AgencyStats { get; private set; }
        public CommonStatController()
        {
            _userService = new UserService(new FileLogger());
            _entiteService = new EntiteService(new FileLogger());
            _statService = new StatsService(new FileLogger());
        }
        public void GetEntityStats(GetEntityRequest request)
        {
            GetStatsRequest statRequest = new GetStatsRequest { IdLang = request.IdLang, EntitySelect = request.EntityName };
            this._Lststat = _statService.GetStatsEntity(statRequest);
            this.SetEntityStats();
        }
        public void SetEntityStats()
        {
            string entityName = this._Lststat.OrderBy(x => x.EntityName).Select(x => x.EntityName).Distinct().FirstOrDefault();
            EntityStat = new Statistiques();
            EntityStat.EntityName = entityName;
            EntityStat.Name = entityName;
            if (this._Lststat != null)
            {
                //Vision globale
                List<int> userIds = this._Lststat.Select(x => x.IdUser).Distinct().ToList();
                EntityStat.NotRead = 0;
                EntityStat.NotApproved = 0;
                EntityStat.NotTested = 0;
                foreach (var userID in userIds)
                {
                    List<StatistiquesDTO> statsUser = this._Lststat.Where(x => x.IdUser == userID).ToList();
                    var notRead = statsUser.Where(x => !x.IsRead.HasValue).Count();
                    var notApproved = statsUser.Where(x => (x.Approbation.HasValue && x.Approbation == 1) && !x.IsApproved.HasValue).Count();
                    var notTested = statsUser.Where(x => (x.Test.HasValue && x.Test == 1) && !x.IsTested.HasValue).Count();
                    if (notRead > 0) EntityStat.NotRead += 1;
                    if (notApproved > 0) EntityStat.NotApproved += 1;
                    if (notTested > 0) EntityStat.NotTested += 1;
                    var toApproved = statsUser.Where(x => x.Approbation.HasValue && x.Approbation == 1).Count();
                    var toTested = statsUser.Where(x => x.Test.HasValue && x.Test == 1).Count();
                    if (toApproved > 0) EntityStat.ToApproved += 1;
                    if (toTested > 0) EntityStat.ToTested += 1;
                }
                EntityStat.Total = this._Lststat.Select(x => x.IdUser).Distinct().Count();

               
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
                AgencyStats = agencyStats;
           }
        }
    }

}