﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace GridMvc.Utility
{
    /// <summary>
    ///     Helper class for reflection operations
    /// </summary>
    internal static class PropertiesHelper
    {
        private const string PropertiesQueryStringDelimeter = ".";

        public static string BuildColumnNameFromMemberExpression(MemberExpression memberExpr)
        {
            var sb = new StringBuilder();
            Expression expr = memberExpr;
            while (true)
            {
                string piece = GetExpressionMemberName(expr, ref expr);
                if (string.IsNullOrEmpty(piece)) break;
                if (sb.Length > 0)
                    sb.Insert(0, PropertiesQueryStringDelimeter);
                sb.Insert(0, piece);
            }
            return sb.ToString();
        }

        private static string GetExpressionMemberName(Expression expr, ref Expression nextExpr)
        {
            if (expr is MemberExpression)
            {
                var memberExpr = (MemberExpression) expr;
                nextExpr = memberExpr.Expression;
                return memberExpr.Member.Name;
            }
            if (expr is BinaryExpression && expr.NodeType == ExpressionType.ArrayIndex)
            {
                var binaryExpr = (BinaryExpression) expr;
                string memberName = GetExpressionMemberName(binaryExpr.Left, ref nextExpr);
                if (string.IsNullOrEmpty(memberName))
                    throw new InvalidDataException("Cannot parse your column expression");
                return string.Format("{0}[{1}]", memberName, binaryExpr.Right);
            }
            return string.Empty;
        }


        public static PropertyInfo GetPropertyFromColumnName(string columnName, Type type,
                                                             out IEnumerable<PropertyInfo> propertyInfoSequence)
        {
            string[] properies = columnName.Split(new[] {PropertiesQueryStringDelimeter},
                                                  StringSplitOptions.RemoveEmptyEntries);
            if (!properies.Any())
            {
                propertyInfoSequence = null;
                return null;
            }
            PropertyInfo pi = null;
            var sequence = new List<PropertyInfo>();
            foreach (string properyName in properies)
            {
                pi = type.GetProperty(properyName);
                if (pi == null)
                {
                    propertyInfoSequence = null;
                    return null; //no match column
                }
                sequence.Add(pi);
                type = pi.PropertyType;
            }
            propertyInfoSequence = sequence;
            return pi;
        }

        public static Type GetUnderlyingType(Type type)
        {
            Type targetType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
            {
                targetType = Nullable.GetUnderlyingType(type);
            }
            else
            {
                targetType = type;
            }
            return targetType;
        }

        public static T GetAttribute<T>(this PropertyInfo pi)
        {
            return (T) pi.GetCustomAttributes(typeof (T), true).FirstOrDefault();
        }

        public static T GetAttribute<T>(this Type type)
        {
            return (T) type.GetCustomAttributes(typeof (T), true).FirstOrDefault();
        }

        public static PropertyInfo GetProperty<T, T2>(Expression<Func<T, T2>> expression)
        {
            MemberExpression memberExpression = null;

            switch (expression.Body.NodeType)
            {
                case ExpressionType.Convert:
                    memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
                    break;
                case ExpressionType.MemberAccess:
                    memberExpression = expression.Body as MemberExpression;
                    break;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("Not a member access", nameof(expression));
            }

            return memberExpression.Member as PropertyInfo;
        }

        public static string GetDisplayName<T, T2>(Expression<Func<T, T2>> titleField)
        {
            var propertyInfo = GetProperty(titleField);
            var displayAttribute = propertyInfo.GetAttribute<DisplayAttribute>();
            if (displayAttribute != null)
            {
                //ajout ResourceType 16/01/2020
                if (displayAttribute.ResourceType != null)
                {
                    PropertyInfo property = displayAttribute.ResourceType.GetProperty(displayAttribute.Name, BindingFlags.Static | BindingFlags.Public);
                    var resourceValue = (string)property.GetValue(null, null);
                    return resourceValue; 
                }
                return displayAttribute.Name;
            }

            var displayNameAttribute = propertyInfo.GetAttribute<DisplayNameAttribute>();
            if (displayNameAttribute != null)
            {
                return displayNameAttribute.DisplayName;
            }

            return null;
        }
       
    }
}