using System;
using System.Linq.Expressions;

namespace Intitek.Welcome.Infrastructure.Specification
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Predicate { get; }

        bool IsSatisfiedBy(T candidate);
    }
}