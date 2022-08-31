using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Intitek.Welcome.Infrastructure.Domain
{
    public class SortHelper<T>
    {
        
            public static Func<T, object> Criteria(string propertyname)
            {
                var param = Expression.Parameter(typeof(T), "x");
                Expression body = param;
                foreach (var member in propertyname.Split('.'))
                {
                    body = Expression.PropertyOrField(body, member.Trim());
                }
                return Expression.Lambda<Func<T, object>>(body, param).Compile();
            }

            public static LambdaExpression CriteriaLambda(string propertyname)
            {
                var param = Expression.Parameter(typeof(T), "x");
                Expression body = param;
                foreach (var member in propertyname.Split('.'))
                {
                    body = Expression.PropertyOrField(body, member.Trim());
                }
                
                return Expression.Lambda<Func<T, object>>(Expression.Convert(body, typeof(object)), param);
            }

            public static List<T> Sort(IEnumerable<T> items, string[] propertynames, int sortdirection)
            {
                if (items == null) return new List<T>();
                if (propertynames == null || !propertynames.Any()) return items.ToList();
                try
                {
                    var queriableData = items.AsQueryable();
                    //Get value of proprety "Expression" of IQueryable
                    var queryableExpression =
                        (Expression)typeof(IQueryable).GetProperty("Expression").GetValue(queriableData, null);
                    BlockExpression block = null;
                    var allmethods = new List<Expression>();
                    LambdaExpression criteria;
                    switch (sortdirection)
                    {
                        case 1: // croissant
                            criteria = CriteriaLambda(propertynames[0].Trim());
                            var orderByCallExpression = Expression.Call(typeof(Queryable),
                                                                        "OrderBy",
                                                                        new[] { typeof(T), typeof(object) },
                                                                        queryableExpression,
                                                                        criteria);
                            var thenByCallExpression = orderByCallExpression;
                            allmethods.Add(thenByCallExpression);
                            for (var i = 1; i < propertynames.Length; i++)
                            {
                                thenByCallExpression = Expression.Call(typeof(Queryable),
                                                                       "ThenBy",
                                                                       new[] { typeof(T), typeof(object) },
                                                                       thenByCallExpression,
                                                                       CriteriaLambda(propertynames[i].Trim()));
                                allmethods.Add(thenByCallExpression);
                            }

                            // Creating a block expression that combines two method calls.
                            block = Expression.Block(allmethods);
                            break;

                        case -1: //decroissant
                            allmethods = new List<Expression>();
                            criteria = CriteriaLambda(propertynames[0].Trim());
                            var orderByDescendingCallExpression = Expression.Call(typeof(Queryable),
                                                                                  "OrderByDescending",
                                                                                  new[] { typeof(T), typeof(object) },
                                                                                  queryableExpression,
                                                                                  criteria);
                            var thenByDescendingCallExpression = orderByDescendingCallExpression;
                            allmethods.Add(thenByDescendingCallExpression);
                            for (var i = 1; i < propertynames.Length; i++)
                            {
                                thenByDescendingCallExpression = Expression.Call(typeof(Queryable),
                                                                                 "ThenByDescending",
                                                                                 new[] { typeof(T), typeof(object) },
                                                                                 thenByDescendingCallExpression,
                                                                                 CriteriaLambda(propertynames[i].Trim()));
                                allmethods.Add(thenByDescendingCallExpression);
                            }

                            // Creating a block expression that combines two method calls.
                            block = Expression.Block(allmethods);
                            break;
                    }

                    //Get value of proprety "Provider" of IQueryable
                    var provider =
                        (IQueryProvider)typeof(IQueryable).GetProperty("Provider").GetValue(queriableData, null);

                    var result = block != null ? provider.CreateQuery(block.Result).AsQueryable() : queriableData;

                    return Enumerable.Cast<T>(result).ToList();
                }
                catch
                {
                    return items.ToList();
                }
            }

            public static List<T> Sort(IEnumerable<T> items, string[] propertynames, SortDirection sortdirection)
            {
                if (items == null) return new List<T>();
                if (propertynames == null || !propertynames.Any()) return items.ToList();
                try
                {
                    var queriableData = items.AsQueryable();
                    //Get value of proprety "Expression" of IQueryable
                    var queryableExpression =
                        (Expression)typeof(IQueryable).GetProperty("Expression").GetValue(queriableData, null);
                    BlockExpression block = null;
                    var allmethods = new List<Expression>();
                    LambdaExpression criteria;
                    switch (sortdirection)
                    {
                        case SortDirection.Ascending:

                            criteria = CriteriaLambda(propertynames[0].Trim());
                            var orderByCallExpression = Expression.Call(typeof(Queryable),
                                                                        "OrderBy",
                                                                        new[] { typeof(T), typeof(object) },
                                                                        queryableExpression,
                                                                        criteria);
                            var thenByCallExpression = orderByCallExpression;
                            allmethods.Add(thenByCallExpression);
                            for (var i = 1; i < propertynames.Length; i++)
                            {
                                thenByCallExpression = Expression.Call(typeof(Queryable),
                                                                       "ThenBy",
                                                                       new[] { typeof(T), typeof(object) },
                                                                       thenByCallExpression,
                                                                       CriteriaLambda(propertynames[i].Trim()));
                                allmethods.Add(thenByCallExpression);
                            }

                            // Creating a block expression that combines two method calls.
                            block = Expression.Block(allmethods);

                            break;
                        case SortDirection.Descending: //decroissant
                            allmethods = new List<Expression>();
                            criteria = CriteriaLambda(propertynames[0].Trim());
                            var orderByDescendingCallExpression = Expression.Call(typeof(Queryable),
                                                                                  "OrderByDescending",
                                                                                  new[] { typeof(T), typeof(object) },
                                                                                  queryableExpression,
                                                                                  criteria);
                            var thenByDescendingCallExpression = orderByDescendingCallExpression;
                            allmethods.Add(thenByDescendingCallExpression);
                            for (var i = 1; i < propertynames.Length; i++)
                            {
                                thenByDescendingCallExpression = Expression.Call(typeof(Queryable),
                                                                                 "ThenByDescending",
                                                                                 new[] { typeof(T), typeof(object) },
                                                                                 thenByDescendingCallExpression,
                                                                                 CriteriaLambda(propertynames[i].Trim()));
                                allmethods.Add(thenByDescendingCallExpression);
                            }

                            // Creating a block expression that combines two method calls.
                            block = Expression.Block(allmethods);
                            break;
                    }

                    //Get value of proprety "Provider" of IQueryable
                    var provider =
                        (IQueryProvider)typeof(IQueryable).GetProperty("Provider").GetValue(queriableData, null);

                    var result = block != null ? provider.CreateQuery(block.Result).AsQueryable() : queriableData;

                    return Enumerable.Cast<T>(result).ToList();
                }
                catch
                {
                    return items.ToList();
                }
            }
        } 
    
    public enum SortDirection
    {
        Ascending = 1,
        Descending = -1
    }
}