using Intitek.Welcome.Domain;
using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public interface IProfilService
    {
        int GetAllCount(GridMvcRequest request);
        List<Profile> GetAll();
        List<ProfilDTO> GetAll(GridMvcRequest request);
        GetProfilResponse Get(GetProfilRequest request);
        List<DocumentDTO> ListDocument(GetProfilRequest request);
        List<UserDTO> ListUserByProfilID(int profilID, bool isactive);
        List<UserDTO> ListUsersForRelance(int profilID);
        ProfilDTO GetProfilById(int idProfil, bool getAssoc);
        /// <summary>
        /// Etats documents
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<DocumentDTO> ListDocumentByProfileId(GetProfilRequest request);
        void Delete(int profilID);
        void Save(SaveProfilRequest request);
        bool IsProfilNameExist(SaveProfilRequest request);



    }
}
