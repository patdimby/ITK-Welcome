using System;
using System.Linq.Expressions;

namespace Intitek.Welcome.Infrastructure.Specification
{
    public class Specification<T> : AbstractSpecification<T>
    {
        private readonly Expression<Func<T, bool>> _predicate;

        public Specification(Expression<Func<T, bool>> predicate)
        {
            _predicate = predicate;
        }

        public override Expression<Func<T, bool>> Predicate
        {
            get { return _predicate; }
        }

        public override bool IsSatisfiedBy(T candidate)
        {
            return _predicate.Compile().Invoke(candidate);
        }

        public static Specification<T> operator &(Specification<T> leftSide, Specification<T> rightSide)
        {
            InvocationExpression rightInvoke = Expression.Invoke(rightSide.Predicate, leftSide.Predicate.Parameters);
            BinaryExpression newExpression = Expression.MakeBinary(ExpressionType.AndAlso, leftSide.Predicate.Body,
                                                                   rightInvoke);
            return new Specification<T>(Expression.Lambda<Func<T, bool>>(newExpression, leftSide.Predicate.Parameters));
        }

        public static Specification<T> operator |(Specification<T> leftSide, Specification<T> rightSide)
        {
            InvocationExpression rightInvoke = Expression.Invoke(rightSide.Predicate, leftSide.Predicate.Parameters);
            BinaryExpression newExpression = Expression.MakeBinary(ExpressionType.OrElse, leftSide.Predicate.Body,
                                                                   rightInvoke);
            return new Specification<T>(Expression.Lambda<Func<T, bool>>(newExpression, leftSide.Predicate.Parameters));
        }
    }
}