using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intitek.Welcome.Service.Back
{
    public class SensibilisationService : BaseService, ISensibilisationService
    {
        private readonly SensibilisationDataAccess _sensibilisationrepository;
        public SensibilisationService(ILogger logger) : base(logger)
        {
            _sensibilisationrepository = new SensibilisationDataAccess(uow);
        }
        public List<SensibilisationDTO> ListSensibilisations(GetAwarenessApprovalsRequest request)
        {
            List<SensibilisationDTO> response = _sensibilisationrepository.ListAll(request.Month).Select(list =>
            {
                return new SensibilisationDTO
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
                    ID_Document = list.ID_Document,
                    Test_Terminé = list.Test_Terminé,
                    Score = list.Score,
                    ScoreMinimal = list.ScoreMinimal 
                };
            }).ToList();
            return response;
        }
        public List<SensibilisationDTO> ListSensibilisationsUsers(GetAwarenessApprovalsRequest request)
        {
            List<SensibilisationDTO> response = _sensibilisationrepository.ListAllUsers(request.Month).Select(list =>
            {
                return new SensibilisationDTO
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
        public List<SensibilisationDTO> ListSensibilisationsDocsPerUser(GetAwarenessApprovalsRequest request)
        {
            List<SensibilisationDTO> response = _sensibilisationrepository.ListAllDocsPerUser(request.Month).Select(list =>
            {
                return new SensibilisationDTO
                {
                    ID_User = list.ID_User,
                    ID_Document = list.ID_Document,
                    Resultat=list.Resultat
                };
            }).ToList();
            return response;
        }
        public List<SensibilisationDTO> ListSensibilisationsDocs(GetAwarenessApprovalsRequest request)
        {
            List<SensibilisationDTO> response = _sensibilisationrepository.ListAllDocs(request.Month).Select(list =>
            {
                return new SensibilisationDTO
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
