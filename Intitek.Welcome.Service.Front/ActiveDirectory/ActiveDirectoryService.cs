using Intitek.Welcome.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Intitek.Welcome.Infrastructure.Log;
using System.Runtime.InteropServices;

namespace Intitek.Welcome.Service.Front
{
    public class ActiveDirectoryService : BaseService
    {
        #region Singleton

        private static readonly Lazy<ActiveDirectoryService> lazy = new Lazy<ActiveDirectoryService>(() => new ActiveDirectoryService(new FileLogger()));

        public static ActiveDirectoryService Instance { get { return lazy.Value; } }

        public ActiveDirectoryService(ILogger logger) : base(logger)
        {
            this.domainAddress = ConfigurationManager.AppSettings["LDAP"];
            this.domainDomain = ConfigurationManager.AppSettings["DomainContainer"];
        }

        public ActiveDirectoryService(ILogger logger, string address, string domain, string userName, string password) : base(logger)
        {
            this.domainAddress = address; // string.Format("LDAP://{0}", domain);
            this.domainDomain = domain;
            this.connexionUsername = userName;
            this.connexionPassword = password;
        }
        #endregion

        #region Properties

        private string domainAddress = string.Empty;
        private string connexionUsername = string.Empty;
        private string connexionPassword = string.Empty;
        private string domainIntitek = string.Empty;
        private string domainDomain = string.Empty;
        #endregion

        /// <summary>
        /// Validate username & password against AD server
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Authenticate(string username, string password)
        {
            DirectoryEntry entry = new DirectoryEntry(string.Format("LDAP://{0}", domainAddress),  username, password);

            try
            {
                //object obj = entry.NativeObject;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(SAMAccountName=" + username + ")";
                SearchResult result = search.FindOne();
                if (null == result)
                {
                    return false;
                }
    
            }
            catch(Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Authenticate",
                    ServiceName = "ActiveDirectoryService",
                    UserName = username
                }, ex);
                return false;
            }

            return true;

        }


        public bool CheckEmailAddress(string emailAddress)
        {
            var context = new PrincipalContext(ContextType.Domain);           
            DirectoryEntry entry = new DirectoryEntry();
            DirectorySearcher search = new DirectorySearcher(entry);
            search.Filter = "(&(objectClass=user)(mail=" + emailAddress + "))";
          
           
            SearchResultCollection ResultList = search.FindAll();
            if (ResultList == null || ResultList.Count == 0)
                return false;
            
            return true;
        }
       
        /// <summary>
        /// Test si l'username et le password sont valides dans l'active directory
        /// </summary>
        /// <param name="username">Nom d'utilisateur</param>
        /// <param name="password">Mot de passe</param>
        /// <returns>True si l'utilisateur et le mot de passe sont valides dans l'AD. False sinon</returns>
        public Boolean IsAllowedInActiveDirectory(string username, string password, ref string Entity, ref string Agency, ref string LongName)
        {
            LongName = string.Empty;
            var CurrentActiveDirectory = new DirectoryEntry(string.Format("LDAP://{0}", domainAddress), username, password);

            #region code with ActiveDirectory enabled
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return false;

                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domainAddress, domainDomain))
                {
                    //_logger.Info("mot de passe :" + password);
                    if (!pc.ValidateCredentials(username, password, ContextOptions.ServerBind))
                    {
                        _logger.Info(string.Format("L'utilisateur {0} n'existe pas dans Active Directory {1} ou le mot de passe est invalide", username, domainAddress));
                        return false;
                    }
                    username = username.ToLower();
                    try
                    {
                        //using (var searcher = new PrincipalSearcher(new UserPrincipal(adContext)))
                        using (var searcher = new DirectorySearcher(CurrentActiveDirectory))
                        {
                            searcher.Filter = "(SAMAccountName=" + username + ")";
                            searcher.PropertiesToLoad.Add("cn");
                            SearchResult result = searcher.FindOne();
                            if (result != null)
                            {
                                LongName = result.Properties["cn"][0].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(new ExceptionLogger()
                        {
                            ExceptionDateTime = DateTime.Now,
                            ExceptionStackTrack = ex.StackTrace,
                            MethodName = "IsAllowedInActiveDirectory",
                            ServiceName = "ActiveDirectoryService",
                            UserName = username

                        }, ex);
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Info(string.Format("Connexion impossible au serveur LDAP://{0}", domainAddress));
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "IsAllowedInActiveDirectory",
                    ServiceName = "ActiveDirectoryService",
                    UserName = username
                }, ex);
               
                return false;
            }
            #endregion
        }

       
        /// <summary>
        /// Définit un utilisateur à partir de son nom dans l'AD
        /// </summary>
        /// <param name="login">Login de l'utilisateur</param>
        /// <returns>L'utilisateur enregistré dans l'AD</returns>
        public UserLDAP ActiveDirectoryUserByLogin(string login)
        {
            try
            {
                UserLDAP user = new UserLDAP();
                user.Login = login;
                if (string.IsNullOrWhiteSpace(login))
                    return user;

                // Preparation
                PrincipalContext adContext = new PrincipalContext(ContextType.Domain);
                DirectoryEntry entry = new DirectoryEntry();
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
                        user.LastName = (string)result.Properties["sn"][0];
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
            catch
            {
                return (null);
            }
        }
        /// <summary>
        /// Liste toutes les entités d'INTITEK
        /// </summary>
        /// <returns>Liste de chaines de caractères contenant le nom des entités</returns>
        public List<string> ListAllEntities()
        {
            try
            {
                List<string> result = null;

                DirectoryEntry ADentry = new DirectoryEntry(domainAddress, connexionUsername, connexionPassword);

                if (ADentry != null && ADentry.Children != null)
                {
                    result = new List<string>();
                    DirectoryEntry utilisateursNode = ADentry.Children.Find("OU=UTILISATEURS");

                    if (utilisateursNode != null)
                    {
                        foreach (DirectoryEntry directoryItem in utilisateursNode.Children)
                        {
                            string name = directoryItem.Name;

                            string schema = directoryItem.SchemaClassName;

                            if (schema == "organizationalUnit")
                            {
                                result.Add(name.Replace("OU=", string.Empty));
                            }
                        }
                    }

                    ADentry.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "ListAllEntities",
                    ServiceName = "ActiveDirectoryService",

                }, ex);

                throw ex;
            }
        }

        /// <summary>
        /// Liste toutes les agences d'une entité
        /// </summary>
        /// <param name="entityName">Nom de l'entité</param>
        /// <returns>Liste de chaines de caractères contenant le nom des agences</returns>
        public List<string> ListAllAgencies(string entityName)
        {
            try
            {
                List<string> result = null;

                DirectoryEntry ADentry = new DirectoryEntry(domainAddress, connexionUsername, connexionPassword);

                if (ADentry != null && ADentry.Children != null)
                {
                    result = new List<string>();

                    DirectoryEntry utilisateursNode = ADentry.Children.Find("OU=UTILISATEURS");

                    if (utilisateursNode != null)
                    {
                        string ADEntityName = "OU=" + entityName;

                        DirectoryEntry subDirectory = utilisateursNode.Children.Find(ADEntityName);

                        if (subDirectory != null)
                        {
                            foreach (DirectoryEntry subDirectoryItem in subDirectory.Children)
                            {
                                string subName = subDirectoryItem.Name;

                                string subSchema = subDirectoryItem.SchemaClassName;

                                if (subSchema == "organizationalUnit")
                                {
                                    result.Add(subName.Replace("OU=", string.Empty));
                                }
                            }
                        }
                    }
                
                    ADentry.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetQcm",
                    ServiceName = "ActiveDirectoryService",

                }, ex);

                throw ex;
            }
        }

        /// <summary>
        /// Liste tous les utilisateurs d'une agence
        /// </summary>
        /// <param name="agencyName">Nom de l'agence</param>
        /// <returns>Liste de chaines de caractères contenant les noms des utilisateurs</returns>
        public Dictionary<String, String> ListAllUserLoginNameByEntityThenByAgency(string entityName, string agencyName)
        {

            Dictionary<String, String> RetVal;  // Return value

            try
            {
                RetVal = new Dictionary<string, string>();

                DirectoryEntry ADentry = new DirectoryEntry(domainAddress, connexionUsername, connexionPassword);
                DirectoryEntry utilisateursNode = ADentry.Children.Find("OU=UTILISATEURS");

                if (utilisateursNode != null)
                {
                    string ADEntityName = "OU=" + entityName;

                    DirectoryEntry entityNode = utilisateursNode.Children.Find(ADEntityName);

                    if (entityNode != null)
                    {

                        string ADAgencyName = "OU=" + agencyName;

                        DirectoryEntry AgencyNode = entityNode.Children.Find(ADAgencyName);

                        if (AgencyNode != null)
                        {
                            foreach (DirectoryEntry userItem in AgencyNode.Children)
                            {
                                //connexion a l'ad
                                var principalContext = new PrincipalContext(ContextType.Domain, domainIntitek, connexionUsername, connexionPassword);
                                //Vérifie si le compte est actif
                                if ((Convert.ToInt32(userItem.Properties["userAccountControl"].Value) & 0x0002) == 0)
                                {
                                    //activé dans l'ad alors on l'ajoute a la liste
                                    string login = userItem.Properties["SamAccountName"].Value.ToString();
                                    string subSchema = userItem.SchemaClassName;
                                    //si le shema dan l'ad correspond a un "user"
                                    if (subSchema == "user")
                                    {
                                        RetVal.Add(login, userItem.Properties["DisplayName"].Value.ToString());
                                    }
                                }
                            }
                        }
                    }

                }

                ADentry.Close();
                return RetVal;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetQcm",
                    ServiceName = "ActiveDirectoryService",

                }, ex);

                throw ex;
            }
        }


        /// <summary>
        /// Liste tous les utilisateurs d'une agence
        /// </summary>
        /// <param name="agencyName">Nom de l'agence</param>
        /// <returns>Liste de chaines de caractères contenant les noms des utilisateurs</returns>
        public List<string> ListAllUserLoginByEntityThenByAgency(string entityName, string agencyName)
        {
            try
            {
                List<string> result = null;

                DirectoryEntry ADentry = new DirectoryEntry(domainAddress, connexionUsername, connexionPassword);

                if (ADentry != null && ADentry.Children != null)
                {
                    result = new List<string>();

                    DirectoryEntry utilisateursNode = ADentry.Children.Find("OU=UTILISATEURS");

                    if (utilisateursNode != null)
                    {
                        string ADEntityName = "OU=" + entityName;

                        DirectoryEntry entityNode = utilisateursNode.Children.Find(ADEntityName);

                        if (entityNode != null)
                        {

                            string ADAgencyName = "OU=" + agencyName;

                            DirectoryEntry AgencyNode = entityNode.Children.Find(ADAgencyName);

                            ////liste pour stocker les emails
                            //List<string> listEmailNonRenseigneDansAd = new List<string>();
                            if (AgencyNode != null)
                            {
                                foreach (DirectoryEntry userItem in AgencyNode.Children)
                                {
                                    //connexion a l'ad
                                    var principalContext = new PrincipalContext(ContextType.Domain, domainIntitek, connexionUsername, connexionPassword);
                                    //Vérifie si le compte est actif
                                    if ((Convert.ToInt32(userItem.Properties["userAccountControl"].Value) & 0x0002) == 0)
                                    {
                                        //activé dans l'ad alors on l'ajoute a la liste
                                        string login = userItem.Properties["SamAccountName"].Value.ToString();
                                        string subSchema = userItem.SchemaClassName;
                                        //si le shema dan l'ad correspond a un "user"
                                        if (subSchema == "user")
                                        {
                                            result.Add(login);
                                        }
                                    }


                                }
   
                            }

                        }

                    }

                    ADentry.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetQcm",
                    ServiceName = "ActiveDirectoryService",

                }, ex);

                throw ex;
            }
        }

        /// <summary>
        /// Get AD as XML String, can be filter by Entity and/or agency
        /// </summary>
        /// <returns></returns>
        public String GetAdasXML(String EntityName, String AgencyName)
        {
            String AllIntitek = "<root>";

            List<String> Entities = ActiveDirectoryService.Instance.ListAllEntities();
            foreach (var Entity in Entities)
            {
                if (String.IsNullOrEmpty(EntityName) || Entity == EntityName)
                {
                    AllIntitek += String.Format("<Entity Name=|{0}|>", Entity).Replace('|', '"');

                    List<String> Agencies = ActiveDirectoryService.Instance.ListAllAgencies(Entity);
                    foreach (var Agency in Agencies)
                    {

                        if (String.IsNullOrEmpty(AgencyName) || Agency == AgencyName)
                        {
                            AllIntitek += String.Format("<Agency Name=|{0}|>", Agency).Replace('|', '"');

                            Dictionary<String, String> Users = ActiveDirectoryService.Instance.ListAllUserLoginNameByEntityThenByAgency(Entity, Agency);

                            foreach (var User in Users)
                            {
                                AllIntitek += String.Format("<user><DisplayName>{0}</DisplayName><Login>{1}</Login></user>", User.Value, User.Key.ToLower());
                            }
                            AllIntitek += String.Format("</Agency>");
                        }
                    }
                    AllIntitek += String.Format("</Entity>");
                }
            }
            AllIntitek += "</root>";

            return (AllIntitek);
        }

        /// <summary>
        /// Get AD as XML String, for several entities
        /// </summary>
        /// <returns></returns>
        public String GetAdasXML(String[] EntityNames)
        {
            String AllIntitek = "<root>";

            List<String> Entities = ActiveDirectoryService.Instance.ListAllEntities();
            foreach (var Entity in Entities)
            {
                if (EntityNames == null || Array.IndexOf(EntityNames, Entity) > -1)
                {
                    AllIntitek += String.Format("<Entity Name=|{0}|>", Entity).Replace('|', '"');

                    List<String> Agencies = ActiveDirectoryService.Instance.ListAllAgencies(Entity);
                    foreach (var Agency in Agencies)
                    {

                        AllIntitek += String.Format("<Agency Name=|{0}|>", Agency).Replace('|', '"');

                        Dictionary<String, String> Users = ActiveDirectoryService.Instance.ListAllUserLoginNameByEntityThenByAgency(Entity, Agency);

                        foreach (var User in Users)
                        {
                            AllIntitek += String.Format("<user><DisplayName>{0}</DisplayName><Login>{1}</Login></user>", User.Value, User.Key);
                        }
                        AllIntitek += String.Format("</Agency>");
                    }
                    AllIntitek += String.Format("</Entity>");
                }
            }
            AllIntitek += "</root>";

            return (AllIntitek);
        }

        /// <summary>
        /// Liste tous les utilisateurs d'une entité
        /// </summary>
        /// <param name="entityName">Nom de l'entité</param>
        /// <returns>Liste de chaines de caractères contenant les noms des utilisateurs</returns>
        public List<string> ListEntityUserLoginByEntityName(string entityName, string agencyName = "")
        {
            string optionAgency = null;
            if (agencyName != "")
            {
                optionAgency = agencyName;
            }

            try
            {
                List<string> result = new List<string>();

                List<string> agencies = ListAllAgencies(entityName);
                if (optionAgency == null)
                {
                    foreach (string agency in agencies)
                    {
                        List<string> agencyUsers = ListAllUserLoginByEntityThenByAgency(entityName, agency);

                        if (agencyUsers != null && agencyUsers.Count > 0)
                        {
                            result.AddRange(agencyUsers);
                        }
                    }
                    return result;
                }
                else
                {
                    List<string> agencyUsers = ListAllUserLoginByEntityThenByAgency(entityName, optionAgency);

                    if (agencyUsers != null && agencyUsers.Count > 0)
                    {
                        result.AddRange(agencyUsers);
                    }
                    return result;
                }



            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetQcm",
                    ServiceName = "ActiveDirectoryService",

                }, ex);

                throw ex;
            }
        }


        /// <summary>
        /// Récupère le nom de l'entité et l'agence d'un utilisateur
        /// </summary>
        /// <param name="Login">Login de l'utilisateur</param>
        /// <returns>Un dictionnaire contenant le nom de l'entité et le nom de l'agence de l'utilisateur</returns>
        public Dictionary<string, string> GetEntityThenAgencyByUserLogin(string Login)
        {
            try
            {
                Dictionary<string, string> result = null;
                if (!string.IsNullOrWhiteSpace(Login))
                {
                    // récupère l'utilisateur
                    PrincipalContext adContext = new PrincipalContext(ContextType.Domain);
                    UserPrincipal user = UserPrincipal.FindByIdentity(adContext, IdentityType.SamAccountName, Login);
                    // Récupère l'agence
                    DirectoryEntry deUser = user.GetUnderlyingObject() as DirectoryEntry;
                    DirectoryEntry deUserContainer = deUser.Parent;
                    string Agency = deUserContainer.Properties["Name"].Value.ToString();
                    // Récupère la société
                    DirectoryEntry deUserContainerParent = deUserContainer.Parent;
                    string Entity = deUserContainerParent.Properties["Name"].Value.ToString();
                    // Contrôles
                    if (string.IsNullOrEmpty(Agency) == false && string.IsNullOrEmpty(Entity) == false)
                    {
                        result = new Dictionary<string, string>();
                        result.Add("Entity", Entity);
                        result.Add("Agency", Agency);
                    }
                }
                return result;
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// Récupère les données utilisateur
        /// </summary>
        /// <param name="Login">Login de l'utilisateur</param>
        /// <returns>Un dictionnaire contenant le nom de l'entité et le nom de l'agence de l'utilisateur</returns>
        public IntitekUser GetDataByUserLogin(string Login)
        {
            try
            {
                IntitekUser result = null;

                if (!string.IsNullOrWhiteSpace(Login))
                {
                    // récupère l'utilisateur
                    PrincipalContext adContext = new PrincipalContext(ContextType.Domain);
                    UserPrincipal user = UserPrincipal.FindByIdentity(adContext, Login);
                    // Récupère l'agence
                    DirectoryEntry deUser = user.GetUnderlyingObject() as DirectoryEntry;
                    DirectoryEntry deUserContainer = deUser.Parent;
                    string Agency = deUserContainer.Properties["Name"].Value.ToString();
                    // Récupère la société
                    DirectoryEntry deUserContainerParent = deUserContainer.Parent;
                    string Entity = deUserContainerParent.Properties["Name"].Value.ToString();

                    if (user.Name.ToString() != null)
                    {
                        result = new IntitekUser();
                        result.EntityName = Entity;
                        result.AgencyName = Agency;
                        //result.Em = user.EmailAddress;
                        return result;
                    }

                }
                return result;
            }
            catch
            {
                return null;
            }

        }
    }
}
