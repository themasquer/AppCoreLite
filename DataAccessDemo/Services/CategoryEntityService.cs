using AppCoreLite.Extensions;
using AppCoreLite.Results;
using AppCoreLite.Results.Bases;
using AppCoreLite.Services;
using AppCoreLite.Utils.Bases.Service;
using DataAccessDemo.Contexts;
using DataAccessDemo.Entities;
using System.Linq.Expressions;

namespace DataAccessDemo.Services
{
    public class CategoryEntityServiceBase : EntityService<CategoryEntity>
    {
        public CategoryEntityServiceBase(Db db, UserUtilBase? userUtil, SessionUtilBase? sessionUtil, FileUtilBase? fileUtil) : base(db, userUtil, sessionUtil, fileUtil)
        {
        }
    }

    public class CategoryEntityService : CategoryEntityServiceBase
    {
        public CategoryEntityService(Db db, UserUtilBase? userUtil, SessionUtilBase? sessionUtil, FileUtilBase? fileUtil) : base(db, userUtil, sessionUtil, fileUtil)
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
                return new ErrorResult(result.Message + " " + Config.OperationFailed);
            return base.Add(entity, save, trim);
        }

        public override Result Update(CategoryEntity entity, bool save = true, bool trim = true)
        {
            var result = base.ItemExists(c => c.Name.ToLower() == entity.Name.ToLower().Trim() && c.Id != entity.Id);
            if (result.IsSuccessful)
                return new ErrorResult(result.Message + " " + Config.OperationFailed);
            return base.Update(entity, save, trim);
        }

        public override Result Delete(int id, bool save = true)
        {
            if (base.GetItem(id).ProductCount > 0)
                return new ErrorResult(Config.RelatedRecordsFound + " " + Config.OperationFailed);
            return base.Delete(id, save);
        }
    }
}
