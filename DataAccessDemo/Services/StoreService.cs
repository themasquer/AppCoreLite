using AppCoreLite.Results;
using AppCoreLite.Results.Bases;
using AppCoreLite.Services;
using AppCoreLite.Utils.Bases.Service;
using AutoMapper;
using DataAccessDemo.Contexts;
using DataAccessDemo.Entities;

namespace DataAccessDemo.Services
{
    public class StoreServiceBase : Service<Store, StoreModel>
    {
        public StoreServiceBase(Db db, UserUtilBase? userUtil, SessionUtilBase? sessionUtil, FileUtilBase? fileUtil) : base(db, userUtil, sessionUtil, fileUtil)
        {
        }
    }

    public class StoreService : StoreServiceBase
    {
        public StoreService(Db db, UserUtilBase? userUtil, SessionUtilBase? sessionUtil, FileUtilBase? fileUtil) : base(db, userUtil, sessionUtil, fileUtil)
        {
            SetMapper(new MapperConfiguration(c => c.AddProfile<StoreProfile>()));
        }

        public override Result Add(StoreModel model, bool save = true, bool trim = true)
        {
            var result = base.ItemExists(s => s.Name.ToLower() == model.Name.ToLower().Trim());
            if (result.IsSuccessful)
                return new ErrorResult(result.Message + " " + Config.OperationFailed);
            model.ProductStores = model.ProductIds?.Select(pId => new ProductStore()
            {
                ProductId = pId
            }).ToList();
            return base.Add(model, save, trim);
        }

        public override Result Update(StoreModel model, bool save = true, bool trim = true)
        {
            var result = base.ItemExists(s => s.Name.ToLower() == model.Name.ToLower().Trim() && s.Id != model.Id);
            if (result.IsSuccessful)
                return new ErrorResult(result.Message + " " + Config.OperationFailed);
            _db.Set<ProductStore>().RemoveRange(_db.Set<ProductStore>().Where(ps => ps.StoreId == model.Id));
            model.ProductStores = model.ProductIds?.Select(pId => new ProductStore()
            {
                ProductId = pId
            }).ToList();
            return base.Update(model, save, trim);
        }
    }
}
