using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using Intitek.Welcome.Domain;

namespace Intitek.Welcome.Service.Back
{
    public class ImportManagerService : BaseService, IImportManagerService
    {
        private readonly ImportManagerDataAccess _importManagerrepository;
        public ImportManagerService(ILogger logger) : base(logger)
        {
            _importManagerrepository = new ImportManagerDataAccess(uow);
        }
        public ImportManager GetImportManager(IntitekUser manager)
        {
            return _importManagerrepository.RepositoryQuery.Where(m => m.accountUser == manager.Email).FirstOrDefault();
        }
    }
}
