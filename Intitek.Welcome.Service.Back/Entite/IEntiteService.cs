using System.Collections.Generic;

namespace Intitek.Welcome.Service.Back
{
    public interface IEntiteService
    {
        List<string> AgencyByEntity(string entity, bool isactive, bool hasEmpty, bool showAgencyNull);
        List<DocumentDTO> ListDocumentEntity(GetEntityRequest request);
        List<DocumentDTO> ListDocumentAgence(GetEntityRequest request);
        void SaveEntity(SaveEntityRequest request);
        void SaveAgency(SaveEntityRequest request);
        List<DocumentDTO> DocumentStateEntity(GetEntityRequest request, bool agence);
        List<DocumentDTO> DocumentStateAgency(GetEntityRequest request);

        GetAllEntityResponse GetAllEntity(GetAllEntityRequest request);
        List<UserDTO> ListUsersForRelance(string entityName);
        List<UserDTO> ListUsersForRelance(string entityName, string agenceName);

    }
}
