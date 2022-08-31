using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using Intitek.Welcome.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Intitek.Welcome.DataAccess
{
    public class HistoADDataAccess : Repository<Histo_AD, int>, IHistoADDataAccess
    {
        public HistoADDataAccess(IUnitOfWork uow) : base(uow)
        {

        }
        public IEnumerable<HistoADMonthDTO> GetMonths()
        {
            return this.Context.Database.SqlQuery<HistoADMonthDTO>("SELECT DISTINCT Mois FROM Histo_AD ORDER BY 1 DESC").ToList();
        }
    }
}
