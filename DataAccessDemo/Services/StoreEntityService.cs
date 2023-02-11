using AppCoreLite.Results.Bases;
using AppCoreLite.Services;
using DataAccessDemo.Contexts;
using DataAccessDemo.Entities;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace DataAccessDemo.Services
{
    public class StoreEntityServiceBase : EntityService<StoreEntity>
    {
        public StoreEntityServiceBase(Db db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
        }
    }

    public class StoreEntityService : StoreEntityServiceBase
    {
        public StoreEntityService(Db db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
        }

        public override IQueryable<StoreEntity> Query(params Expression<Func<StoreEntity, object?>>[] navigationPropertyPaths)
        {
            return base.Query(s => s.ProductStoreEntities).Select(s => new StoreEntity()
            {
                CreateDate = s.CreateDate,
                CreatedBy = s.CreatedBy,
                UpdateDate = s.UpdateDate,
                UpdatedBy = s.UpdatedBy,
                Guid = s.Guid,
                Id = s.Id,
                IsVirtual = s.IsVirtual,
                IsVirtualDisplay = s.IsVirtual ? "Yes" : "No",
                CreateDateDisplay = s.CreateDate.HasValue ? s.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : "",
                UpdateDateDisplay = s.UpdateDate.HasValue ? s.UpdateDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : "",
                Name = s.Name,
                ProductsEntityDisplay = string.Join("<br />", s.ProductStoreEntities.Select(ps => ps.ProductEntity.Name)),
                ProductStoreEntities = s.ProductStoreEntities,
                ProductEntityIds = s.ProductStoreEntities.Select(ps => ps.ProductEntityId).ToList()
            });
        }

        public override Result Add(StoreEntity entity, bool save = true, bool trim = true)
        {
            var result = base.ItemExists(s => s.Name.ToLower() == entity.Name.ToLower().Trim());
            if (result.IsSuccessful)
                return Error(result.Message + " " + Config.OperationFailed);
            entity.ProductStoreEntities = entity.ProductEntityIds?.Select(pId => new ProductStoreEntity()
            {
                ProductEntityId = pId
            }).ToList();
            return base.Add(entity, save, trim);
        }

        public override Result Update(StoreEntity entity, bool save = true, bool trim = true)
        {
            var result = base.ItemExists(s => s.Name.ToLower() == entity.Name.ToLower().Trim() && s.Id != entity.Id);
            if (result.IsSuccessful)
                return Error(result.Message + " " + Config.OperationFailed);
            base.Delete<ProductStoreEntity>(ps => ps.StoreEntityId == entity.Id);
            entity.ProductStoreEntities = entity.ProductEntityIds?.Select(pId => new ProductStoreEntity()
            {
                ProductEntityId = pId
            }).ToList();
            return base.Update(entity, save, trim);
        }
    }
}
