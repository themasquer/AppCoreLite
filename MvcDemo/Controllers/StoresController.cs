#nullable disable
using AppCoreLite.Extensions;
using AppCoreLite.Managers.Bases;
using AppCoreLite.Models;
using DataAccessDemo.Entities;
using DataAccessDemo.Managers;
using DataAccessDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcDemo.Controllers
{
    [Authorize(Roles = "admin")]
    public class StoresController : Controller
    {
        // Add service injections here
        private readonly StoreServiceBase _storeService;
        private readonly ProductServiceBase _productService;
        private readonly StoreExportManagerBase _exportManager;

        public StoresController(StoreServiceBase storeService, ProductServiceBase productService, StoreExportManagerBase exportManager)
        {
            _storeService = storeService;
            _productService = productService;
            _exportManager = exportManager;
        }

        // GET: Stores
        public IActionResult Index()
        {
            List<StoreModel> storeList = _storeService.GetList(s => s.ProductStores);
            ViewBag.Message = _storeService.TotalRecordsCount;
            return View(storeList);
        }

        // GET: Stores/Details/5
        public IActionResult Details(int id)
        {
            StoreModel store = _storeService.GetItem(id);
            if (store == null)
            {
                //return NotFound();
                ViewBag.Message = _storeService.Config.RecordNotFound;
            }
            return View(store);
        }

        // GET: Stores/Create
        public IActionResult Create()
        {
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            ViewData["ProductIds"] = new MultiSelectList(_productService.GetList(new PageOrderFilter()), "Id", "Name");
            return View();
        }

        // POST: Stores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StoreModel store)
        {
            if (ModelState.IsValid)
            {
                var result = _storeService.Add(store);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                    return RedirectToAction(nameof(Index));
                }
                //ViewBag.Message = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            ViewData["ProductIds"] = new MultiSelectList(_productService.GetList(new PageOrderFilter()), "Id", "Name", store.ProductIds);
            return View(store);
        }

        // GET: Stores/Edit/5
        public IActionResult Edit(int id)
        {
            StoreModel store = _storeService.GetItem(id);
            if (store == null)
            {
                //return NotFound();
                ViewBag.Message = _storeService.Config.RecordNotFound;
            }
            else
            {
                ViewData["ProductIds"] = new MultiSelectList(_productService.GetList(new PageOrderFilter()), "Id", "Name", store.ProductIds);
            }
            return View(store);
        }

        // POST: Stores/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StoreModel store)
        {
            if (ModelState.IsValid)
            {
                var result = _storeService.Update(store);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                    return RedirectToAction(nameof(Index));
                }
                //ViewBag.Message = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            ViewData["ProductIds"] = new MultiSelectList(_productService.GetList(new PageOrderFilter()), "Id", "Name", store.ProductIds);
            return View(store);
        }

        // GET: Stores/Delete/5
        public IActionResult Delete(int id)
        {
            var result = _storeService.Delete(id);
            TempData["Message"] = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
            return RedirectToAction(nameof(Index));
        }

        public void Export()
        {
            var stores = _storeService.GetList();
            if (stores != null && stores.Count > 0)
            {
                for(int i = 0; i < stores.Count; i++)
                {
                    stores[i].ProductsDisplay = stores[i].ProductsDisplay.RemoveHtmlTags();
                }
            }
            _exportManager.ExportToExcel(stores);
        }
	}
}
