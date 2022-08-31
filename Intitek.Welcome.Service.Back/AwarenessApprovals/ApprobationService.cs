using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intitek.Welcome.Service.Back
{
    public class ApprobationService : BaseService, IApprobationService
    {
        private readonly ApprobationDataAccess _approbationrepository;
        public ApprobationService(ILogger logger) : base(logger)
        {
            _approbationrepository = new ApprobationDataAccess(uow);
        }
        public List<ApprobationDTO> ListApprobations(GetAwarenessApprovalsRequest request)
        {
            List<ApprobationDTO> response = _approbationrepository.ListAll(request.Month).Select(list =>
            {
                return new ApprobationDTO
                {
                    Mois = list.Mois,
                    Entité = list.Entité,
                    Agence = list.Agence,
                    FullName = list.FullName,
                    Email = list.Email,
                    Actif = list.Actif,
                    Present = list.Present,
                    Nom_Document = list.Nom_Document,
                    Version = list.Version,
                    Resultat = list.Resultat,
                    ID_User = list.ID_User,
                    ID_Document = list.ID_Document
                };
            }).ToList();
            return response;
        }
        public List<ApprobationDTO> ListApprobationsUsers(GetAwarenessApprovalsRequest request)
        {
            List<ApprobationDTO> response = _approbationrepository.ListAllUsers(request.Month).Select(list =>
            {
                return new ApprobationDTO
                {
                    Mois = list.Mois,
                    Entité = list.Entité,
                    Agence = list.Agence,
                    FullName = list.FullName,
                    Email = list.Email,
                    Actif = list.Actif,
                    Present = list.Present,
                    ID_User = list.ID_User
                };
            }).ToList();
            return response;
        }
        public List<ApprobationDTO> ListApprobationsDocsPerUser(GetAwarenessApprovalsRequest request)
        {
            List<ApprobationDTO> response = _approbationrepository.ListAllDocsPerUser(request.Month).Select(list =>
            {
                return new ApprobationDTO
                {
                    ID_User = list.ID_User,
                    ID_Document = list.ID_Document,
                    Resultat = list.Resultat
                };
            }).ToList();
            return response;
        }
        public List<ApprobationDTO> ListApprobationsDocs(GetAwarenessApprovalsRequest request)
        {
            List<ApprobationDTO> response = _approbationrepository.ListAllDocs(request.Month).Select(list =>
            {
                return new ApprobationDTO
                {
                    Mois = list.Mois,
                    Nom_Document = list.Nom_Document,
                    ID_Document = list.ID_Document,
                    Version = list.Version
                };
            }).ToList();
            return response;
        }

    }
}
