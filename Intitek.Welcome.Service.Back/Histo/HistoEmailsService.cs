using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Log;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace Intitek.Welcome.Service.Back
{
    public class HistoEmailsService : BaseService, IHistoEmailsService
    {
        private readonly HistoEmailsDataAccess _histoEmailsRepository;
        private readonly MailTemplateDataAccess _mailTemplateRepository;
        private readonly BatchsDataAccess _batchsRepository;
        private readonly UserDataAccess _userRepository;

        public HistoEmailsService(ILogger logger) : base(logger)
        {
            _histoEmailsRepository = new HistoEmailsDataAccess(uow);
            _mailTemplateRepository = new MailTemplateDataAccess(uow);
            _batchsRepository = new BatchsDataAccess(uow);
            _userRepository = new UserDataAccess(uow);
        }

        public IQueryable<HistoEmailsDTO> GetAllHistoEmailsAsQueryable(GetAllHistoEmailsRequest allrequest)
        {
            var request = allrequest.GridRequest;
            var limitDate = allrequest.LimitDate;
            var query = this._histoEmailsRepository.RepositoryQuery.GroupJoin(this._mailTemplateRepository.RepositoryTable,
                    histoEmail => histoEmail.Id_MailTemplate,
                    mailTemplate => mailTemplate.ID,
                    (histoEmail, mailTemplate) => new { histoEmail, mailTemplate = mailTemplate.FirstOrDefault() })
               .GroupJoin(this._batchsRepository.RepositoryTable,
                    obj => obj.histoEmail.ID_Batch,
                    batch => batch.ID,
                    (obj, batch) => new { histoEmail = obj.histoEmail, mailTemplate = obj.mailTemplate, batch = batch.FirstOrDefault() })
               .GroupJoin(this._userRepository.RepositoryTable,
                    obj => obj.histoEmail.Id_IntitekUser,
                    user => user.ID,
                    (obj, user) => new { histoEmail = obj.histoEmail, mailTemplate = obj.mailTemplate, batch = obj.batch, user = user })
               .SelectMany(x => x.user.DefaultIfEmpty(), (parent, child) => new HistoEmailsDTO()
               {
                   DateAction = parent.histoEmail.Date,
                   TemplateName = parent.mailTemplate.Name,
                   BatchProgName = parent.batch.ProgName,
                   Username = child.Username,
                   Message = parent.histoEmail.Message
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
                    query = query.Where(x => x.TemplateName.ToLower().Contains(search)
                            || x.BatchProgName.ToLower().Contains(search)
                            || x.Username.ToLower().Contains(search)
                    );
                }
                query = this.FiltrerQuery(request.Filtres, query);
            }

            return query;
        }

        public GetAllHistoEmailsResponse GetAll()
        {
            var query = _histoEmailsRepository.FindAll();

            try
            {
                return new GetAllHistoEmailsResponse()
                {
                    HistoEmails = query.Select(q => new HistoEmailsDTO()
                    {
                        DateAction = q.Date,
                        Message = q.Message,
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

        public int GetAllCount(GetAllHistoEmailsRequest request)
        {
            return GetAllHistoEmailsAsQueryable(request).Count();
        }

        public GetAllHistoEmailsResponse GetAllFromQueryable(GetAllHistoEmailsRequest allRequest)
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
            var response = new GetAllHistoEmailsResponse();
            try
            {
                IQueryable<HistoEmailsDTO> query = GetAllHistoEmailsAsQueryable(allRequest)
                    .OrderBy(orderBy).Skip((request.Page - 1) * request.Limit).Take(request.Limit);
                var list = query.ToList();
                response.HistoEmails = list;
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetAllFromQueryable",
                    ServiceName = "HistoEmailsService",

                }, ex);
                throw ex;
            }
        }

        public GetAllHistoEmailsResponse GetAll(GetAllHistoEmailsRequest allRequest)
        {
            if (allRequest.GridRequest != null)
                return GetAllFromQueryable(allRequest);
            else
            {
                var response = GetAll();
                if (allRequest.LimitDate != null)
                {
                    response.HistoEmails = response.HistoEmails.Where(histoEmail => histoEmail.DateAction < allRequest.LimitDate).ToList();
                }

                return response;
            }
        }

        public DeleteHistoEmailsResponse Delete(DeleteHistoEmailsRequest request)
        {
            IEnumerable<HistoEmails> query = null;
            if (request.LimitDate != null) query = _histoEmailsRepository.RepositoryQuery.Where(histoAction => DbFunctions.TruncateTime(histoAction.Date) <= request.LimitDate);
            else
                query = _histoEmailsRepository.RepositoryQuery;

            var response = new DeleteHistoEmailsResponse();
            try
            {
                _histoEmailsRepository.RemoveAll(query);

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Delete",
                    ServiceName = "HistoEmailsService",

                }, ex);
                throw ex;
            }
        }
    }
}
