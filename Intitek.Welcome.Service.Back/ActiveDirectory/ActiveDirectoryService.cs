using System;
using System.Collections.Generic;
using Intitek.Welcome.Infrastructure.Log;
using System.DirectoryServices;
using System.Configuration;
using Intitek.Welcome.DataAccess;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Diagnostics;

namespace Intitek.Welcome.Service.Back
{
    public class ActiveDirectoryService : BaseService
    {
        #region Singleton
        private BlackListDataAccess _blacklistRepository;

        private static readonly Lazy<ActiveDirectoryService> lazy = new Lazy<ActiveDirectoryService>(() => new ActiveDirectoryService(new FileLogger()));

        public static ActiveDirectoryService Instance { get { return lazy.Value; } }

        public ActiveDirectoryService(ILogger logger) : base(logger)
        {
            _blacklistRepository = new BlackListDataAccess(uow);
            this.domainAddress = ConfigurationManager.AppSettings["LDAP"];
        }

        public ActiveDirectoryService(ILogger logger, string address, string domain, string userName, string password) : base(logger)
        {
            _blacklistRepository = new BlackListDataAccess(uow);
            this.domainAddress = address; // string.Format("LDAP://{0}", domain);
            this.domainDomain = domain;
            this.connexionUsername = userName;
            this.connexionPassword = password;
        }
        #endregion

        #region Properties

        private string domainAddress = string.Empty;
        private string domainDomain = string.Empty;
        private string connexionUsername = string.Empty;
        private string connexionPassword = string.Empty;
        private string domainIntitek = string.Empty;

        #endregion

        /// <summary>
        /// Définit un utilisateur à partir de son nom dans l'AD
        /// </summary>
        /// <param name="login">Login de l'utilisateur</param>
        /// <returns>L'utilisateur enregistré dans l'AD</returns>
        public UserLDAP ActiveDirectoryUserByLogin(string login, bool required)
        {
            UserLDAP user = new UserLDAP();
            user.Login = login;
            if (required)
            {               
                try
                {
                   if (string.IsNullOrWhiteSpace(login))
                        return user;

                DirectoryEntry entry = new DirectoryEntry(string.Format("LDAP://{0}/{1}", this.domainAddress, this.domainDomain), this.connexionUsername, this.connexionPassword);
                DirectorySearcher search = new DirectorySearcher(entry);

                    // specify the search filter
                    search.Filter = "(&(objectClass=user)(SamAccountName=" + login + "))";
                    // specify which property values to return in the searchd
                    search.PropertiesToLoad.Add("mail");        // smtp mail address
                    search.PropertiesToLoad.Add("givenName");   // first name
                    search.PropertiesToLoad.Add("sn");          // last name
                                                                // perform the search
                    SearchResult result;
                    SearchResultCollection ResultList = search.FindAll();
                    if (ResultList == null || ResultList.Count == 0)
                        return user;
                    else
                    {
                        result = ResultList[0];
                        if (result.Properties.Contains("sn"))
                        {
                            user.FullName = (string)result.Properties["sn"][0];
                        }
                        if (result.Properties.Contains("givenName"))
                        {
                            user.FirstName = (string)result.Properties["givenName"][0];
                        }
                        if (result.Properties.Contains("mail"))
                        {
                            user.Email = (string)result.Properties["mail"][0];
                        }
                    }

                    return user;
                }
                catch (Exception ex)
                {
                    //return (null);
                    this._logger.Error(new ExceptionLogger()
                    {
                        ExceptionDateTime = DateTime.Now,
                        ExceptionStackTrack = ex.StackTrace,
                        MethodName = "ActiveDirectoryUserByLogin",
                        ServiceName = "ActiveDirectoryService",

                    }, ex);
                }
            }
            return user;
        }
        public GetAllUserADResponse GetAllUsers(GetAllUserADRequest request)
        {
            PrincipalContext Ad = new PrincipalContext(ContextType.Domain, this.domainAddress, this.connexionUsername, this.connexionPassword);
            var response = new GetAllUserADResponse()
            {
                IsOK = true,
                ErrMessage = string.Empty,
                Users = new List<UserLDAP>(),
                DiscardedUsers = new List<string>()
            };
            var blackLists = GetBlackList();
            try
            {
                UserPrincipal User = new UserPrincipal(Ad);
                User.Enabled = true;
                PrincipalSearcher HndSearch = new PrincipalSearcher(User);
                var Results = HndSearch.FindAll();
                foreach (var u in Results)
                {
                    DirectoryEntry sr = u.GetUnderlyingObject() as DirectoryEntry;
                    var distinguishedName = (sr.Properties["distinguishedName"] != null && sr.Properties["distinguishedName"].Count > 0) ? sr.Properties["distinguishedName"][0].ToString() : string.Empty;
                    if(distinguishedName.Contains(",OU=") && sr.Properties["extensionAttribute8"] != null && sr.Properties["extensionAttribute8"].Count > 0)
                    {
                        //if (Location(distinguishedName).Length == 6)
                        if (sr.Properties["extensionAttribute8"][0].ToString().Equals("Structure") || sr.Properties["extensionAttribute8"][0].ToString().Equals("Métier"))
                        {
                            var myLocation = new LocationLDAP()
                            {
                                Entity = (sr.Properties["company"] != null && sr.Properties["company"].Count > 0) ? sr.Properties["company"][0].ToString() : string.Empty, //!string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 2 ? Location(distinguishedName)[1].ToUpper() : string.Empty,
                                Agency = (sr.Properties["l"] != null && sr.Properties["l"].Count > 0) ? sr.Properties["l"][0].ToString() : string.Empty, //!string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 3 ? Location(distinguishedName)[2].ToUpper() : string.Empty,
                                Country = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 4 ? Location(distinguishedName)[3].ToUpper() : string.Empty,
                                Zone = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 5 ? Location(distinguishedName)[4].ToUpper() : string.Empty,
                            };

                            var myLocationAgency = new LocationLDAP()
                            {
                                Entity = string.Empty,
                                Agency = (sr.Properties["l"] != null && sr.Properties["l"].Count > 0) ? sr.Properties["l"][0].ToString() : string.Empty, //!string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 3 ? Location(distinguishedName)[2].ToUpper() : string.Empty,
                                Country = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 4 ? Location(distinguishedName)[3].ToUpper() : string.Empty,
                                Zone = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 5 ? Location(distinguishedName)[4].ToUpper() : string.Empty,
                            };

                            var myLocationCountry = new LocationLDAP()
                            {
                                Entity = string.Empty,
                                Agency = string.Empty,
                                Country = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 4 ? Location(distinguishedName)[3].ToUpper() : string.Empty,
                                Zone = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 5 ? Location(distinguishedName)[4].ToUpper() : string.Empty,
                            };

                            var myLocationZone = new LocationLDAP()
                            {
                                Entity = string.Empty,
                                Agency = string.Empty,
                                Country = string.Empty,
                                Zone = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 5 ? Location(distinguishedName)[4].ToUpper() : string.Empty,
                            };

                            response.Users.Add(new UserLDAP()
                            {
                                Login = sr.Properties["samaccountname"][0].ToString(),
                                FirstName = (sr.Properties["givenName"] != null && sr.Properties["givenName"].Count > 0) ? sr.Properties["givenName"][0].ToString() : FullName(distinguishedName)[0],
                                FullName = !string.IsNullOrEmpty(distinguishedName) ? FullName(distinguishedName)[0] : string.Empty,
                                Email = (sr.Properties["mail"] != null && sr.Properties["mail"].Count > 0) ? sr.Properties["mail"][0].ToString() : string.Empty,
                                Type = (sr.Properties["extensionAttribute8"] != null && sr.Properties["extensionAttribute8"].Count > 0) ? sr.Properties["extensionAttribute8"][0].ToString().Equals("Structure") ? "STR" : "MET" : string.Empty,
                                Entity = (sr.Properties["company"] != null && sr.Properties["company"].Count > 0) ? sr.Properties["company"][0].ToString() : string.Empty, //!string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 2 ? Location(distinguishedName)[1] : string.Empty,
                                Agency = (sr.Properties["l"] != null && sr.Properties["l"].Count > 0) ? sr.Properties["l"][0].ToString() : string.Empty, //!string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 3 ? Location(distinguishedName)[2] : string.Empty,
                                Department = (sr.Properties["department"] != null && sr.Properties["department"].Count > 0) ? sr.Properties["department"][0].ToString() : string.Empty,
                                Division = (sr.Properties["division"] != null && sr.Properties["division"].Count > 0) ? sr.Properties["division"][0].ToString() : string.Empty,
                                Country = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 4 ? Location(distinguishedName)[3] : string.Empty,
                                Zone = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 5 ? Location(distinguishedName)[4] : string.Empty,
                                EntryDate = (sr.Properties["whenCreated"] != null && sr.Properties["whenCreated"].Count > 0) ? DateTime.Parse(sr.Properties["whenCreated"][0].ToString()) : DateTime.Now,
                                Location = myLocation,
                                IsBlackListed = blackLists.Contains(myLocation) || blackLists.Contains(myLocationAgency) || blackLists.Contains(myLocationCountry) || blackLists.Contains(myLocationZone)
                            });
                        }
                        else
                        {
                            var discardedUser = new UserLDAP()
                            {
                                Login = sr.Properties["samaccountname"][0].ToString(),
                                FirstName = (sr.Properties["givenName"] != null && sr.Properties["givenName"].Count > 0) ? sr.Properties["givenName"][0].ToString() : FullName(distinguishedName)[0],
                                FullName = !string.IsNullOrEmpty(distinguishedName) ? FullName(distinguishedName)[0] : string.Empty,
                                Email = (sr.Properties["mail"] != null && sr.Properties["mail"].Count > 0) ? sr.Properties["mail"][0].ToString() : string.Empty

                            };

                            response.DiscardedUsers.Add(string.Format("Path: {0} - User: {1} {2}", distinguishedName, discardedUser.Login, discardedUser.Email));
                        }
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionMessage = string.Format("{0} {1} :{2}", request.ADName, request.ADDomain, ex.StackTrace),
                    ExceptionStackTrack = string.Format("{0} {1} :{2}", request.ADName, request.ADDomain, ex.StackTrace),
                    MethodName = "GetAll",
                    ServiceName = "ActiveDirectoryService",

                }, ex);
                return new GetAllUserADResponse()
                {
                    IsOK = false,
                    ErrMessage = string.Format("{0} {1}", ex.Message, ex.InnerException),
                    Users = new List<UserLDAP>()
                };
            }
        }

        public List<UserLDAP> GetAll()
        {
            var response = new List<UserLDAP>();
            SearchResultCollection results;
            DirectorySearcher ds = null;
            DirectoryEntry de = new DirectoryEntry(string.Format("LDAP://{0}/OU=UTILISATEURS,{1}", this.domainAddress, this.domainDomain), this.connexionUsername, this.connexionPassword);
            //de.Path = "OU=TANA,DC=astekmtius,DC=ltd";

            ds = new DirectorySearcher(de);

            ds.Filter = "(&(objectClass=user))";
            //ds.PropertiesToLoad.Add("mail");
            try
            {
                ds.PageSize = 5000;
                results = ds.FindAll();

                foreach (SearchResult sr in results)
                {
                    var distinguishedName = (sr.Properties["distinguishedName"] != null && sr.Properties["distinguishedName"].Count > 0) ? sr.Properties["distinguishedName"][0].ToString() : string.Empty;
                    response.Add(new UserLDAP()
                    {
                        Login = sr.Properties["samaccountname"][0].ToString(),
                        FirstName = (sr.Properties["givenName"] !=null && sr.Properties["givenName"].Count > 0) ? sr.Properties["givenName"][0].ToString() : FullName(distinguishedName)[0],                       
                        FullName = !string.IsNullOrEmpty(distinguishedName) ? FullName(distinguishedName)[0] : string.Empty,
                        Email = (sr.Properties["mail"] != null && sr.Properties["mail"].Count > 0) ? sr.Properties["mail"][0].ToString() : string.Empty,
                        Entity = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 2 ? Location(distinguishedName)[1] : string.Empty,
                        Agency = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 1 ? Location(distinguishedName)[2] : string.Empty,                        
                        Country = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 3 ? Location(distinguishedName)[3] : string.Empty
                    });
                }

                return response;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public GetAllUserADResponse GetAll(GetAllUserADRequest request)
        {
            var response = new GetAllUserADResponse()
            {
                IsOK = true,
                ErrMessage = string.Empty,
                Users = new List<UserLDAP>(),
                DiscardedUsers = new List<string>()
            };

            var blackLists = GetBlackList();
            SearchResultCollection results;
            DirectorySearcher ds = null;
            DirectoryEntry de = new DirectoryEntry(string.Format("LDAP://{0}/OU=UTILISATEURS,{1}", this.domainAddress, this.domainDomain), this.connexionUsername, this.connexionPassword);
            //de.Path = "OU=TANA,DC=astekmtius,DC=ltd";

            ds = new DirectorySearcher(de);
            ds.PageSize = 5000;
            ds.Filter = "(&(objectClass=user))";
            //ds.PropertiesToLoad.Add("mail");
            try
            {
                results = ds.FindAll();

                foreach (SearchResult sr in results)
                {
                    var distinguishedName = (sr.Properties["distinguishedName"] != null && sr.Properties["distinguishedName"].Count > 0) ? sr.Properties["distinguishedName"][0].ToString() : string.Empty;
                    if (Location(distinguishedName).Length == 6)
                    {
                        var myLocation =  new LocationLDAP()
                        {
                            Entity = (sr.Properties["company"] != null && sr.Properties["company"].Count > 0) ? sr.Properties["company"][0].ToString() : string.Empty, //!string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 2 ? Location(distinguishedName)[1].ToUpper() : string.Empty,
                            Agency = (sr.Properties["l"] != null && sr.Properties["l"].Count > 0) ? sr.Properties["l"][0].ToString() : string.Empty, //!string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 3 ? Location(distinguishedName)[2].ToUpper() : string.Empty,
                            Country = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 4 ? Location(distinguishedName)[3].ToUpper() : string.Empty,
                            Zone = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 5 ? Location(distinguishedName)[4].ToUpper() : string.Empty,
                        };

                        var myLocationAgency = new LocationLDAP()
                        {
                            Entity = string.Empty,
                            Agency = (sr.Properties["l"] != null && sr.Properties["l"].Count > 0) ? sr.Properties["l"][0].ToString() : string.Empty, //!string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 3 ? Location(distinguishedName)[2].ToUpper() : string.Empty,
                            Country = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 4 ? Location(distinguishedName)[3].ToUpper() : string.Empty,
                            Zone = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 5 ? Location(distinguishedName)[4].ToUpper() : string.Empty,
                        };

                        var myLocationCountry = new LocationLDAP()
                        {
                            Entity = string.Empty,
                            Agency = string.Empty,
                            Country = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 4 ? Location(distinguishedName)[3].ToUpper() : string.Empty,
                            Zone = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 5 ? Location(distinguishedName)[4].ToUpper() : string.Empty,
                        };

                        var myLocationZone = new LocationLDAP()
                        {
                            Entity = string.Empty,
                            Agency = string.Empty,
                            Country = string.Empty,
                            Zone = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 5 ? Location(distinguishedName)[4].ToUpper() : string.Empty,
                        };

                        response.Users.Add(new UserLDAP()
                        {
                            Login = sr.Properties["samaccountname"][0].ToString(),
                            FirstName = (sr.Properties["givenName"] != null && sr.Properties["givenName"].Count > 0) ? sr.Properties["givenName"][0].ToString() : FullName(distinguishedName)[0],
                            FullName = !string.IsNullOrEmpty(distinguishedName) ? FullName(distinguishedName)[0] : string.Empty,
                            Email = (sr.Properties["mail"] != null && sr.Properties["mail"].Count > 0) ? sr.Properties["mail"][0].ToString() : string.Empty,
                            Type = (sr.Properties["extensionAttribute8"] != null && sr.Properties["extensionAttribute8"].Count > 0) ? sr.Properties["extensionAttribute8"][0].ToString().Equals("Structure") ? "STR" : "MET" : string.Empty,
                            Entity = (sr.Properties["company"] != null && sr.Properties["company"].Count > 0) ? sr.Properties["company"][0].ToString() : string.Empty, //!string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 2 ? Location(distinguishedName)[1] : string.Empty,
                            Agency = (sr.Properties["l"] != null && sr.Properties["l"].Count > 0) ? sr.Properties["l"][0].ToString() : string.Empty, //!string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 3 ? Location(distinguishedName)[2] : string.Empty,
                            Department = (sr.Properties["department"] != null && sr.Properties["department"].Count > 0) ? sr.Properties["department"][0].ToString() : string.Empty,
                            Division = (sr.Properties["division"] != null && sr.Properties["division"].Count > 0) ? sr.Properties["division"][0].ToString() : string.Empty,
                            Country = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 4 ? Location(distinguishedName)[3] : string.Empty,
                            Zone = !string.IsNullOrEmpty(distinguishedName) && Location(distinguishedName).Length > 5 ? Location(distinguishedName)[4] : string.Empty,
                            EntryDate = (sr.Properties["whenCreated"] != null && sr.Properties["whenCreated"].Count > 0) ? DateTime.Parse(sr.Properties["whenCreated"][0].ToString()) : DateTime.Now,
                            Location = myLocation,
                            IsBlackListed = blackLists.Contains(myLocation) || blackLists.Contains(myLocationAgency) || blackLists.Contains(myLocationCountry) || blackLists.Contains(myLocationZone)
                        });
                    }
                    else
                    {
                        var discardedUser = new UserLDAP()
                        {
                            Login = sr.Properties["samaccountname"][0].ToString(),
                            FirstName = (sr.Properties["givenName"] != null && sr.Properties["givenName"].Count > 0) ? sr.Properties["givenName"][0].ToString() : FullName(distinguishedName)[0],
                            FullName = !string.IsNullOrEmpty(distinguishedName) ? FullName(distinguishedName)[0] : string.Empty,
                            Email = (sr.Properties["mail"] != null && sr.Properties["mail"].Count > 0) ? sr.Properties["mail"][0].ToString() : string.Empty
                           
                        };

                        response.DiscardedUsers.Add(string.Format("Path: {0} - User: {1} {2}", distinguishedName, discardedUser.Login, discardedUser.Email));
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionMessage = string.Format("{0} {1} :{2}", request.ADName, request.ADDomain, ex.StackTrace),
                    ExceptionStackTrack = string.Format("{0} {1} :{2}",  request.ADName , request.ADDomain, ex.StackTrace),
                    MethodName = "GetAll",
                    ServiceName = "ActiveDirectoryService",

                }, ex);
                return new GetAllUserADResponse()
                {
                    IsOK = false,
                    ErrMessage = string.Format("{0} {1}", ex.Message, ex.InnerException),
                    Users = new List<UserLDAP>()
                };
            }
        }

        private string[] Location(string ou)
        {
            string[] separator = { ",OU=" };
            int start = ou.IndexOf(",OU=");
            int end = ou.IndexOf(",DC=");

            var _location = ou.Substring(start, end - start);
            return _location.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        private string[] FullName(string cn)
        {
            string[] separator = { "CN=" };
            int start = cn.IndexOf("CN=");
            int end = cn.IndexOf(",OU=");

            var _location = cn.Substring(start, end - start);
            return _location.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        private List<LocationLDAP> GetBlackList()
        {
            
            string[] separator = { "/" };
            var blackList = new List<LocationLDAP>();
            var blacklistpath = _blacklistRepository.FindAll();
            foreach (var blp in blacklistpath)
            {
                int end = blp.Path.IndexOf("/*");

                var _location = blp.Path.Substring(0, end);
                var locations = _location.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                blackList.Add(new LocationLDAP()
                {
                    Entity = locations.Length > 4 ? locations[4].ToUpper() : string.Empty,
                    Agency = locations.Length > 3 ? locations[3].ToUpper() : string.Empty,
                    Country = locations.Length > 2 ? locations[2].ToUpper() : string.Empty,
                    Zone = locations.Length > 1 ? locations[1].ToUpper() : string.Empty,
                });
            }

            return blackList;
        }
       
    }
}
