using AppCoreLite.Managers.Bases;
using DataAccessDemo.Contexts;
using DataAccessDemo.Entities;
using DataAccessDemo.Models;
using Microsoft.AspNetCore.Http;

namespace DataAccessDemo.Managers
{
    public abstract class ProductReportManagerBase : ReportManagerBase<ProductReport>
    {
        protected ProductReportManagerBase(Db db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
        }
    }

    public class ProductReportManager : ProductReportManagerBase
    {
        public ProductReportManager(Db db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
        }

        public override IQueryable<ProductReport> Query()
        {
            var query = from p in _db.Set<Product>()
                        join c in _db.Set<Category>()
                        on p.CategoryId equals c.Id into categories
                        from category in categories.DefaultIfEmpty()
                        join ps in _db.Set<ProductStore>()
                        on p.Id equals ps.ProductId into productStores
                        from productStore in productStores.DefaultIfEmpty()
                        join s in _db.Set<Store>()
                        on productStore.StoreId equals s.Id into stores
                        from store in stores.DefaultIfEmpty()
                        select new ProductReport()
                        {
                            CategoryDescription = category.Description,
                            CategoryId = category.Id,
                            CategoryName = category.Name,
                            ExpirationDate = p.ExpirationDate,
                            ExpirationDateDisplay = p.ExpirationDate.HasValue ? p.ExpirationDate.Value.ToString("MM/dd/yyyy") : "",
                            ProductDescription = p.Description,
                            ProductName = p.Name,
                            StockAmount = p.StockAmount,
                            StoreId = store.Id,
                            StoreName = store.Name + (store.IsVirtual ? " (Virtual)" : ""),
                            UnitPrice = p.UnitPrice,
                            UnitPriceDisplay = p.UnitPrice.HasValue ? p.UnitPrice.Value.ToString("C2") : "",
                            IsDeleted = (category.IsDeleted ?? false) || (store.IsDeleted ?? false)
                        };
            return query;
        }
    }
}
