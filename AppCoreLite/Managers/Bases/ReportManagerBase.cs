using AppCoreLite.Configs;
using AppCoreLite.Enums;
using AppCoreLite.Models;
using AppCoreLite.Models.Bases;
using AppCoreLite.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppCoreLite.Managers.Bases
{
    /// <summary>
    /// Base report manager class for managing report operations using report models.
    /// </summary>
    public abstract class ReportManagerBase<TReport> : ExportManagerBase, IDisposable where TReport : ReportBase, new()
    {
        protected readonly DbContext _db;

        protected PageOrder? _pageOrder;
        protected List<Property>? _propertiesForOrdering;
        
        public ReportManagerConfig ReportConfig { get; }

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

        protected ReportManagerBase(DbContext db, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _db = db;
            _reflectionUtil = new ReflectionUtil();
            ReportConfig = new ReportManagerConfig();
            _pageOrder = null;
        }

        public void Set(Languages language, bool isExcelLicenseCommercial = false)
        {
            base.Set(language);
            base.Set(isExcelLicenseCommercial);
            ReportConfig.Set(language);
        }

        public abstract IQueryable<TReport> Query();

        public virtual List<TReport> GetList(PageOrder? pageOrder = null, Expression<Func<TReport, bool>>? predicate = null)
        {
            var query = Query().Where(q => (q.IsDeleted ?? false) == false);
            if (pageOrder != null)
            {
                _propertiesForOrdering = _reflectionUtil.GetProperties<TReport>(TagAttributes.Order);
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
                TotalRecordsCount = pageOrder.TotalRecordsCountResult == 0 ? ReportConfig.RecordNotFound
                    : pageOrder.TotalRecordsCountResult == 1 ? (pageOrder.TotalRecordsCountResult + " " + ReportConfig.RecordFound).ToLower()
                    : (pageOrder.TotalRecordsCountResult + " " + ReportConfig.RecordsFound).ToLower();
                RecordsPerPageCounts = ReportConfig.RecordsPerPageCounts;
                _pageOrder = pageOrder;
                if (RecordsPerPageCounts != null && RecordsPerPageCounts.Count > 0 && pageOrder.RecordsPerPageCount != RecordsPerPageCounts.LastOrDefault())
                {
                    query = query.Skip((pageOrder.PageNumber - 1) * Convert.ToInt32(pageOrder.RecordsPerPageCount))
                            .Take(Convert.ToInt32(pageOrder.RecordsPerPageCount));
                }
            }
            return query.ToList();
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
    }
}
