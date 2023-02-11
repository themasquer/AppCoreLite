using AppCoreLite.Results.Bases;
using AppCoreLite.Services;
using DataAccessDemo.Contexts;
using DataAccessDemo.Entities;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace DataAccessDemo.Services
{
    public class CategoryEntityServiceBase : EntityService<CategoryEntity>
    {
        public CategoryEntityServiceBase(Db db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
        }
    }

    public class CategoryEntityService : CategoryEntityServiceBase
    {
        public CategoryEntityService(Db db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
        }

        public override IQueryable<CategoryEntity> Query(params Expression<Func<CategoryEntity, object?>>[] navigationPropertyPaths)
        {
            return base.Query(c => c.ProductEntities).Select(c => new CategoryEntity()
            {
                Id = c.Id,
                Guid = c.Guid,
                Description = c.Description,
                Name = c.Name,
                ProductCount = c.ProductEntities.Count,
                ProductCountDisplay = c.ProductEntities.Count.ToString(),
                CreateDate = c.CreateDate,
                UpdateDate = c.UpdateDate,
                CreateDateDisplay = c.CreateDate.HasValue ? c.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : "",
                UpdateDateDisplay = c.UpdateDate.HasValue ? c.UpdateDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : "",
                CreatedBy = c.CreatedBy,
                UpdatedBy = c.UpdatedBy,
                ProductEntities = c.ProductEntities
            });
        }

        public override Result Add(CategoryEntity entity, bool save = true, bool trim = true)
        {
            var result = base.ItemExists(c => c.Name.ToLower() == entity.Name.ToLower().Trim());
            if (result.IsSuccessful)
                return Error(result.Message + " " + Config.OperationFailed);
            return base.Add(entity, save, trim);
        }

        public override Result Update(CategoryEntity entity, bool save = true, bool trim = true)
        {
            var result = base.ItemExists(c => c.Name.ToLower() == entity.Name.ToLower().Trim() && c.Id != entity.Id);
            if (result.IsSuccessful)
                return Error(result.Message + " " + Config.OperationFailed);
            return base.Update(entity, save, trim);
        }

        public override Result Delete(int id, bool save = true)
        {
            if (base.GetItem(id).ProductCount > 0)
                return Error(Config.RelatedRecordsFound + " " + Config.OperationFailed);
            return base.Delete(id, save);
        }
    }
}
