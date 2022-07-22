#nullable disable
using AppCoreLite.Managers.Bases;
using AppCoreLite.Models;
using DataAccessDemo.Entities;
using DataAccessDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcDemo.Models;
using System.Linq.Expressions;

namespace MvcDemo.Controllers
{
    [Authorize(Roles = "admin")]
    public class CategoriesController : Controller
    {
        // Add service injections here
        private readonly CategoryServiceBase _categoryService;
        private readonly DataTableManagerBase _dataTableManager;

        public CategoriesController(CategoryServiceBase categoryService, DataTableManagerBase dataTableManager)
        {
            _categoryService = categoryService;
            _dataTableManager = dataTableManager;
        }

        // GET: Categories
        public IActionResult Index()
        {
            return View(new CategoriesIndexViewModel());
        }

        [HttpPost]
        public async Task<JsonResult> BindDataTable([FromBody]DtParameters dtParameters)
        {
            var query = _categoryService.Query(c => c.Products);
            Expression<Func<CategoryModel, bool>> predicate = null;
            if (!string.IsNullOrWhiteSpace(dtParameters.Search.Value))
            {
                var searchValue = dtParameters.Search.Value.ToLower().Trim();
                predicate = c =>
                    (c.Name != null && c.Name.ToLower().Contains(searchValue)) ||
                    (c.ProductCountDisplay != null && c.ProductCountDisplay.Contains(searchValue)) ||
                    (c.CreatedBy != null && c.CreatedBy.ToLower().Contains(searchValue)) ||
                    (c.UpdatedBy != null && c.UpdatedBy.ToLower().Contains(searchValue));
            }
            var dataTable = await _dataTableManager.Bind(dtParameters, query, predicate);
            var dataTableOperations = new DtOperationsModel()
            {
                DetailsUrl = "/Categories/Details",
                EditUrl = "/Categories/Edit",
                DeleteUrl = "/Categories/Delete"
            };
            _dataTableManager.AddOperations(dataTable, dataTableOperations, true);
            return Json(dataTable);
        }

        // GET: Categories/Details/5
        public IActionResult Details(int id)
        {
            CategoryModel category = _categoryService.GetItem(id);
            if (category == null)
            {
                //return NotFound();
                ViewBag.Message = _categoryService.Config.RecordNotFound;
            }
            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                var result = _categoryService.Add(category);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                    return RedirectToAction(nameof(Index));
                }
                //ViewBag.Message = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View(category);
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(int id)
        {
            CategoryModel category = _categoryService.GetItem(id);
            if (category == null)
            {
                //return NotFound();
                ViewBag.Message = _categoryService.Config.RecordNotFound;
            }
            return View(category);
        }

        // POST: Categories/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                var result = _categoryService.Update(category);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                    return RedirectToAction(nameof(Index));
                }
                //ViewBag.Message = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View(category);
        }

        // GET: Categories/Delete/5
        public IActionResult Delete(int id)
        {
            var result = _categoryService.Delete(id);
            TempData["Message"] = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
            return RedirectToAction(nameof(Index));
        }
	}
}
