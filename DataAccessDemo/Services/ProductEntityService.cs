using AppCoreLite.Results.Bases;
using AppCoreLite.Services;
using AppCoreLite.Utilities;
using DataAccessDemo.Contexts;
using DataAccessDemo.Entities;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace DataAccessDemo.Services
{
    public class ProductEntityServiceBase : EntityService<ProductEntity>
    {
        public ProductEntityServiceBase(Db db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
        }
    }

    public class ProductEntityService : ProductEntityServiceBase
    {
        public ProductEntityService(Db db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
        }

        public override IQueryable<ProductEntity> Query(params Expression<Func<ProductEntity, object?>>[] navigationPropertyPaths)
        {
            return base.Query(p => p.CategoryEntity, p => p.ProductStoreEntities).Select(p => new ProductEntity()
            {
                CategoryEntity = p.CategoryEntity,
                CategoryEntityId = p.CategoryEntityId,
                CategoryEntityDisplay = p.CategoryEntity.Name,
                Description = p.Description,
                Guid = p.Guid,
                Id = p.Id,
                Name = p.Name,
                ProductStoreEntities = p.ProductStoreEntities,
                StoreEntityIds = p.ProductStoreEntities.Select(ps => ps.StoreEntityId).ToList(),
                StoreEntities = p.ProductStoreEntities.Select(ps => ps.StoreEntity).ToList(),
                UnitPrice = p.UnitPrice,
                UnitPriceDisplay = p.UnitPrice.HasValue ? p.UnitPrice.Value.ToString("C2") : "",
                StockAmount = p.StockAmount,
                ExpirationDate = p.ExpirationDate,
                ExpirationDateDisplay = p.ExpirationDate.HasValue ? p.ExpirationDate.Value.ToString("MM/dd/yyyy") : "",
                FileContent = RecordFileUtil.GetImgSrc(p)
            });
        }

        public override Result Add(ProductEntity entity, bool save = true, bool trim = true)
        {
            var result = base.ItemExists(p => p.Name.ToLower() == entity.Name.ToLower().Trim());
            if (result.IsSuccessful)
                return Error(result.Message + " " + Config.OperationFailed);
            entity.ProductStoreEntities = entity.StoreEntityIds?.Select(sId => new ProductStoreEntity()
            {
                StoreEntityId = sId
            }).ToList();
            return base.Add(entity, save, trim);
        }

        public override Result Update(ProductEntity entity, bool save = true, bool trim = true)
        {
            var result = base.ItemExists(p => p.Name.ToLower() == entity.Name.ToLower().Trim() && p.Id != entity.Id);
            if (result.IsSuccessful)
                return Error(result.Message + " " + Config.OperationFailed);
            base.Delete<ProductStoreEntity>(ps => ps.ProductEntityId == entity.Id);
            entity.ProductStoreEntities = entity.StoreEntityIds?.Select(sId => new ProductStoreEntity()
            {
                StoreEntityId = sId
            }).ToList();
            return base.Update(entity, save, trim);
        }
    }
}
