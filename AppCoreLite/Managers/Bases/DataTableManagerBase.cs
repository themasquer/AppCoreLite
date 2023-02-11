#nullable disable
using AppCoreLite.Configs.Bases;
using AppCoreLite.Enums;
using AppCoreLite.Models;
using AppCoreLite.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppCoreLite.Managers.Bases
{
    // Reference: https://github.com/DavidSuescunPelegay/jQuery-datatable-server-side-net-core
    public abstract class DataTableManagerBase : IConfig
    {
        private readonly ReflectionUtil _reflectionUtility;
        private Languages _language;

        protected DataTableManagerBase()
        {
            _reflectionUtility = new ReflectionUtil();
            _language = Languages.Turkish;
        }

        public void Set(Languages language)
        {
            _language = language;
        }

        public virtual async Task<DtResult<T>> Bind<T>(DtParameters dtParameters, IQueryable<T> query,
            Expression<Func<T, bool>> predicate = null, string orderExpressionValueSuffix = "Display")
        where T : class, new()
        {
            string orderExpression = null;
            bool orderDirectionAscending = true;
            if (dtParameters.Order != null)
            {
                orderExpression = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderDirectionAscending = dtParameters.Order[0].Dir == DtOrderDir.Asc;
            }
            int recordsCount = await query.CountAsync();
            IQueryable<T> orderedQuery = _reflectionUtility.OrderQuery(query, orderDirectionAscending, orderExpression, orderExpressionValueSuffix);
            IQueryable<T> filteredQuery;
            if (predicate == null)
                filteredQuery = orderedQuery;
            else
                filteredQuery = orderedQuery.Where(predicate);
            int filteredRecordsCount = await filteredQuery.CountAsync();
            var dataTable = new DtResult<T>()
            {
                Draw = dtParameters.Draw,
                RecordsTotal = recordsCount,
                RecordsFiltered = filteredRecordsCount,
                Data = dtParameters.Length != -1 ? await filteredQuery.Skip(dtParameters.Start).Take(dtParameters.Length).ToListAsync() : await filteredQuery.ToListAsync()
            };
            return dataTable;
        }

        public virtual DtResult<T> Bind<T>(DtParameters dtParameters, List<T> list,
            Expression<Func<T, bool>> predicate = null, string orderExpressionValueSuffix = "Display")
        where T : class, new()
        {
            string orderExpression = null;
            bool orderDirectionAscending = true;
            if (dtParameters.Order != null)
            {
                orderExpression = dtParameters.Columns[dtParameters.Order[0].Column].Data;
                orderDirectionAscending = dtParameters.Order[0].Dir == DtOrderDir.Asc;
            }
            int recordsCount = list.Count;
            IQueryable<T> orderedQuery = _reflectionUtility.OrderQuery(list.AsQueryable(), orderDirectionAscending, orderExpression, orderExpressionValueSuffix);
            IQueryable<T> filteredQuery;
            if (predicate == null)
                filteredQuery = orderedQuery;
            else
                filteredQuery = orderedQuery.Where(predicate);
            int filteredRecordsCount = filteredQuery.Count();
            var dataTable = new DtResult<T>()
            {
                Draw = dtParameters.Draw,
                RecordsTotal = recordsCount,
                RecordsFiltered = filteredRecordsCount,
                Data = dtParameters.Length != -1 ? filteredQuery.Skip(dtParameters.Start).Take(dtParameters.Length).ToList() : filteredQuery.ToList()
            };
            return dataTable;
        }

        public virtual void AddOperations<T>(DtResult<T> dataTable, DtOperationsModel dataTableOperations, bool deleteLinkCallingJavascriptFunction = false, byte numberOfSpacesBetweenLinks = 1, char linksSeperator = '|')
        where T : class, new()
        {
            dataTableOperations.Set(_language);
            if (string.IsNullOrWhiteSpace(dataTableOperations.DetailsUrl) && string.IsNullOrWhiteSpace(dataTableOperations.EditUrl) && string.IsNullOrWhiteSpace(dataTableOperations.DeleteUrl))
                return;
            string linksValue;
            string seperator = "&nbsp;" + linksSeperator + "&nbsp;";
            string seperators = "";
            for (int i = 1; i <= numberOfSpacesBetweenLinks; i++)
            {
                seperators += seperator;
            }
            foreach (T data in dataTable.Data)
            {
                var linksProperty = _reflectionUtility.GetPropertyInfo<T>(dataTableOperations.OperationLinksProperty);
                var keyProperty = _reflectionUtility.GetPropertyInfo<T>(dataTableOperations.OperationKeyProperty);
                if (linksProperty == null || keyProperty == null)
                    break;
                linksValue = "";
                if (!string.IsNullOrWhiteSpace(dataTableOperations.DetailsUrl))
                    linksValue += "<a class='" + dataTableOperations.DetailsCss + "' " +
                        "href='" + dataTableOperations.DetailsUrl + (dataTableOperations.DetailsUrl.Contains("?") ? "&" : "?") +
                        dataTableOperations.OperationKeyProperty + "=" + keyProperty.GetValue(data) + "' " +
                        "title='" + dataTableOperations.DetailsTitle + "'>" +
                        dataTableOperations.DetailsText + "</a>";
                if (!string.IsNullOrWhiteSpace(dataTableOperations.EditUrl))
                    linksValue += seperators + "<a class='" + dataTableOperations.EditCss + "' " +
                        "href='" + dataTableOperations.EditUrl + (dataTableOperations.EditUrl.Contains("?") ? "&" : "?") +
                        dataTableOperations.OperationKeyProperty + "=" + keyProperty.GetValue(data) + "' " +
                        "title='" + dataTableOperations.EditTitle + "'>" +
                        dataTableOperations.EditText + "</a>";
                if (!string.IsNullOrWhiteSpace(dataTableOperations.DeleteUrl))
                {
                    if (!deleteLinkCallingJavascriptFunction)
                    {
                        linksValue += seperators + "<a class='" + dataTableOperations.DeleteCss + "' " +
                            "href='" + dataTableOperations.DeleteUrl + (dataTableOperations.DeleteUrl.Contains("?") ? "&" : "?") +
                            dataTableOperations.OperationKeyProperty + "=" + keyProperty.GetValue(data) + "' " +
                            "title='" + dataTableOperations.DeleteTitle + "'>" +
                            dataTableOperations.DeleteText + "</a>";
                    }
                    else
                    {
                        linksValue += seperators + "<a href='#' class='deleteItem " + dataTableOperations.DeleteCss + "' " +
                            "onclick='deleteItem(" + keyProperty.GetValue(data) + ");' " +
                            "title='" + dataTableOperations.DeleteTitle + "'>" +
                            dataTableOperations.DeleteText + "</a>";
                    }
                }
                linksProperty.SetValue(data, linksValue.Trim(seperator.ToCharArray()));
            }
        }
    }
}
