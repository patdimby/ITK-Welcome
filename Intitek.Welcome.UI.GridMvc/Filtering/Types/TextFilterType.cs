﻿using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace GridMvc.Filtering.Types
{
    /// <summary>
    ///     Object builds filter expressions for text (string) grid columns
    /// </summary>
    internal sealed class TextFilterType : FilterTypeBase
    {
        public override Type TargetType
        {
            get { return typeof (String); }
        }

        public override GridFilterType GetValidType(GridFilterType type)
        {
            switch (type)
            {
                case GridFilterType.Equals:
                case GridFilterType.Contains:
                case GridFilterType.StartsWith:
                case GridFilterType.EndsWidth:
                    return type;
                default:
                    return GridFilterType.Equals;
            }
        }
        /// <summary>
        /// Supprimer l 'accent et convert la chaine en minuscule
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string RemoveAccent(string str)
        {
            if (null == str) return null;
            str = str.Replace("&amp;", "&").Replace("&#233;", "e").Replace("&#39;", "'");
            var chars = str
                .Normalize(NormalizationForm.FormD)
                .ToCharArray()
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();

            var ret = new string(chars).Normalize(NormalizationForm.FormC);
            return ret.ToLower();
        }
        public override object GetTypedValue(string value)
        {
            return value;
        }

        public override Expression GetFilterExpression(Expression leftExpr, string value, GridFilterType filterType)
        {
            //Custom implementation of string filter type. Case insensitive compartion.

            filterType = GetValidType(filterType);
            object typedValue = GetTypedValue(value);
            if (typedValue == null)
                return null; //incorrent filter value;

            Expression valueExpr = Expression.Constant(typedValue);
            Expression binaryExpression;
            switch (filterType)
            {
                case GridFilterType.Equals:
                    binaryExpression = GetCaseInsensitiveСompartion(string.Empty, leftExpr, valueExpr);
                    break;
                case GridFilterType.Contains:
                    binaryExpression = GetCaseInsensitiveСompartion("Contains", leftExpr, valueExpr);
                    break;
                case GridFilterType.StartsWith:
                    binaryExpression = GetCaseInsensitiveСompartion("StartsWith", leftExpr, valueExpr);
                    break;
                case GridFilterType.EndsWidth:
                    binaryExpression = GetCaseInsensitiveСompartion("EndsWith", leftExpr, valueExpr);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return binaryExpression;
        }

        /// <summary>
        /// Case insensitive et accent insensitive
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="leftExpr"></param>
        /// <param name="rightExpr"></param>
        /// <returns></returns>
        private Expression GetCaseInsensitiveСompartion(string methodName, Expression leftExpr, Expression rightExpr)
        {
            Type self = typeof(TextFilterType);
            Type targetType = TargetType;
            //case insensitive compartion:
            //MethodInfo miUpper = targetType.GetMethod("ToUpper", new Type[] { });
            //MethodCallExpression upperValueExpr = Expression.Call(rightExpr, miUpper);
            //MethodCallExpression upperFirstExpr = Expression.Call(leftExpr, miUpper);
            MethodInfo removeA = self.GetTypeInfo().GetDeclaredMethod("RemoveAccent");
            MethodCallExpression upperValueExpr = Expression.Call(removeA, rightExpr);
            MethodCallExpression upperFirstExpr = Expression.Call(removeA, leftExpr);
            if (!string.IsNullOrEmpty(methodName))
            {
                MethodInfo mi = targetType.GetMethod(methodName, new[] { typeof(string) });
                if (mi == null)
                    throw new MissingMethodException("There is no method - " + methodName);
                return Expression.Call(upperFirstExpr, mi, upperValueExpr);
            }
            return Expression.Equal(upperFirstExpr, upperValueExpr);
        }
    }
}