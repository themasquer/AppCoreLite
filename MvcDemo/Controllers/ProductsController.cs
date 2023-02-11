#nullable disable
using AppCoreLite.Extensions;
using DataAccessDemo.Entities;
using DataAccessDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcDemo.Models;
using MvcDemo.Settings;

namespace MvcDemo.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        // Add service injections here
        private readonly ProductServiceBase _productService;
        private readonly CategoryServiceBase _categoryService;
        private readonly StoreServiceBase _storeService;

        public ProductsController(ProductServiceBase productService, CategoryServiceBase categoryService, StoreServiceBase storeService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _storeService = storeService;
            _productService.Set(AppSettings.ImageExtensions, AppSettings.ImageMaximumLength, "wwwroot", "files", "products");
        }

        // GET: Products
        [AllowAnonymous]
        public IActionResult Index(ProductsIndexViewModel viewModel, bool usePageOrderFilterSession = false)
        {
            List<ProductModel> productList = _productService.GetList(viewModel.PageOrderFilter, usePageOrderFilterSession, p => p.Category, p => p.ProductStores);
            ViewBag.Message = _productService.TotalRecordsCount;
            viewModel.Products = productList;
            viewModel.PageNumbers = new SelectList(_productService.PageNumbers);
            viewModel.RecordsPerPageCounts = new SelectList(_productService.RecordsPerPageCounts);
            viewModel.OrderExpressions = new SelectList(_productService.OrderExpressions);
            return View(viewModel);
        }

        // GET: Products/Details/5
        public IActionResult Details(int id)
        {
            ProductModel product = _productService.GetItem(id, p => p.Category, p => p.ProductStores);
            if (product == null)
            {
                return NotFound(_productService.Config.RecordNotFound);
            }
            if (product.UnitPrice.HasValue)
                product.UnitPriceTextDisplay = product.UnitPrice.Value.ConvertMoneyToString();
            return PartialView("_Details", product);
        }

        // GET: Products/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            ViewData["CategoryId"] = new SelectList(_categoryService.GetList(), "Id", "Name");
            ViewData["StoreIds"] = new SelectList(_storeService.GetList(), "Id", "Name");
            return PartialView("_CreateEdit");
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public IActionResult Create(ProductModel product, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                _productService.Set(image);
                var result = _productService.Add(product);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                    var queryString = "?usePageOrderFilterSession=true";
                    return Ok(queryString);
                }
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            ViewData["CategoryId"] = new SelectList(_categoryService.GetList(), "Id", "Name", product.CategoryId);
            ViewData["StoreIds"] = new SelectList(_storeService.GetList(), "Id", "Name", product.StoreIds);
            return PartialView("_CreateEdit", product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "admin")]
        public IActionResult Edit(int id)
        {
            ProductModel product = _productService.GetItem(id, p => p.Category, p => p.ProductStores);
            if (product == null)
            {
                return NotFound(_productService.Config.RecordNotFound);
            }
            else
            {
                // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
                ViewData["CategoryId"] = new SelectList(_categoryService.GetList(), "Id", "Name", product.CategoryId);
                ViewData["StoreIds"] = new SelectList(_storeService.GetList(), "Id", "Name", product.StoreIds);
            }
            return PartialView("_CreateEdit", product);
        }

        // POST: Products/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public IActionResult Edit(ProductModel product, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                _productService.Set(image);
                var result = _productService.Update(product);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
					var queryString = "?usePageOrderFilterSession=true";
					return Ok(queryString);
				}
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            ViewData["CategoryId"] = new SelectList(_categoryService.GetList(), "Id", "Name", product.CategoryId);
            ViewData["StoreIds"] = new SelectList(_storeService.GetList(), "Id", "Name", product.StoreIds);
            return PartialView("_CreateEdit", product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            var result = _productService.Delete(id);
            TempData["Message"] = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
            return RedirectToAction(nameof(Index), new { usePageOrderFilterSession = true });
        }

        [Authorize(Roles = "admin")]
        public IActionResult DeleteFile(int id)
        {
            _productService.DeleteFile(id, true);
            return RedirectToAction(nameof(Index), new { usePageOrderFilterSession = true });
        }

        [Authorize(Roles = "admin")]
        public IActionResult DownloadFile(int id)
        {
            var file = _productService.DownloadFile(id);
            return File(file.Stream, file.ContentType, file.FileName);
        }
    }
}
