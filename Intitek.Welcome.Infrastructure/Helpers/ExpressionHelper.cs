using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Intitek.Welcome.Infrastructure.Helpers
{
    public static class ExpressionHelper
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string field, string defaultField, int dir = 1, bool isFirst=true)
        {
            if (isFirst && string.IsNullOrEmpty(field))
            {
                field = defaultField;
            }
            Type type = source.ElementType;
            var parameter = Expression.Parameter(type, "r");
            var expressionBody = Expression.Property(parameter, field);
            var lambda = Expression.Lambda(expressionBody, parameter); // r => r.Field
            var propertyType = type.GetProperty(field).PropertyType;
            var nome = "";
            if (isFirst)
            {
                nome = "OrderBy";
                if (dir == 0)
                {
                    nome = "OrderByDescending";
                }
            }
            else
            {
                nome = "ThenBy";
                if (dir == 0)
                {
                    nome = "ThenByDescending";
                }
            }
            var method = typeof(Queryable).GetMethods().First(m => m.Name == nome && m.GetParameters().Length == 2);
            var metthodGeneric = method.MakeGenericMethod(new[] { type, propertyType });
            return metthodGeneric.Invoke(source, new object[] { source, lambda }) as IOrderedQueryable<T>;

        }


    }
}
