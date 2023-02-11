using AppCoreLite.Results.Bases;
using AppCoreLite.Services;
using AutoMapper;
using DataAccessDemo.Contexts;
using DataAccessDemo.Entities;
using Microsoft.AspNetCore.Http;

namespace DataAccessDemo.Services
{
    public class CategoryServiceBase : Service<Category, CategoryModel>
    {
        public CategoryServiceBase(Db db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
        }
    }

    public class CategoryService : CategoryServiceBase
    {
        public CategoryService(Db db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
            Set(new MapperConfiguration(c => c.AddProfile<CategoryProfile>()));
        }

        public override Result Add(CategoryModel model, bool save = true, bool trim = true)
        {
            var result = base.ItemExists(c => c.Name.ToLower() == model.Name.ToLower().Trim());
            if (result.IsSuccessful)
                return Error(result.Message + " " + Config.OperationFailed);
            return base.Add(model, save, trim);
        }

        public override Result Update(CategoryModel model, bool save = true, bool trim = true)
        {
            var result = base.ItemExists(c => c.Name.ToLower() == model.Name.ToLower().Trim() && c.Id != model.Id);
            if (result.IsSuccessful)
                return Error(result.Message + " " + Config.OperationFailed);
            return base.Update(model, save, trim);
        }

        public override Result Delete(int id, bool save = true)
        {
            if (base.GetItem(id).ProductCount > 0)
                return Error(Config.RelatedRecordsFound + " " + Config.OperationFailed);
            return base.Delete(id, save);
        }
    }
}
