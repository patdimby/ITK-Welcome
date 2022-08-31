using System.Collections.Generic;
using Intitek.Welcome.Infrastructure.Specification;
namespace Intitek.Welcome.Infrastructure.Domain
{
    public interface IReadOnlyRepository<T, TId> where T : IAggregateRoot
    {
        IEnumerable<T> FindAll();
        IEnumerable<T> FindAll(string orderBy, int index, int count);
       
        T FindBy(TId id);
        IEnumerable<T> FindBy(ISpecification<T> specification);
        IEnumerable<T> FindBy(ISpecification<T> specification, string orderBy, int index, int count);
    }
}