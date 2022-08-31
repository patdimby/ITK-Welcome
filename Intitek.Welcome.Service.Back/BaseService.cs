using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Infrastructure.Log;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using Intitek.Welcome.Infrastructure.Histo;
using System.Reflection;
using System.Data.Entity;
using Intitek.Welcome.Infrastructure.Config;
using System.Configuration;

namespace Intitek.Welcome.Service.Back
{

    public abstract class BaseService : IDisposable
    {
        protected readonly WelcomeDB _context;
        protected readonly IUnitOfWork uow;
        protected ILogger _logger;
        protected IHistorize _histoActionBO;
        protected Infrastructure.Config.Config config;

        public BaseService(ILogger logger)
        {
            var xpath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ConfigurationFilePath"]);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ConfigurationFilePath"], "config.bin");
            //var path = Path.Combine(ConfigurationManager.AppSettings["ConfigurationFilePath"], "config.bin");
            config = Infrastructure.Config.Config.Deserialize(path);
            _context = new WelcomeDB(ConfigurationEncryption.DecryptConfig(config.DBServer),
                    ConfigurationEncryption.DecryptConfig(config.DBName),
                    ConfigurationEncryption.DecryptConfig(config.DBUserID),
                    ConfigurationEncryption.DecryptConfig(config.DBPassword));
            uow = _context;
            _logger = logger;
        }

        protected Database Database
        {
            get
            {
                return _context.Database;
            }

        }

        public virtual string GetOperator(Type type, ColumnFilter columnFilter, int index, string filterValue)
        {
            return columnFilter.GetOperator(type, index, filterValue);
        }

        public List<ColumnFilter> GetColumnFilters(string[] filterColumns, List<string> onlyColumns)
        {
            List<ColumnFilter> lst = new List<ColumnFilter>();
            if (filterColumns != null && filterColumns.Count() > 0)
            {
                foreach (var queryFilter in filterColumns)
                {
                    ColumnFilter columnFilter = ColumnFilter.CreateColumnFilter(queryFilter);
                    if (columnFilter == null) continue;
                    if (onlyColumns == null)
                    {
                        lst.Add(columnFilter);
                    }
                    else if (onlyColumns != null && onlyColumns.Contains(columnFilter.ColumnName))
                    {
                        lst.Add(columnFilter);
                    }
                }
            }
            return lst;
        }

        protected IQueryable<T> FiltrerQuery<T>(List<ColumnFilter> filterColumns, IQueryable<T> query)
        {
            int index = 0;
            string where = "";
            List<object> values = new List<object>();
            if (filterColumns != null && filterColumns.Count() > 0)
            {
                foreach (var columnFilter in filterColumns)
                {
                    System.Reflection.PropertyInfo property = typeof(T).GetProperty(columnFilter.ColumnName);
                    if (columnFilter.FilterValue.Contains("|"))
                    {
                        string orWhere = "";
                        var filterValues = columnFilter.FilterValue.Split('|');
                        for (int i = 0; i < filterValues.Count(); i++)
                        {
                            if (i == 0)
                            {
                                orWhere = GetOperator(typeof(T), columnFilter, index, filterValues[i]);
                            }
                            else
                            {
                                orWhere += " OR " + GetOperator(typeof(T), columnFilter, index, filterValues[i]);
                            }
                            if (property.PropertyType == typeof(Int32) || property.PropertyType == typeof(Int32?))
                            {
                                values.Add(Int32.Parse(filterValues[i]));
                            }
                            else if (property.PropertyType == typeof(Boolean) || property.PropertyType == typeof(Boolean?))
                            {
                                values.Add(Boolean.Parse(filterValues[i]));
                            }
                            else if (typeof(System.Collections.IList).IsAssignableFrom(property.PropertyType))
                            {
                                values.Add(Int32.Parse(filterValues[i]));
                            }
                            else
                            {
                                values.Add(filterValues[i]);
                            }
                            index++;
                        }
                        if (!string.IsNullOrEmpty(where))
                            where += " AND (" + orWhere + ")";
                        else
                            where = "(" + orWhere + ")";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(where))
                            where += " AND " + GetOperator(typeof(T), columnFilter, index, columnFilter.FilterValue);
                        else
                            where = GetOperator(typeof(T), columnFilter, index, columnFilter.FilterValue);
                        if (property.PropertyType == typeof(Int32) || property.PropertyType == typeof(Int32?))
                        {
                            values.Add(Int32.Parse(columnFilter.FilterValue));
                        }
                        else if (property.PropertyType == typeof(Boolean) || property.PropertyType == typeof(Boolean?))
                        {
                            values.Add(Boolean.Parse(columnFilter.FilterValue));
                        }
                        else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                        {
                            values.Add(DateTime.Parse(columnFilter.FilterValue));
                        }
                        else if (typeof(System.Collections.IList).IsAssignableFrom(property.PropertyType))
                        {
                            values.Add(Int32.Parse(columnFilter.FilterValue));
                        }
                        else
                        {
                            values.Add(columnFilter.FilterValue);
                        }
                        index++;
                    }

                }
                if (!string.IsNullOrEmpty(where))
                    query = query.Where(where, values.ToArray());
            }
            return query;
        }
        protected IQueryable<T> FiltrerQuery<T>(string[] filterColumns, IQueryable<T> query)
        {
            List<ColumnFilter> columnFilters = this.GetColumnFilters(filterColumns, null);
            query = this.FiltrerQuery(columnFilters, query);
            return query;
        }
        /// <summary>
        /// path pour la classe System.Link.Dynamic pour reconnaitre la classe EdmxFunction
        /// </summary>
        protected void DynamicLinkPatch()
        {
            //path for System.Link.Dynamic pour reconnaitre la classe EdmxFunction
            var type = typeof(DynamicQueryable).Assembly.GetType("System.Linq.Dynamic.ExpressionParser");
            FieldInfo field = type.GetField("predefinedTypes", BindingFlags.Static | BindingFlags.NonPublic);
            Type[] predefinedTypes = (Type[])field.GetValue(null);
            if (!predefinedTypes.Contains(typeof(EdmxFunction)))
            {
                var length = predefinedTypes.Length + 2;
                Array.Resize(ref predefinedTypes, length);
                predefinedTypes[length - 2] = typeof(EdmxFunction);
                predefinedTypes[length - 1] = typeof(DbFunctions);
                field.SetValue(null, predefinedTypes);
            }
        }

        protected bool HasFiltre(GridMvcRequest request, string columnName)
        {
            return request.Filtres != null && request.Filtres.Select(x => x).Where(x => x.StartsWith(columnName)).Any();

        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
