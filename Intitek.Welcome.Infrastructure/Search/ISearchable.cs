using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Infrastructure.Search
{
    public interface ISearchable<T> where T: class
    {
        Expression<Func<T,bool>> Where(QuerySearch<T> querySearch);

    }
}
