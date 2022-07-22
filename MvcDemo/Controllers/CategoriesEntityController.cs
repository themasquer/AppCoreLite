#nullable disable
using AppCoreLite.Enums;
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
    public class CategoriesEntityController : Controller
    {
        // Add service injections here
        private readonly CategoryEntityServiceBase _categoryService;
        private readonly DataTableManagerBase _dataTableManager;

        public CategoriesEntityController(CategoryEntityServiceBase categoryService, DataTableManagerBase dataTableManager)
        {
            _categoryService = categoryService;
            _categoryService.Set(Languages.English);
            _dataTableManager = dataTableManager;
            _dataTableManager.Set(Languages.English);
        }

        // GET: CategoriesEntity
        public IActionResult Index()
        {
            return View(new CategoriesEntityIndexViewModel());
        }

        [HttpPost]
        public async Task<JsonResult> BindDataTable([FromBody]DtParameters dtParameters)
        {
            var query = _categoryService.Query(c => c.ProductEntities);
            Expression<Func<CategoryEntity, bool>> predicate = null;
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
                DetailsUrl = "/CategoriesEntity/Details",
                EditUrl = "/CategoriesEntity/Edit",
                DeleteUrl = "/CategoriesEntity/Delete"
            };
            _dataTableManager.AddOperations(dataTable, dataTableOperations);
            return Json(dataTable);
        }

        // GET: CategoriesEntity/Details/5
        public IActionResult Details(int id)
        {
            CategoryEntity categoryEntity = _categoryService.GetItem(id);
            if (categoryEntity == null)
            {
                //return NotFound();
                ViewBag.Message = _categoryService.Config.RecordNotFound;
            }
            return View(categoryEntity);
        }

        // GET: CategoriesEntity/Create
        public IActionResult Create()
        {
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View();
        }

        // POST: CategoriesEntity/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryEntity categoryEntity)
        {
            if (ModelState.IsValid)
            {
                var result = _categoryService.Add(categoryEntity);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                    return RedirectToAction(nameof(Index));
                }
                //ViewBag.Message = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View(categoryEntity);
        }

        // GET: CategoriesEntity/Edit/5
        public IActionResult Edit(int id)
        {
            CategoryEntity categoryEntity = _categoryService.GetItem(id);
            if (categoryEntity == null)
            {
                //return NotFound();
                ViewBag.Message = _categoryService.Config.RecordNotFound;
            }
            return View(categoryEntity);
        }

        // POST: CategoriesEntity/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryEntity categoryEntity)
        {
            if (ModelState.IsValid)
            {
                var result = _categoryService.Update(categoryEntity);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                    return RedirectToAction(nameof(Index));
                }
                //ViewBag.Message = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View(categoryEntity);
        }

        // GET: CategoriesEntity/Delete/5
        public IActionResult Delete(int id)
        {
            CategoryEntity categoryEntity = _categoryService.GetItem(id);
            if (categoryEntity == null)
            {
                //return NotFound();
                ViewBag.Message = _categoryService.Config.RecordNotFound;
            }
            return View(categoryEntity);
        }

        // POST: CategoriesEntity/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var result = _categoryService.Delete(id);
            TempData["Message"] = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
            return RedirectToAction(nameof(Index));
        }
	}
}
