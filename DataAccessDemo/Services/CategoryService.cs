using AppCoreLite.Extensions;
using AppCoreLite.Results;
using AppCoreLite.Results.Bases;
using AppCoreLite.Services;
using AppCoreLite.Utils.Bases.Service;
using AutoMapper;
using DataAccessDemo.Contexts;
using DataAccessDemo.Entities;

namespace DataAccessDemo.Services
{
    public class CategoryServiceBase : Service<Category, CategoryModel>
    {
        public CategoryServiceBase(Db db, UserUtilBase? userUtil, SessionUtilBase? sessionUtil, FileUtilBase? fileUtil) : base(db, userUtil, sessionUtil, fileUtil)
        {
        }
    }

    public class CategoryService : CategoryServiceBase
    {
        public CategoryService(Db db, UserUtilBase? userUtil, SessionUtilBase? sessionUtil, FileUtilBase? fileUtil) : base(db, userUtil, sessionUtil, fileUtil)
        {
            SetMapper(new MapperConfiguration(c => c.AddProfile<CategoryProfile>()));
        }

        public override Result Add(CategoryModel model, bool save = true, bool trim = true)
        {
            var result = base.ItemExists(c => c.Name.ToLower() == model.Name.ToLower().Trim());
            if (result.IsSuccessful)
                return new ErrorResult(result.Message + " " + Config.OperationFailed);
            return base.Add(model, save, trim);
        }

        public override Result Update(CategoryModel model, bool save = true, bool trim = true)
        {
            var result = base.ItemExists(c => c.Name.ToLower() == model.Name.ToLower().Trim() && c.Id != model.Id);
            if (result.IsSuccessful)
                return new ErrorResult(result.Message + " " + Config.OperationFailed);
            return base.Update(model, save, trim);
        }

        public override Result Delete(int id, bool save = true)
        {
            if (base.GetItem(id).ProductCount > 0)
                return new ErrorResult(Config.RelatedRecordsFound + " " + Config.OperationFailed);
            return base.Delete(id, save);
        }
    }
}
