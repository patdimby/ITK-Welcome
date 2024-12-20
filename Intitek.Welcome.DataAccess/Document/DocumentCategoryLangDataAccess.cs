﻿using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccess
{
    public class DocumentCategoryLangDataAccess : Repository<DocumentCategoryLang, int>, IDocumentCategoryLangDataAccess
    {
        public DocumentCategoryLangDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
