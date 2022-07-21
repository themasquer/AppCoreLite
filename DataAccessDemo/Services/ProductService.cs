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
    public class ProductServiceBase : Service<Product, ProductModel>
    {
        public ProductServiceBase(Db db, UserUtilBase? userUtil, SessionUtilBase? sessionUtil, FileUtilBase? fileUtil) : base(db, userUtil, sessionUtil, fileUtil)
        {
        }
    }

    public class ProductService : ProductServiceBase
    {
        public ProductService(Db db, UserUtilBase? userUtil, SessionUtilBase? sessionUtil, FileUtilBase? fileUtil) : base(db, userUtil, sessionUtil, fileUtil)
        {
            SetMapper(new MapperConfiguration(c => c.AddProfile<ProductProfile>()));
        }

        public override Result Add(ProductModel model, bool save = true, bool trim = true)
        {
            var result = base.ItemExists(p => p.Name.ToLower() == model.Name.ToLower().Trim());
            if (result.IsSuccessful)
                return new ErrorResult(result.Message + " " + Config.OperationFailed);
            model.ProductStores = model.StoreIds?.Select(sId => new ProductStore()
            {
                StoreId = sId
            }).ToList();
            return base.Add(model, save, trim);
        }

        public override Result Update(ProductModel model, bool save = true, bool trim = true)
        {
            var result = base.ItemExists(p => p.Name.ToLower() == model.Name.ToLower().Trim() && p.Id != model.Id);
            if (result.IsSuccessful)
                return new ErrorResult(result.Message + " " + Config.OperationFailed);
            _db.Set<ProductStore>().RemoveRange(_db.Set<ProductStore>().Where(ps => ps.ProductId == model.Id));
            model.ProductStores = model.StoreIds?.Select(sId => new ProductStore()
            {
                StoreId = sId
            }).ToList();
            return base.Update(model, save, trim);
        }
    }
}
