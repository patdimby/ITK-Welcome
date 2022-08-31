using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Helpers;
using Intitek.Welcome.Infrastructure.Log;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Resources;

namespace Intitek.Welcome.Service.Back
{
    public class HistoActionsService : BaseService, IHistoActionsService
    {
        private readonly HistoActionsDataAccess _histoActionsRepository;
        private readonly UserDataAccess _userRepository;
        private readonly DocumentDataAccess _docRepository;
        private readonly QcmDataAccess _qcmRepository;
        private readonly EntityDocumentDataAccess _entityDocRepository;
        private readonly ProfileDocumentDataAccess _profileDocRepository;
        private readonly ProfileDataAccess _profileRepository;
        private readonly DocumentLangDataAccess _doclangrepository;

        public HistoActionsService(ILogger logger) : base(logger)
        {
            _histoActionsRepository = new HistoActionsDataAccess(uow);
            _userRepository = new UserDataAccess(uow);
            _docRepository = new DocumentDataAccess(uow);
            _qcmRepository = new QcmDataAccess(uow);
            _entityDocRepository = new EntityDocumentDataAccess(uow);
            _profileDocRepository = new ProfileDocumentDataAccess(uow);
            _profileRepository = new ProfileDataAccess(uow);
            _doclangrepository = new DocumentLangDataAccess(uow);

            //path for System.Link.Dynamic pour reconnaitre la classe EdmxFunction
            this.DynamicLinkPatch();
        }

        public IQueryable<HistoActionsDTO> GetAllHistoActionsAsQueryable(GetAllHistoActionsRequest allrequest)
        {
            var request = allrequest.GridRequest;
            var limitDate = allrequest.LimitDate;
            var query = this._histoActionsRepository.RepositoryQuery.GroupJoin(this._userRepository.RepositoryTable,
                    histoAction => histoAction.ID_IntitekUser,
                    user => user.ID,
                    (histoAction, user) => new { histoAction, user = user.FirstOrDefault() })
               .GroupJoin(this._docRepository.RepositoryTable,
                    obj => obj.histoAction.ID_Object,
                    doc => doc.ID,
                    (obj, doc) => new { histoAction = obj.histoAction, user = obj.user, doc = doc.FirstOrDefault() })
               .GroupJoin(
                    this._doclangrepository.RepositoryQuery.Where(l => l.ID_Lang == allrequest.IdLang),
                    docL => docL.doc.ID,
                    lang => lang.ID_Document,
                    (docL, lang) => new { docL, lang }).SelectMany(x => x.lang.DefaultIfEmpty(), (parent, child) => new { docL = parent.docL, lang = child })

               .GroupJoin(this._qcmRepository.RepositoryTable,
                    obj => obj.docL.histoAction.ID_Object,
                    qcm => qcm.Id,
                    (obj, qcm) => new { histoAction = obj.docL.histoAction, user = obj.docL.user, lang = obj.lang, qcm = qcm })
               .SelectMany(x => x.qcm.DefaultIfEmpty(), (parent, child) => new HistoActionsDTO()
               {
                   ID = parent.histoAction.ID,
                   ID_Object = parent.histoAction.ID_Object,
                   ID_IntitekUser = parent.histoAction.ID_IntitekUser,
                   Username = parent.user.Username,
                   ObjectCode = parent.histoAction.ObjectCode,
                   Action = parent.histoAction.Action,
                   DateAction = parent.histoAction.DateAction,
                   LinkedObjects = parent.histoAction.LinkedObjects,
                   Description = parent.histoAction.ObjectCode == "QUIZZ" ? string.Empty : parent.lang.Name
               });

            if (limitDate != null)
            {
                query = query.Where(x => DbFunctions.TruncateTime(x.DateAction) <= limitDate);
            }
            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.Search))
                {
                    string search = request.Search.ToLower();
                    string searchSansA = Utils.RemoveAccent(request.Search);
                    query = query.Where(x => x.Username.ToLower().Contains(search)
                            || EdmxFunction.RemoveAccent(x.Description).Contains(searchSansA)
                            || EdmxFunction.RemoveAccent(x.LinkedObjects).Contains(searchSansA)
                    );
                }
                query = this.FiltrerQuery(request.Filtres, query);
            }

            return query;
        }

        private List<HistoActionsDTO> SetQuizzHistoActionsObjectName(List<HistoActionsDTO> histoActions)
        {
            return histoActions.ToList();
        }

        public GetAllHistoActionsResponse GetAll()
        {
            var query = _histoActionsRepository.FindAll();

            try
            {
                return new GetAllHistoActionsResponse()
                {
                    HistoActions = query.Select(q => new HistoActionsDTO()
                    {
                        ID = q.Id,
                        ID_Object = q.ID_Object,
                        ID_IntitekUser = q.ID_IntitekUser,
                        ObjectCode = q.ObjectCode,
                        Action = q.Action,
                        DateAction = q.DateAction
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetAll",
                    ServiceName = "HistoActionsService",

                }, ex);
                throw ex;
            }
        }

        public int GetAllCount(GetAllHistoActionsRequest request)
        {
            return GetAllHistoActionsAsQueryable(request).Count();

        }

        public GetAllHistoActionsResponse GetAllFromQueryable(GetAllHistoActionsRequest allRequest)
        {
            string orderBy = "";
            var request = allRequest.GridRequest;
            if (string.IsNullOrEmpty(request.OrderColumn))
            {
                orderBy = "DateAction DESC";
            }
            else
            {
                orderBy = request.OrderColumn + request.SortAscDesc;
            }
            var response = new GetAllHistoActionsResponse();
            try
            {
                IQueryable<HistoActionsDTO> query = GetAllHistoActionsAsQueryable(allRequest)
                    .OrderBy(orderBy).Skip((request.Page - 1) * request.Limit).Take(request.Limit);
                var list = query.ToList();
                response.HistoActions = list;
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetAllFromQueryable",
                    ServiceName = "HistoActionsService",

                }, ex);
                throw ex;
            }
        }

        public GetAllHistoActionsResponse GetAll(GetAllHistoActionsRequest allRequest)
        {
            if (allRequest.GridRequest != null)
                return GetAllFromQueryable(allRequest);
            else
            {
                var response = GetAll();
                if(allRequest.LimitDate != null)
                {
                    response.HistoActions = response.HistoActions.Where(histoAction => histoAction.DateAction < allRequest.LimitDate).ToList();
                }

                return response;
            }
        }

        public List<string> GetAllObjectCodes()
        {
            var query = _histoActionsRepository.RepositoryTable.Select(h => h.ObjectCode).Distinct().OrderBy(h => h);
            var objectCodes = query.ToList();
            return objectCodes;
        }

        public List<string> GetAllActions()
        {
            var query = _histoActionsRepository.RepositoryTable.Select(h => h.Action).Distinct().OrderBy(h => h);
            var actions = query.ToList();
            return actions;
        }

        public List<string> GetAllActionsFullNames(ResourceManager rm)
        {
            List<string> actions = GetAllActions();
            return  actions.Select(action => rm.GetString("histoAction_" + action)).ToList();
        }

        public DeleteHistoActionsResponse Delete(DeleteHistoActionsRequest request)
        {
            IEnumerable<HistoActions> query = null; 
            if (request.LimitDate != null) query = _histoActionsRepository.RepositoryQuery.Where(histoAction => DbFunctions.TruncateTime(histoAction.DateAction) <= request.LimitDate);
            else
                query = _histoActionsRepository.RepositoryQuery;

            var response = new DeleteHistoActionsResponse();
            try
            {
                _histoActionsRepository.RemoveAll(query);

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Delete",
                    ServiceName = "HistoActionsService",

                }, ex);
                throw ex;
            }
        }
    }
}
