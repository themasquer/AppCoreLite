#nullable disable
using AppCoreLite.Enums;
using AppCoreLite.Models;
using DataAccessDemo.Entities;
using DataAccessDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcDemo.Controllers
{
    [Authorize(Roles = "admin")]
    public class StoresEntityController : Controller
    {
        // Add service injections here
        private readonly StoreEntityServiceBase _storeService;
        private readonly ProductEntityServiceBase _productService;

        public StoresEntityController(StoreEntityServiceBase storeService, ProductEntityServiceBase productService)
        {
            _storeService = storeService;
            _productService = productService;
            _storeService.Set(Languages.English);
            _productService.Set(Languages.English);
        }

        // GET: StoresEntity
        public IActionResult Index()
        {
            List<StoreEntity> storeList = _storeService.GetList();
            ViewBag.Message = _storeService.TotalRecordsCount;
            return View(storeList);
        }

        // GET: StoresEntity/Details/5
        public IActionResult Details(int id)
        {
            StoreEntity store = _storeService.GetItem(id);
            if (store == null)
            {
                //return NotFound();
                ViewBag.Message = _storeService.Config.RecordNotFound;
            }
            return View(store);
        }

        // GET: StoresEntity/Create
        public IActionResult Create()
        {
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            ViewData["ProductIds"] = new MultiSelectList(_productService.GetList(new PageOrderFilter()), "Id", "Name");
            return View();
        }

        // POST: StoresEntity/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StoreEntity store)
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
            ViewData["ProductIds"] = new MultiSelectList(_productService.GetList(new PageOrderFilter()), "Id", "Name", store.ProductEntityIds);
            return View(store);
        }

        // GET: StoresEntity/Edit/5
        public IActionResult Edit(int id)
        {
            StoreEntity store = _storeService.GetItem(id);
            if (store == null)
            {
                //return NotFound();
                ViewBag.Message = _storeService.Config.RecordNotFound;
            }
            else
            {
                ViewData["ProductIds"] = new MultiSelectList(_productService.GetList(new PageOrderFilter()), "Id", "Name", store.ProductEntityIds);
            }
            return View(store);
        }

        // POST: StoresEntity/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StoreEntity store)
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
            ViewData["ProductIds"] = new MultiSelectList(_productService.GetList(new PageOrderFilter()), "Id", "Name", store.ProductEntityIds);
            return View(store);
        }

        // GET: StoresEntity/Delete/5
        public IActionResult Delete(int id)
        {
            var result = _storeService.Delete(id);
            TempData["Message"] = result.Message; // End message in service with '.' for success, '!' for danger Bootstrap CSS classes to be used in the View
            return RedirectToAction(nameof(Index));
        }
	}
}
