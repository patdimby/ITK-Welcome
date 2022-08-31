using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Helpers;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Service.Back;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Astek.Welcome.Batch.Service
{
    public class SyncronizeAD : IBatch
    {
        private static ActiveDirectoryService activeDirectoryService;
        private static ADService _adService;
        private static UserService _userService;
        private readonly string _ivPwd;
        private readonly string _keyPwd;

        public SyncronizeAD()
        {
            _adService = new ADService(new FileLogger());
            _userService = new UserService(new FileLogger());
            _ivPwd = "qf6bYB7dJxer+CQjoVhAdQ=="; // ConfigurationManager.AppSettings["ivPwd"];
            _keyPwd = "x2pV8E8NK5Y85oHVqM0B2agPDDX9e1mJk0bJO75Hr+M="; //ConfigurationManager.AppSettings["keyPwd"];
        }

        public BatchResponse Execute(BatchRequest request, Synthese synthese)
        {
            var response = new BatchResponse() {
                Result = 0,
                Errors = new List<string>()             
            };
            var currentAD = new ADDTO();
            var errorList = new List<string>();
            var allADs = _adService.GetAll().ADs.Where(ad=> ad.ToBeSynchronized).ToList();
            var cityEntityBlacklisted = new List<EntityAgencyDTO>();
            foreach (var ad in allADs)
            {
                currentAD = ad;
                activeDirectoryService = new ActiveDirectoryService(new FileLogger(), ad.Address, ad.Domain, ad.Username, EncryptionHelper.Decrypt(ad.Password, _keyPwd, _ivPwd));
                
                //Créer une liste d'utilisateurs par rapport aux données AD
                SyntheseAD syntheseAD = new SyntheseAD() { Name= ad.Name, Address = ad.Address};
                SynchronizeADResponse syncADResponse = null;
                try
                {
                    var allUserADResponse = activeDirectoryService.GetAllUsers(new GetAllUserADRequest() {
                        ADDomain = ad.Address,
                        ADName = ad.Name
                    });
                    Console.WriteLine("Nombre d'entrées " + allUserADResponse.Users.Count());
                    if (allUserADResponse.IsOK)
                    {
                        syntheseAD.NbUsersAD = allUserADResponse.Users.Count();
                        syntheseAD.NbUsersDiscardedAD = allUserADResponse.DiscardedUsers.Count();
                        syntheseAD.NbUsersEmptyMailAD = allUserADResponse.Users.Where(u => string.IsNullOrEmpty(u.Email)).Count();
                        syntheseAD.NbUsersBlackListAD = allUserADResponse.Users.Where(u => !string.IsNullOrEmpty(u.Login) && u.IsBlackListed).Count();
                        var usersToSynchronize = allUserADResponse.Users.Select(adu => new UserAdDTO()
                        {
                            UserName = adu.Login,
                            Email = adu.Email,
                            EntityName = adu.Entity,
                            AgencyName = adu.Agency,
                            Departement = adu.Department,
                            Division = adu.Division,
                            Country = adu.Country,
                            Pays = adu.Country,
                            Plaque = adu.Zone,
                            Type = adu.Type,
                            FirstName = adu.FirstName,
                            FullName = adu.FullName,
                            ID_AD = ad.ID,
                            IsBlackListed = adu.IsBlackListed,
                            EntryDate = adu.EntryDate
                        });

                        
                        cityEntityBlacklisted.AddRange(usersToSynchronize
                                .Where(u => !string.IsNullOrEmpty(u.UserName) && u.IsBlackListed)
                                .Select(x => new EntityAgencyDTO { EntityName = x.EntityName, AgencyName = x.AgencyName })
                                .Distinct().ToList());
                        var nameAD = ad.Name + " " + ad.Address;

                        syncADResponse = _userService.SynchronizeWithAd(new SynchronizeADRequest()
                        {
                            Id_AD = ad.ID,
                            Name_AD = nameAD,
                            ADUsers = usersToSynchronize.Where(u => !string.IsNullOrEmpty(u.UserName)).ToList(),
                            ADBlackListedUsers = usersToSynchronize.Where(u => !string.IsNullOrEmpty(u.UserName) && u.IsBlackListed).ToList(),
                            BlackListedAgencies = cityEntityBlacklisted
                        });
                        //MAJ ID_Manager
                        _userService.UpdateIdManager(syncADResponse, ad.ID, nameAD);
                        _adService.Save(new SaveADRequest()
                        {
                            AD = new AD()
                            {
                                ID = ad.ID,
                                Id = ad.ID,
                                Name = ad.Name,
                                Address = ad.Address,
                                Domain = ad.Domain,
                                Username = ad.Username,
                                Password = EncryptionHelper.Decrypt(ad.Password, _keyPwd, _ivPwd),
                                ToBeSynchronized = true,
                                LastSynchronized = DateTime.Now
                            },
                            fromBatch = true,
                            PwdIV = _ivPwd,
                            PwdKey = _keyPwd
                        });

                        if (syncADResponse.Result == 0)
                        {
                            WriteToLog(syncADResponse, allUserADResponse.DiscardedUsers, ad.Name);
                        }
                        else
                        {
                            var errMessage = string.Format("AD: {0} {1} - {2}", currentAD.Name, currentAD.Address, syncADResponse.ErrorMessage);
                            errorList.Add(errMessage);
                        }

                    }
                    else
                    {
                        var errMessage = string.Format("AD: {0} {1} - {2}", currentAD.Name, currentAD.Address, allUserADResponse.ErrMessage);
                        errorList.Add(errMessage);
                    }
                }
                catch (Exception ex)
                {
                    var errMessage = string.Format("{0} {1}: {2}", currentAD.Name, currentAD.Address, ex.Message);
                    errorList.Add(errMessage);
                }
                finally
                {
                    if (syncADResponse != null)
                    {
                        syntheseAD.NbUsersAddedDB = syncADResponse.UsersToAdd.Count();
                        syntheseAD.NbUsersUpdatedDB = syncADResponse.UsersToUpdate.Count();
                        syntheseAD.NbUsersDeletedDB = syncADResponse.UsersToDelete.Count();
                        syntheseAD.Errors = syncADResponse.SyntheseADErrors;
                    }                 
                    synthese.SynchronizeADs.Add(syntheseAD);
                }
            }

            if(errorList.Count > 0)
            {
                response.Errors = errorList;
                response.Result = -1;
                synthese.Errors = errorList;
            }
            return response;
        }

        void WriteToLog(SynchronizeADResponse response, List<string> DiscardedADUsers, string adName)
        {
            var myDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var filePath = string.Format("{2}\\Logs\\log_{0}_{1:yyMMdhhmmss}.txt", EscapeFileName(adName), DateTime.Now, myDirectory);
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, true))
            {
                file.WriteLine("From AD {0}", adName);
                file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                if (response.Result == 0)
                {
                    file.WriteLine("User discarded from AD: {0}", DiscardedADUsers.Count);
                    file.WriteLine("User with empty email from AD: {0}", response.UsersADMailDiscarded.Count);
                    file.WriteLine("User AD to synchronize: {0}", response.UsersADToSync.Count());
                    file.WriteLine("User AD activated: {0}", response.Count);
                    file.WriteLine("User AD BlackListed: {0}", response.BlackListedUsers.Count);
                    file.WriteLine("Nb of Users AD added to Db: {0}", response.UsersToAdd.Count);
                    file.WriteLine("Nb of Users AD udpated to Db: {0}", response.UsersToUpdate.Count);
                    file.WriteLine("Nb of Users AD deleted from Db: {0}", response.UsersToDelete.Count);
                    file.WriteLine("");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-  USERS DISCARDED FROM AD -=-=-=-=-=-=-=-=-=-=-=-=-=");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    foreach (var discarded in DiscardedADUsers)
                    {
                                       
                        file.WriteLine(discarded);
                    }

                    file.WriteLine("");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-  USERS BLACKLISTED -=-=-=-=-=-=-=-=-=-=-=-=-=");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    foreach (var line in response.BlackListedUsers)
                    {

                        var userToSync = string.Format("{0},{1},{2},{3},{4},{5}", line.UserName, line.Email, line.EntityName, line.AgencyName, line.FullName, line.ID_AD);
                        // If the line doesn't contain the word 'Second', write the line to the file.                       
                        file.WriteLine(userToSync);
                    }

                    file.WriteLine("");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-  USERS WITH EMPTY EMAIL FROM AD -=-=-=-=-=-=-=-=-=-=-=-=-=");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    foreach (var line in response.UsersADMailDiscarded)
                    {
                        var userToSync = string.Format("{0},{1},{2},{3},{4},{5}", line.UserName, line.Email, line.EntityName, line.AgencyName, line.FullName, line.ID_AD);
                        // If the line doesn't contain the word 'Second', write the line to the file.                       
                        file.WriteLine(userToSync);
                    }

                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-  USERS FROM AD {0}-=-=-=-=-=-=-=-=-=-=-=-=-=", adName);
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    foreach (var line in response.UsersADToSync)
                    {
                        var userToSync = string.Format("{0},{1},{2},{3},{4},{5}", line.UserName, line.Email, line.EntityName, line.AgencyName, line.FullName, line.ID_AD);
                        // If the line doesn't contain the word 'Second', write the line to the file.                       
                        file.WriteLine(userToSync);
                    }
                    
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-  USERS TO ADD -=-=-=-=-=-=-=-=-=-=-=-=-=");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    foreach (var line in response.UsersToAdd)
                    {
                        var userToSync = string.Format("{0},{1},{2},{3},{4},{5}", line.Username, line.Email, line.EntityName, line.AgencyName, line.FullName, line.ID_AD);
                        // If the line doesn't contain the word 'Second', write the line to the file.                       
                        file.WriteLine(userToSync);
                    }
                   
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-  USERS TO UPDATE -=-=-=-=-=-=-=-=-=-=-=-=");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    foreach (var line in response.UsersToUpdate)
                    {
                        var userToSync = string.Format("{0},{1},{2},{3},{4},{5}", line.Username, line.Email, line.EntityName, line.AgencyName, line.FullName, line.ID_AD);
                        // If the line doesn't contain the word 'Second', write the line to the file.                       
                        file.WriteLine(userToSync);
                    }
                   
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-  USERS TO DELETE -=-=-=-=-=-=-=-=-=-=-=-=");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    foreach (var line in response.UsersToDelete)
                    {
                        var userToSync = string.Format("{0},{1},{2},{3},{4},{5}", line.Username, line.Email, line.EntityName, line.AgencyName, line.FullName, line.ID_AD);
                        // If the line doesn't contain the word 'Second', write the line to the file.                       
                        file.WriteLine(userToSync);
                    }

                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=  ERRORS -=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    foreach (var line in response.CRUDErrors)
                    {                        
                        file.WriteLine(line);
                    }

                    file.WriteLine("");
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                    file.WriteLine("User discarded from AD: {0}", DiscardedADUsers.Count);
                    file.WriteLine("User with empty email from AD: {0}", response.UsersADMailDiscarded.Count);
                    file.WriteLine("User AD to synchronize: {0}", response.UsersADToSync.Count());
                    file.WriteLine("User AD activated: {0}", response.Count);
                    file.WriteLine("User AD BlackListed: {0}", response.BlackListedUsers.Count);
                    file.WriteLine("Nb of Users AD added to Db: {0}", response.UsersToAdd.Count);
                    file.WriteLine("Nb of Users AD udpated to Db: {0}", response.UsersToUpdate.Count);
                    file.WriteLine("Nb of Users AD deleted from Db: {0}", response.UsersToDelete.Count);
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");

                }
                else
                {
                    file.WriteLine(response.ErrorMessage);
                    file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-===-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                }
            }
        }

        private string EscapeFileName(string filename)
        {
            string specialChar = @"<,>:«/\|?*.";
            var filenameWithoutSpecialChar = string.Empty;
            if(string.IsNullOrEmpty(filename))
                return filenameWithoutSpecialChar;
       
            foreach(char c in specialChar)
            {
                filename = filename.Replace(c, '_');
            }
            filenameWithoutSpecialChar = filename.Replace(' ', '_');
            return filenameWithoutSpecialChar;
        }
    }
}
