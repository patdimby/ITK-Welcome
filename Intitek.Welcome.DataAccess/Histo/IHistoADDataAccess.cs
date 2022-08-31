using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Domain;
using System.Collections.Generic;

namespace Intitek.Welcome.DataAccess
{
    public interface IHistoADDataAccess : IRepository<Histo_AD, int>
    {
        IEnumerable<HistoADMonthDTO> GetMonths();
    }
}
