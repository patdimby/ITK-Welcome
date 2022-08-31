using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Helpers;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Infrastructure.Specification;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace Intitek.Welcome.Service.Back
{
    public class HistoUserQcmDocVersionService : BaseService, IHistoUserQcmDocVersionService
    {
        private readonly HistoUserQcmDocVersionDataAccess _histoUserQcmRepository;
        private readonly UserDataAccess _userRepository;
        private readonly QcmDataAccess _qcmRepository;
        private readonly QcmLangDataAccess _qcmlangRepository;
        private readonly DocumentDataAccess _docRepository;
        private readonly DocumentLangDataAccess _doclangRepository;

        public HistoUserQcmDocVersionService(ILogger logger) : base(logger)
        {
            _histoUserQcmRepository = new HistoUserQcmDocVersionDataAccess(uow);
            _userRepository = new UserDataAccess(uow);
            _qcmRepository = new QcmDataAccess(uow);
            _qcmlangRepository = new QcmLangDataAccess(uow);
            _docRepository = new DocumentDataAccess(uow);
            _doclangRepository = new DocumentLangDataAccess(uow);
            //path for System.Link.Dynamic pour reconnaitre la classe EdmxFunction
            this.DynamicLinkPatch();
        }

        public override string GetOperator(Type type, ColumnFilter columnFilter, int index, string filterValue)
        {
            if ("DocName".Equals(columnFilter.ColumnName) && !string.IsNullOrEmpty(filterValue))
            {
                columnFilter.Field = "EdmxFunction.RemoveAccent(DocName)";
                columnFilter.FilterValue = Utils.RemoveAccent(filterValue);
            }
            else if ("QcmName".Equals(columnFilter.ColumnName) && !string.IsNullOrEmpty(filterValue))
            {
                columnFilter.Field = "EdmxFunction.RemoveAccent(QcmName)";
                columnFilter.FilterValue = Utils.RemoveAccent(filterValue);
            }
            return columnFilter.GetOperator(type, index, filterValue);
        }

        private IQueryable<HistoUserQcmDocVersionDTO> GetAllHistoUserQcmDocVersionAsQueryable(GetAllHistoUserQcmDocVersionRequest allRequest)
        {
            var request = allRequest.GridRequest;
            var limitDate = allRequest.LimitDate;
            var query = this._histoUserQcmRepository.RepositoryQuery.Join(this._userRepository.RepositoryTable,
                   histoUserQcm => histoUserQcm.ID_IntitekUser,
                   user => user.ID,
                   (histoUserQcm, user) => new { histoUserQcm, user = user })
               .Join(this._qcmRepository.RepositoryTable,
                    obj => obj.histoUserQcm.ID_Qcm,
                    qcm => qcm.Id,
                    (obj, qcm) => new { histoUserQcm = obj.histoUserQcm, user = obj.user, qcm = qcm })
              .Select(parent => new HistoUserQcmDocVersionDTO()
              {
                  ID = parent.histoUserQcm.ID,
                  ID_Document = parent.histoUserQcm.ID_Document,
                  DocName = EdmxFunction.GetNameDocument(parent.histoUserQcm.ID_Document, allRequest.IdLang, allRequest.IdDefaultLang),
                  ID_Qcm = parent.histoUserQcm.ID_Qcm,
                  DateCreation = parent.qcm.DateCreation,
                  DateAction = parent.histoUserQcm.DateFin,
                  Username = parent.user.Username,
                  Version = parent.histoUserQcm.Version,
                  Score = parent.histoUserQcm.Score,
                  ScoreMinimal = parent.histoUserQcm.ScoreMinimal
              });
            if (limitDate != null)
            {
                query = query.Where(x => DbFunctions.TruncateTime(x.DateAction) <= limitDate);
            }
            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.Search))
                {
                    string search = Utils.RemoveAccent(request.Search);
                    query = query.Where(x => EdmxFunction.RemoveAccent(x.Username).Contains(search)
                            || EdmxFunction.RemoveAccent(x.DocName).Contains(search)
                    );
                }
                bool hasFiltreQcm = this.HasFiltre(request, "QcmName");
                if (hasFiltreQcm)
                {
                    List<ColumnFilter> columnsFiltreQcm = this.GetColumnFilters(request.Filtres, new List<string>() { "QcmName" });
                    var iqQcms = this.FiltrerQuery(columnsFiltreQcm, _qcmlangRepository.RepositoryQuery).Select(x=> x.ID_Qcm);
                    query = query.Where(x => iqQcms.Contains(x.ID_Qcm));
                }
                List<ColumnFilter> columnsFilters = this.GetColumnFilters(request.Filtres, null);
                columnsFilters.Remove(new ColumnFilter() { ColumnName= "QcmName" });
                query = this.FiltrerQuery(columnsFilters, query);
            }
            return query;
        }

       
        public int GetAllHistoUserQcmDocVersionCount(GetAllHistoUserQcmDocVersionRequest request)
        {
            return GetAllHistoUserQcmDocVersionAsQueryable(request).Count();

        }

        public GetAllHistoUserQcmDocVersionResponse GetAllFromQueryable(GetAllHistoUserQcmDocVersionRequest allRequest)
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
            var response = new GetAllHistoUserQcmDocVersionResponse();
            try
            {
                IQueryable<HistoUserQcmDocVersionDTO> query = GetAllHistoUserQcmDocVersionAsQueryable(allRequest)
                    .OrderBy(orderBy).Skip((request.Page - 1) * request.Limit).Take(request.Limit);
                var list = query.ToList();
                foreach (var item in list)
                {
                    var qcmlang = _qcmlangRepository.FindBy(new Specification<QcmLang>(d => d.ID_Qcm == item.ID_Qcm && d.ID_Lang == allRequest.IdLang)).FirstOrDefault();
                    if (qcmlang == null)
                    {
                        qcmlang = _qcmlangRepository.FindBy(new Specification<QcmLang>(d => d.ID_Qcm == item.ID_Qcm && d.ID_Lang == allRequest.IdDefaultLang)).FirstOrDefault();
                    }
                    item.QcmName = qcmlang.QcmName; 
                }
                response.HistoUserQcms = list;
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "GetAllHistoUserQcmDocFromQueryable",
                    ServiceName = "QcmService",

                }, ex);
                throw ex;
            }
        }

        public GetAllHistoUserQcmDocVersionResponse GetAllHistoUserQcmDocVersion(GetAllHistoUserQcmDocVersionRequest allRequest)
        {
            var response = new GetAllHistoUserQcmDocVersionResponse();
            response = GetAllFromQueryable(allRequest);             
            return response;
        }

        public DeleteHistoUserQcmDocVersionResponse Delete(DeleteHistoUserQcmDocVersionRequest request)
        {
            IEnumerable<HistoUserQcmDocVersion> query = null;
            if (request.LimitDate != null) query = _histoUserQcmRepository.RepositoryQuery.Where(histoAction => DbFunctions.TruncateTime(histoAction.DateCre) <= request.LimitDate);
            else
                query = _histoUserQcmRepository.RepositoryQuery;

            var response = new DeleteHistoUserQcmDocVersionResponse();
            try
            {
                _histoUserQcmRepository.RemoveAll(query);

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Delete",
                    ServiceName = "HistoUserQcmDocVersionService",

                }, ex);
                throw ex;
            }
        }
    }
}
