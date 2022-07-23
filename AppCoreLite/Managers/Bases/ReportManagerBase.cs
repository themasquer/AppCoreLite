using AppCoreLite.Configs;
using AppCoreLite.Configs.Bases;
using AppCoreLite.Enums;
using AppCoreLite.Models;
using AppCoreLite.Models.Bases;
using AppCoreLite.Results;
using AppCoreLite.Results.Bases;
using AppCoreLite.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Linq.Expressions;

namespace AppCoreLite.Managers.Bases
{
    /// <summary>
    /// Base report manager class for managing report operations using models.
    /// </summary>
    public abstract class ReportManagerBase<TReport> : IDisposable, IConfig where TReport : ReportBase, new()
    {
        protected readonly DbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected ReflectionUtility _reflectionUtil;
        protected PageOrder? _pageOrder;
        protected List<Property>? _propertiesForOrdering;
        protected List<Property>? _propertiesForReporting;

        public ReportManagerConfig Config { get; }
        public List<int> PageNumbers
        {
            get
            {
                var pageNumbers = new List<int>();
                if (_pageOrder != null)
                {
                    if (_pageOrder.TotalRecordsCountResult == 0 || RecordsPerPageCounts != null && RecordsPerPageCounts.Count > 0 && _pageOrder.RecordsPerPageCount == RecordsPerPageCounts.LastOrDefault())
                    {
                        pageNumbers.Add(1);
                    }
                    else
                    {
                        int numberOfPages = Convert.ToInt32(Math.Ceiling(_pageOrder.TotalRecordsCountResult / Convert.ToDecimal(_pageOrder.RecordsPerPageCount)));
                        for (int page = 1; page <= numberOfPages; page++)
                        {
                            pageNumbers.Add(page);
                        }
                    }
                }
                return pageNumbers;
            }
        }
        public List<string>? RecordsPerPageCounts { get; set; }
        public List<string>? OrderExpressions => _propertiesForOrdering?.Select(pm => !string.IsNullOrWhiteSpace(pm.DisplayName) ? pm.DisplayName : pm.Name).ToList();
        public string? TotalRecordsCount { get; set; }

        protected ReportManagerBase(DbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _reflectionUtil = new ReflectionUtility();
            Config = new ReportManagerConfig();
            _pageOrder = null;
            _propertiesForOrdering = _reflectionUtil.GetProperties<TReport>(TagAttributes.Order);
        }

        public void Set(Languages language)
        {
            Config.Set(language);
        }

        public void Set(bool isExcelLicenseCommercial)
        {
            Config.Set(isExcelLicenseCommercial);
        }

        public abstract IQueryable<TReport> Query();

        public virtual List<TReport> GetList(PageOrder pageOrder, Expression<Func<TReport, bool>>? predicate = null)
        {
            var query = Query().Where(q => (q.IsDeleted ?? false) == false);
            if (pageOrder != null)
            {
                var propertyForOrdering = GetOrderingProperty(pageOrder.OrderExpression);
                if (propertyForOrdering != null)
                {
                    query = pageOrder.IsOrderDirectionAscending ? query.OrderBy(_reflectionUtil.GetOrderExpression<TReport>(propertyForOrdering.Name))
                            : query.OrderByDescending(_reflectionUtil.GetOrderExpression<TReport>(propertyForOrdering.Name));
                }
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (pageOrder != null)
            {
                pageOrder.TotalRecordsCountResult = query.Count();
                TotalRecordsCount = pageOrder.TotalRecordsCountResult == 0 ? Config.RecordNotFound
                    : pageOrder.TotalRecordsCountResult == 1 ? (pageOrder.TotalRecordsCountResult + " " + Config.RecordFound).ToLower()
                    : (pageOrder.TotalRecordsCountResult + " " + Config.RecordsFound).ToLower();
                RecordsPerPageCounts = Config.RecordsPerPageCounts;
                _pageOrder = pageOrder;
                if (RecordsPerPageCounts != null && RecordsPerPageCounts.Count > 0 && pageOrder.RecordsPerPageCount != RecordsPerPageCounts.LastOrDefault())
                {
                    query = query.Skip((pageOrder.PageNumber - 1) * Convert.ToInt32(pageOrder.RecordsPerPageCount))
                            .Take(Convert.ToInt32(pageOrder.RecordsPerPageCount));
                }
            }
            return query.ToList();
        }

        public virtual void ExportToExcel<TModel>(List<TModel> list) where TModel : class, new()
        {
            var data = ConvertToByteArrayForExcel(list);
            if (data != null && data.Length > 0)
            {
                _httpContextAccessor.HttpContext.Response.Headers.Clear();
                _httpContextAccessor.HttpContext.Response.Clear();
                _httpContextAccessor.HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                _httpContextAccessor.HttpContext.Response.Headers.Add("content-length", data.Length.ToString());
                _httpContextAccessor.HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=\"" + Config.FileNameWithoutExtension + ".xlsx\"");
                _httpContextAccessor.HttpContext.Response.Body.WriteAsync(data, 0, data.Length);
                _httpContextAccessor.HttpContext.Response.Body.Flush();
            }
        }

        public void Dispose()
        {
            _db?.Dispose();
            GC.SuppressFinalize(this);
        }

        private Property? GetOrderingProperty(string orderExpression)
        {
            Property? propertyForOrdering = _propertiesForOrdering?.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(orderExpression))
            {
                propertyForOrdering = _propertiesForOrdering?.FirstOrDefault(pm => pm.DisplayName == orderExpression);
                if (propertyForOrdering == null)
                    propertyForOrdering = _propertiesForOrdering?.FirstOrDefault(pm => pm.Name == orderExpression);
            }
            return propertyForOrdering;
        }

        private byte[]? ConvertToByteArrayForExcel<TModel>(List<TModel> list) where TModel : class, new()
        {
            byte[]? data = null;
            if (list != null && list.Count > 0)
            {
                var dataTable = _reflectionUtil.ConvertToDataTable(list);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    ExcelPackage.LicenseContext = Config.IsExcelLicenseCommercial ? LicenseContext.Commercial : LicenseContext.NonCommercial;
                    ExcelPackage excelPackage = new ExcelPackage();
                    ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add(Config.ExcelWorksheetName);
                    excelWorksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                    excelWorksheet.Cells["A:AZ"].AutoFitColumns();
                    data = excelPackage.GetAsByteArray();
                }
            }
            return data;
        }
    }
}
