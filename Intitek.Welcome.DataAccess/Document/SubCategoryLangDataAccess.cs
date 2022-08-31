using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;

namespace Intitek.Welcome.DataAccess
{
    public class SubCategoryLangDataAccess : Repository<SubCategoryLang, int>, ISubCategoryLangDataAccess
    {
        public SubCategoryLangDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
    }
}
