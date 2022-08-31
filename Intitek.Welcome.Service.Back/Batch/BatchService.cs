using Intitek.Welcome.DataAccess;
using Intitek.Welcome.Infrastructure.Log;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intitek.Welcome.Service.Back
{
    public class BatchService : BaseService, IBatchService
    {
        private readonly BatchsDataAccess _batchRepository;
        public HistoBatchBO HistoBatch { get; private set; }
        public BatchService(ILogger logger) : base(logger)
        {
            _batchRepository = new BatchsDataAccess(uow);
            HistoBatch = new HistoBatchBO(uow);

        }

        public GetBatchResponse Get(GetBatchRequest request)
        {
            var response = new GetBatchResponse();
            try
            {

                var batch = string.IsNullOrEmpty(request.ProgName) ? _batchRepository.FindBy(request.Id) : _batchRepository.FindAll().FirstOrDefault(p => p.ProgName == request.ProgName);

                if (batch != null)
                {
                    response.Batch = batch;
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Get",
                    ServiceName = "BatchService",

                }, ex);
                throw ex;
            }
        }

        public List<string> GetAllProgNames()
        {
            var query = _batchRepository.RepositoryTable.Select(h => h.ProgName).Distinct().OrderBy(h => h);
            var actions = query.ToList();
            return actions;
        }

        public void Historize()
        {
            try
            {
                HistoBatch.SaveHisto();
            }

            catch (Exception ex)
            {
                _logger.Error(new ExceptionLogger()
                {
                    ExceptionDateTime = DateTime.Now,
                    ExceptionStackTrack = ex.StackTrace,
                    MethodName = "Historize",
                    ServiceName = "BatchService",

                }, ex);
                throw ex;
            }

        }
    }
}
