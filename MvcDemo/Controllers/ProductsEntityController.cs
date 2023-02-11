#nullable disable
using AppCoreLite.Enums;
using AppCoreLite.Models;
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
    public class ProductsEntityController : Controller
    {
        // Add service injections here
        private readonly ProductEntityServiceBase _productService;
        private readonly CategoryEntityServiceBase _categoryService;
        private readonly StoreEntityServiceBase _storeService;

        public ProductsEntityController(ProductEntityServiceBase productService, CategoryEntityServiceBase catgoryService, StoreEntityServiceBase storeService)
        {
            _productService = productService;
            _categoryService = catgoryService;
            _storeService = storeService;
            _productService.Set(Languages.English);
            _categoryService.Set(Languages.English);
            _storeService.Set(Languages.English);
            _productService.Set(AppSettings.ImageExtensions, AppSettings.ImageMaximumLength);
        }

        // GET: Products
        [AllowAnonymous]
        public IActionResult Index(ProductsEntityIndexViewModel viewModel, bool usePageOrderFilterSession = false)
        {
            viewModel.Set(Languages.English);
            List<ProductEntity> productList = _productService.GetList(viewModel.PageOrderFilter, usePageOrderFilterSession);
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
            ProductEntity product = _productService.GetItem(p => p.Id == id);
            if (product == null)
            {
                //return NotFound();
                ViewBag.Message = _productService.Config.RecordNotFound;
            }
            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            ViewData["CategoryId"] = new SelectList(_categoryService.GetList(new PageOrderFilter()), "Id", "Name");
            ViewData["StoreIds"] = new MultiSelectList(_storeService.GetList(new PageOrderFilter()), "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public IActionResult Create(ProductEntity product, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                _productService.Set(image);
                var result = _productService.Add(product);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                    return RedirectToAction(nameof(Index), new { usePageOrderFilterSession = true });
                }
                //ViewBag.Message = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            ViewData["CategoryId"] = new SelectList(_categoryService.GetList(new PageOrderFilter()), "Id", "Name", product.CategoryEntityId);
            ViewData["StoreIds"] = new MultiSelectList(_storeService.GetList(new PageOrderFilter()), "Id", "Name", product.StoreEntityIds);
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "admin")]
        public IActionResult Edit(int id)
        {
            ProductEntity product = _productService.GetItem(p => p.Id == id);
            if (product == null)
            {
                //return NotFound();
                ViewBag.Message = _productService.Config.RecordNotFound;
            }
            else
            {
                // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
                ViewData["CategoryId"] = new SelectList(_categoryService.GetList(new PageOrderFilter()), "Id", "Name", product.CategoryEntityId);
                ViewData["StoreIds"] = new MultiSelectList(_storeService.GetList(new PageOrderFilter()), "Id", "Name", product.StoreEntityIds);
            }
            return View(product);
        }

        // POST: Products/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public IActionResult Edit(ProductEntity product, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                _productService.Set(image);
                var result = _productService.Update(product);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                    return RedirectToAction(nameof(Index), new { usePageOrderFilterSession = true });
                }
                //ViewBag.Message = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            ViewData["CategoryId"] = new SelectList(_categoryService.GetList(new PageOrderFilter()), "Id", "Name", product.CategoryEntityId);
            ViewData["StoreIds"] = new MultiSelectList(_storeService.GetList(new PageOrderFilter()), "Id", "Name", product.StoreEntityIds);
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            ProductEntity product = _productService.GetItem(p => p.Id == id);
            if (product == null)
            {
                //return NotFound();
                ViewBag.Message = _productService.Config.RecordNotFound;
            }
            return View(product);
        }

        // POST: Products/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteConfirmed(int id)
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
