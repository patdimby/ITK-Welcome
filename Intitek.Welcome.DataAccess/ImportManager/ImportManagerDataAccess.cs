using System;
using System.Collections.Generic;
using System.Linq;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace Intitek.Welcome.DataAccess
{
    public class ImportManagerDataAccess : Repository<ImportManager, int>, IImportManagerDataAccess
    {
        public ImportManagerDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
