using System;
using System.Linq.Expressions;

namespace Intitek.Welcome.Infrastructure.Specification
{
    public abstract class AbstractSpecification<T> : ISpecification<T>
    {
        public abstract Expression<Func<T, bool>> Predicate { get; }

        public abstract bool IsSatisfiedBy(T candidate);
    }
}