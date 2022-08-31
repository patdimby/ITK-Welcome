using Intitek.Welcome.Domain;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public interface IUserService
    {
        GetUserResponse Get(GetUserRequest request);
        bool GetActivityUser(int idUser);
        IntitekUser GetIntitekUserByLogin(string login);
        IntitekUser GetIntitekUserByEmail(string email);
        List<IntitekUser> GetIntitekUserByFullname(string fullname);
        List<string> EntityNameList(bool isactive);
        int GetCountUserActiveByEntityName(string entityName);
        List<string> AgencyNameList(bool hasEmpty);
        int GetCountUserActiveByAgencyName(string entity, string agence);
        List<string> DepartementNameList(bool hasEmpty);
        int GetCountUserActiveByDepartementName(string department);
        List<DepartementDTO> ManagerList();
        int GetCountUserActiveByIDManagerByDepartement(string departement);
        int GetCountUserActiveByIDManager(string departement, int idManager);
        int ListUsersCount(GetUserRequest request);
        List<UserDTO> ListUsers(GetUserRequest request);
        List<Profile> ListProfileByUserId(int userId);
        List<ProfilDTO> ListProfil(GetUserRequest request);
        void Save(SaveUserRequest request);
		List<DocumentDTO> ListDocuments(bool lu, GetUserRequest request) ;
        int ListDocumentReadByLoginCount(GetUserRequest request);
        List<DocumentDTO> ListDocumentReadByLogin(GetUserRequest allrequest);

        SynchronizeADResponse SynchronizeWithAd(SynchronizeADRequest request);
        void RemoveAllInactivity();
        void UpdateAll(List<IntitekUser> users);
        List<string> SetManagers(string[] ListManager);
        IntitekUser GetManager(int idManager);
        IntitekUser GetUser(int idUser);
        List<IntitekUser> GetManagerList(int idManager);
        List<string> GetUsersByName();
        List<string> GetUsersTeam(int idManager);
        List<IntitekUser> GetIntitekUserTeam(string fullname);
        List<IntitekUser> GetIntitekUsers();
    }
}
