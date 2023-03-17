using AppCoreLite.Enums;
using DataAccessDemo.Managers;
using DataAccessDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcDemo.Models;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataAccessDemo.Services;
using AppCoreLite.Models;

namespace MvcDemo.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProductsReportController : Controller
    {
        private readonly ProductReportManagerBase _reportManager;
        private readonly CategoryServiceBase _categoryService;
        private readonly StoreServiceBase _storeService;

        public ProductsReportController(ProductReportManagerBase reportManager, CategoryServiceBase categoryService, StoreServiceBase storeService)
        {
            _reportManager = reportManager;
            _reportManager.Set(Languages.English);
            _categoryService = categoryService;
            _categoryService.Set(Languages.English);
            _storeService = storeService;
            _storeService.Set(Languages.English);
        }

        public IActionResult Index(ProductsReportIndexViewModel viewModel)
        {
            UpdateViewModelList(viewModel);
            return View(viewModel);
        }

        public void Export(ProductsReportIndexViewModel viewModel)
        {
            UpdateViewModelList(viewModel);
            _reportManager.ExportToExcel(viewModel.Products);
        }

        public void ExportAll()
        {
            _reportManager.ExportToExcel(_reportManager.GetList());
        }

        private void UpdateViewModelList(ProductsReportIndexViewModel viewModel)
        {
            viewModel.Set(Languages.English);
            Expression<Func<ProductReportModel, bool>>? predicate = p => true;
            if (!string.IsNullOrWhiteSpace(viewModel.ProductName))
                predicate = predicate.And(p => p.ProductName.ToLower().Contains(viewModel.ProductName.ToLower().Trim()));
            if (viewModel.CategoryId.HasValue)
                predicate = predicate.And(p => p.CategoryId == viewModel.CategoryId);
            if (viewModel.StoreIds != null && viewModel.StoreIds.Count > 0)
                predicate = predicate.And(p => viewModel.StoreIds.Contains(p.StoreId ?? 0));
            if (viewModel.UnitPriceMinimum.HasValue)
                predicate = predicate.And(p => p.UnitPrice >= viewModel.UnitPriceMinimum);
            if (viewModel.UnitPriceMaximum.HasValue)
                predicate = predicate.And(p => p.UnitPrice <= viewModel.UnitPriceMaximum);
            if (viewModel.ExpirationDateMinimum.HasValue)
                predicate = predicate.And(p => p.ExpirationDate >= viewModel.ExpirationDateMinimum);
            if (viewModel.ExpirationDateMaximum.HasValue)
                predicate = predicate.And(p => p.ExpirationDate <= viewModel.ExpirationDateMaximum);
            List<ProductReportModel> products = _reportManager.GetList(viewModel.PageOrder, predicate);
            ViewBag.Message = _reportManager.TotalRecordsCount;
            viewModel.Products = products;
            viewModel.PageNumbers = new SelectList(_reportManager.PageNumbers);
            viewModel.RecordsPerPageCounts = new SelectList(_reportManager.RecordsPerPageCounts);
            viewModel.OrderExpressions = new SelectList(_reportManager.OrderExpressions);
            viewModel.Categories = new SelectList(_categoryService.GetList(new PageOrderFilter()), "Id", "Name", viewModel.CategoryId);
            viewModel.Stores = new MultiSelectList(_storeService.GetList(new PageOrderFilter()), "Id", "Name", viewModel.StoreIds);
        }
    }
}
