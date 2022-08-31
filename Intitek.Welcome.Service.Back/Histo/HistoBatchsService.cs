using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Helpers;
using Intitek.Welcome.Infrastructure.Log;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace Intitek.Welcome.Service.Back
{
    public class HistoBatchsService : BaseService, IHistoBatchsService
    {
        private readonly HistoBatchsDataAccess _histoBatchsRepository;
        private readonly BatchsDataAccess _batchsRepository;

        public HistoBatchsService(ILogger logger) : base(logger)
        {
            _histoBatchsRepository = new HistoBatchsDataAccess(uow);
            _batchsRepository = new BatchsDataAccess(uow);
            //path for System.Link.Dynamic pour reconnaitre la classe EdmxFunction
            this.DynamicLinkPatch();
        }

        public IQueryable<HistoBatchsDTO> GetAllHistoBatchsAsQueryable(GetAllHistoBatchsRequest allrequest)
        {
            var request = allrequest.GridRequest;
            var limitDate = allrequest.LimitDate;
            var query = this._histoBatchsRepository.RepositoryQuery.GroupJoin(this._batchsRepository.RepositoryTable,
                    histoBatch => histoBatch.ID_Batch,
                    batch => batch.ID,
                    (histoBatch, batch) => new { histoBatch, batch })
               .SelectMany(x => x.batch.DefaultIfEmpty(), (parent, child) => new HistoBatchsDTO()
               {
                   DateStart = parent.histoBatch.Start,
                   DateEnd = parent.histoBatch.Finish,
                   BatchProgName = child.ProgName,
                   Description = child.Description,
                   ReturnCode = parent.histoBatch.ReturnCode,
                   Message = parent.histoBatch.Message
               });
            if (limitDate != null)
            {
                query = query.Where(x => DbFunctions.TruncateTime(x.DateEnd) <= limitDate);
            }
            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.Search))
                {
                    string search = Utils.RemoveAccent(request.Search);
                    query = query.Where(x => EdmxFunction.RemoveAccent(x.Message).Contains(search)
                            || EdmxFunction.RemoveAccent(x.BatchProgName).Contains(search)
                    );
                }
                query = this.FiltrerQuery(request.Filtres, query);
            }

            return query;
        }

        public GetAllHistoBatchsResponse GetAll()
        {
            var query = _histoBatchsRepository.FindAll();

            try
            {
                return new GetAllHistoBatchsResponse()
                {
                    HistoBatchs = query.Select(q => new HistoBatchsDTO()
                    {
                        DateStart = q.Start,
                        DateEnd = q.Finish,
                        Message = q.Message,
                        ReturnCode = q.ReturnCode
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
                    ServiceName = "HistoBatchsService",

                }, ex);
                throw ex;
            }
        }
        public override string GetOperator(Type type, ColumnFilter columnFilter, int index, string filterValue)
        {
            if ("Description".Equals(columnFilter.ColumnName) && !string.IsNullOrEmpty(filterValue))
            {
                columnFilter.Field = "EdmxFunction.RemoveAccent(Description)";
                columnFilter.FilterValue = Utils.RemoveAccent(filterValue);
            }
            return columnFilter.GetOperator(type, index, filterValue);
        }
        public int GetAllCount(GetAllHistoBatchsRequest request)
        {
            return GetAllHistoBatchsAsQueryable(request).Count();
        }

        public GetAllHistoBatchsResponse GetAllFromQueryable(GetAllHistoBatchsRequest allRequest)
        {
            string orderBy = "";
            var request = allRequest.GridRequest;
            if (string.IsNullOrEmpty(request.OrderColumn))
            {
                orderBy = "DateStart DESC";
            }
            else
            {
                orderBy = request.OrderColumn + request.SortAscDesc;
            }
            var response = new GetAllHistoBatchsResponse();
            try
            {
                IQueryable<HistoBatchsDTO> query = GetAllHistoBatchsAsQueryable(allRequest)
                    .OrderBy(orderBy).Skip((request.Page - 1) * request.Limit).Take(request.Limit);
                var list = query.ToList();
                response.HistoBatchs = list;
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetAllFromQueryable",
                    ServiceName = "HistoBatchsService",

                }, ex);
                throw ex;
            }
        }

        public GetAllHistoBatchsResponse GetAll(GetAllHistoBatchsRequest allRequest)
        {
            if (allRequest.GridRequest != null)
                return GetAllFromQueryable(allRequest);
            else
            {
                var response = GetAll();
                if (allRequest.LimitDate != null)
                {
                    response.HistoBatchs = response.HistoBatchs.Where(histoBatch => histoBatch.DateStart < allRequest.LimitDate).ToList();
                }

                return response;
            }
        }

        public DeleteHistoBatchsResponse Delete(DeleteHistoBatchsRequest request)
        {
            IEnumerable<HistoBatchs> query = null;
            if (request.LimitDate != null) query = _histoBatchsRepository.RepositoryQuery.Where(histoAction => DbFunctions.TruncateTime(histoAction.Start) <= request.LimitDate);
            else
                query = _histoBatchsRepository.RepositoryQuery;
            var response = new DeleteHistoBatchsResponse();
            try
            {
                _histoBatchsRepository.RemoveAll(query);

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Delete",
                    ServiceName = "HistoBatchsService",

                }, ex);
                throw ex;
            }
        }
    }
}
