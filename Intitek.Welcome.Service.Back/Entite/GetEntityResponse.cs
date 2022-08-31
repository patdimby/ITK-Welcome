using Intitek.Welcome.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intitek.Welcome.Service.Back
{

    public class StatistiquesTeam
    {
        public List<StatistiquesDTO> StatsUser = new List<StatistiquesDTO>();
        public int ToTested { get; set; }
        public int NotTested { get; set; }
        public int ToRead { get; set; }
        public int NotRead { get; set; }
        public int ToApproved { get; set; }
        public int NotApproved { get; set; }
        public int Total { get; set; }
        public bool Sensibilisant { get; set; } = false;
        public int Id { get; set; }
        public string Name { get; set; }

        public StatistiquesTeam(List<StatistiquesDTO> lststat, int userID)
        {
            StatsUser = lststat.Where(x => x.IdUser == userID).ToList();
            var notRead = StatsUser.Where(x => !x.IsRead.HasValue).Count();
            var read = StatsUser.Where(x => x.IsRead.HasValue).Count();
            var notApproved = StatsUser.Where(x => (x.Approbation.HasValue && x.Approbation == 1) && !x.IsApproved.HasValue).Count();
            var notTested = StatsUser.Where(x => (x.Test.HasValue && x.Test == 1) && !x.IsTested.HasValue).Count();
            if (notRead > 0) NotRead += 1;
            if (notApproved > 0) NotApproved += 1;
            if (notTested > 0) NotTested += 1;
            var toApproved = StatsUser.Where(x => x.Approbation.HasValue && x.Approbation == 1).Count();
            var toTested = StatsUser.Where(x => x.Test.HasValue && x.Test == 1).Count();
            if (toApproved > 0) ToApproved += 1;
            if (toTested > 0) ToTested += 1;
            this.Total = lststat.Select(x => x.IdUser).Distinct().Count();
        }

        public StatistiquesTeam()
        {
            //
        }

    }
    public class StatistiquesDTO
    {
        public int Num { get; set; }
        public int ID { get; set; }
        public string Division { get; set; }
        public int IdUser { get; set; }
        public string EntityName { get; set; }
        public string AgencyName { get; set; }
        public string Departement { get; set; }
        public DateTime? InactivityStart { get; set; }
        public DateTime? InactivityEnd { get; set; }
        public int? ID_Manager { get; set; }
        public int? ID_Profile { get; set; }
        public int? ID_Category { get; set; }
        public int? ID_SubCategory { get; set; }

        public string Ed_EntityName { get; set; }
        public string Ed_AgencyName { get; set; }
        public int IsMetier { get; set; }
        public int IsStructure { get; set; }
        public Nullable<int> Approbation { get; set; }
        public Nullable<int> Test { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<System.DateTime> IsRead { get; set; }
        public Nullable<System.DateTime> IsApproved { get; set; }
        public Nullable<System.DateTime> IsTested { get; set; }
        public Nullable<System.DateTime> EntityDate { get; set; }
        public Nullable<System.DateTime> PrfDate { get; set; }
    }

    public class UserStats : Statistiques
    {
        public int? ID_Manager { get; set; }
        public float? Note { get; set; } = null;
        public string Couleur { get; set; } = "white";
        public string TeamColor { get; set; } = "white";
        public string Label { get; set; }
        public Labels ColorTrace { get; set; } = new Labels();


        public string Flag { get; set; }
        public string pX { get; set; }
        public string pY { get; set; }
        public string pZ { get; set; }
        public UserStats()
        {
            //
        }

        public void Init()
        {
            Flag = ColorTrace.GetLabel();
            var x = (100 * ColorTrace.X / ColorTrace.Length);
            var y = (100 * ColorTrace.Y / ColorTrace.Length);
            var z = 100 - x - y;
            pX = x.ToString() + "%";
            pY = y.ToString() + "%";
            pZ = z.ToString() + "%";
        }


        public UserStats(IntitekUser user)
        {
            ID_Manager = user.ID_Manager;
            EntityName = user.EntityName;
            AgencyName = user.AgencyName;
            Departement = user.Departement;
            Email = user.Email;
        }

        public void Copy(IntitekUser user)
        {
            ID_Manager = user.ID_Manager;
            EntityName = user.EntityName;
            AgencyName = user.AgencyName;
            Departement = user.Departement;
            Email = user.Email;
            Name = user.FullName;          
        }


        public UserStats(IntitekUser user, int id)
        {
            ID_Manager = user.ID_Manager;
            EntityName = user.EntityName;
            AgencyName = user.AgencyName;
            Departement = user.Departement;
            Email = user.Email;
            Name = user.FullName;
            UserId = id;
        }

        

        private void Test()
        {
            Sensibilisant = true;
            if((NotApproved == 0)&&(NotRead == 0)&&(NotTested == 0))
            {
                Sensibilisant = false;
            }
            if ((NotApproved > 0) || (NotRead == 0) || (NotTested == 0))
            {
                Note = -1;
            }
            if (NotRead > 0)
            {
                if(NotRead != ToRead)
                {
                    Sensibilisant = false;
                    if(NotRead > 0)
                    {
                        Note = 0;
                    }
                }
                else
                {
                    if(NotApproved > 0)
                    {
                        if (NotApproved != ToApproved)
                        {
                            Sensibilisant = false;
                            if (ToApproved > 0)
                            {
                                Note = 0;
                            }
                        }
                    }
                    if(Note == null || Note < 0)
                    {
                        if (NotTested > 0)
                        {
                            if (NotTested != ToTested)
                            {
                                Sensibilisant = false;
                                if (ToTested > 0)
                                {
                                    Note = 0;
                                }
                            }
                        }
                    }
                }
            }
            if ((NotApproved == ToApproved) && (ToRead == 0) && (NotTested == ToTested))
            {
                Sensibilisant = true;
                Note = 1;
            }
            if (Note == -1)
            {
                Couleur = "green"; 
            }
            if (Note == 0)
            {
                Couleur = "orange";
            }
            if (Note == 1)
            {
                Couleur = "red";
            }
        }

        public void Copy(Statistiques stat)
        {
            ToTested = stat.ToTested;
            NotTested = stat.NotTested;
            ToRead = stat.ToRead;
            NotRead = stat.NotRead;
            ToApproved = stat.ToApproved;
            NotApproved = stat.NotApproved;
            Total = stat.Total;
            Sensibilisant = stat.Sensibilisant;
            Test();
        }


    }

    public class ColorLabel
    {
        public ColorLabel()
        {

        }

        public string Color { get; set; }
        public string Label { get; set; }
    }

    public class Labels
    {
        public int X { get; set; } // white 
        public int Y { get; set; } // orange
        public int Z { get; set; } // green
        public int Length { get; set; }
        

        public Labels()
        {
            //
        }

        public string GetLabel()
        {
            return " "+Y.ToString() + "/" + Length.ToString()+" ";
        }

        public Labels(int length)
        {
            Length = length;
        }

       
        public Labels(Labels label)
        {
            Length = label.Length;
            X = label.X;
            Y = label.Y;
            Z = label.Z;           
        }
    }


    public class ManagerStats
    {
        public int ToTested { get; set; }
        public int NotTested { get; set; }
        public int ToRead { get; set; }
        public int NotRead { get; set; }
        public int ToApproved { get; set; }
        public int NotApproved { get; set; }

        public string Title {get; }= "SENSIBISATION DE MON EQUIPE";
        public UserStats StatUsers { get; set; }
        public List<UserStats> Collaborateurs { get; set; } = new List<UserStats>();
        public int Size { get; set; }
        public IUserService _userService { get; set; }
        public IStatsService _statsService { get; set; }
        public GetStatsRequest Request { get; set; } = new GetStatsRequest();
        public int Id { get; set; }
        public List<ColorLabel> UserState { get; set; } = new List<ColorLabel>();
        public Labels ColorTrace { get; set; } = new Labels();

        public ManagerStats(IStatsService statsService, IUserService userService)
        {
            _userService = userService;
            _statsService = statsService;
        }

        public ManagerStats()
        {
            //
        }

        private ManagerStats Manager()
        {          
            Request.StatType = StatsRequestType.Manager;
            var stats = _statsService.GetStats(Request, false);
            GetStatsResponse reponse = new GetStatsResponse();
            reponse.SetManagerStats(stats, Id);
            StatUsers = new UserStats(_userService.GetUser(Id), Id);
            if (reponse.DicoAgencyStats.Count == 0)
            {
                Id = -1;
                return null;
            }
            int n = reponse.DicoAgencyStats.Count;
            for (int i = 0; i < n; i++)
            {
                foreach (var stat in reponse.DicoAgencyStats[i].ToList())
                {
                    var user = new UserStats(_userService.GetUser(stat.UserId), stat.UserId);
                    user.Copy(stat);
                   Collaborateurs.Add(user);
                }
            }
            return this;
        }


        public Labels SetCollaborateur(List<UserStats> collabs)
        {
            var col = new Labels(collabs.Count);
            foreach (var c in collabs)
            {
                ToRead += c.ToRead;
                ToTested += c.ToTested;
                ToApproved += c.ToApproved;
                NotApproved += c.NotApproved;
                NotTested += c.NotTested;
                NotRead += c.NotRead;

                if (c.ToRead == 0)
                {
                    if (c.NotRead > 0)
                    {
                        col.X += 1;
                        if (c.ToApproved == c.NotApproved && c.ToTested == c.NotTested)
                        {
                            col.Z += 1;
                        }
                    }
                    else
                    {
                        if (c.ToTested == 0 && c.NotTested == 0)
                        {
                            if (c.NotApproved == 0)
                            {
                                col.Z += 1;
                            }
                        }
                        else
                        {
                            if (c.ToApproved == 0)
                            {
                                col.Z += 1;
                            }
                            else
                            {
                                col.Y += 1;
                            }
                        }
                    }
                }
            }
            return col;
        }

        public void GetRetails()
        {
            if (Id > 0)
            {
                ColorTrace = SetCollaborateur(Collaborateurs);
                foreach (var c in Collaborateurs)
                {
                    var count = _userService.GetManagerList(c.UserId).Count;
                    c.Label = "";
                    c.TeamColor = "white";
                    if (count > 0)
                    {
                        var elt = new ManagerStats(_statsService, _userService)
                        {
                            Id = c.UserId
                        };

                        elt.InitRequest();
                        if (elt != null)
                        {
                            elt.ColorTrace = elt.SetCollaborateur(elt.Collaborateurs);
                            c.ColorTrace = new Labels(elt.ColorTrace);
                            c.ColorTrace.X = c.ColorTrace.Length - c.ColorTrace.Y - c.ColorTrace.Z;
                            c.Init();
                            int sensibilized = elt.Collaborateurs.Where(x => x.Note == 0).Count();
                            int allSens = elt.Collaborateurs.Where(x => x.Note == 1).Count();
                            if (allSens == elt.Collaborateurs.Count)
                            {
                                c.TeamColor = "green";
                                c.Label = elt.Collaborateurs.Count + "/" + elt.Collaborateurs.Count;
                            }
                            else
                            {
                                if (sensibilized > 0)
                                {
                                    c.TeamColor = "orange";
                                    c.Label = sensibilized + "/" + elt.Collaborateurs.Count;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(c.Label))
                        {
                            var arr = c.Label.Split('/');
                            if (arr[0] == arr[1])
                            {
                                c.TeamColor = "green";
                            }
                        }                                  
                    }
                    UserState.Add(new ColorLabel { Color = c.TeamColor, Label = c.Label });
                }
                
            }            
        }

        public void InitRequest()
        {
            List<DepartementDTO> managers = _userService.ManagerList();
            List<string> departements = managers.Where(x => x.ID_Manager == Id).GroupBy(x => x.Departement).Select(x => x.Key).ToList();
            Size = departements.Count;
            var d_list = new List<string>();
            foreach (var d in departements)
            {
                d_list.Add(d + "|" + Id);
            }
            Request= new GetStatsRequest(d_list.ToArray());
            Manager();
        }       

        public int Length
        {
            get { return Collaborateurs.Count; }
        }
    }

    public class Statistiques
    {
        public int UserId { get; set; }
        public string UserLogin { get; set; }
        public string EntityName { get; set; }
        public string AgencyName { get; set; }
        public string Departement { get; set; }
        public string Division { get; set; }
        public int ID_Profile { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int ToTested { get; set; }
        public int NotTested { get; set; }
        public int ToRead { get; set; }
        public int NotRead { get; set; }
        public int ToApproved { get; set; }
        public int NotApproved { get; set; }        
        public int Total { get; set; }       
        public Nullable<System.DateTime> LastDate { get; set; }
        public List<StatistiqueDocs> StatistiqueDocs { get; set; }
        public bool Sensibilisant { get; set; } = false;
        public int TeamSize { get; set; }

    }
    public class StatistiqueDocs
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int NotTested { get; set; }
        public int NotRead { get; set; }
        public int NotApproved { get; set; }
        public int ToTested { get; set; }
        public int ToRead { get; set; }
        public int ToApproved { get; set; }
        public string LibTested { get; set; }
        public string LibRead { get; set; }
        public string LibApproved { get; set; }

    }
    public class GetEntityResponse
    {
        private List<Statistiques> _Lststat;
        public string EntityName { get; set; }
        public string AgencyName { get; set; }
        public List<string> EntityNameList { get; set; }
        public List<string> AgencyNameList { get; set; }
        public List<Statistiques> AgencyStats { get; set; }
        public Statistiques EntityStat { get; set; }

        public List<Statistiques> EntityStats {
            get {
                return new List<Statistiques>() { this.EntityStat };
            }
        }
        public GetEntityResponse()
        {
            this.AgencyStats = new List<Statistiques>();
        }
        public void SetEntityStats(List<Statistiques> lststat)
        {
            this._Lststat = lststat;
            this.AgencyStats = new List<Statistiques>();
            Statistiques stat = new Statistiques();
            stat.Name = this.EntityName;
            if (this._Lststat != null)
            {
                foreach (var item in this._Lststat)
                {
                    if (item.NotRead > 0)
                        stat.NotRead += 1;
                    if (item.NotTested > 0)
                        stat.NotTested += 1;
                    if (item.NotApproved > 0)
                        stat.NotApproved += 1;
                    stat.Total += 1;
                }
                this.EntityStat = stat;
                this.GetAgencyStats();
            }
        }
        private void GetAgencyStats()
        {
            if (this._Lststat != null)
            {
                foreach (var agence in this.AgencyNameList)
                {
                    Statistiques stat = new Statistiques();
                    stat.EntityName = this.EntityName;
                    stat.Name = agence;
                    var agstats = this._Lststat.Where(x => x.AgencyName.Equals(agence));
                    foreach (var item in agstats)
                    {
                        if (item.NotRead > 0)
                            stat.NotRead += 1;
                        if (item.NotTested > 0)
                            stat.NotTested += 1;
                        if (item.NotApproved > 0)
                            stat.NotApproved += 1;
                        stat.Total += 1;
                    }
                    this.AgencyStats.Add(stat);
                }               
            }
        }
    }
}
