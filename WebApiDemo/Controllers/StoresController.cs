#nullable disable
using AppCoreLite.Enums;
using AppCoreLite.Extensions;
using DataAccessDemo.Entities;
using DataAccessDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class StoresController : ControllerBase
    {
        // Add service injections here
        private readonly StoreServiceBase _storeService;

        public StoresController(StoreServiceBase storeService)
        {
            _storeService = storeService;
            _storeService.Set(Languages.English);
        }

        // GET: api/Stores
        [HttpGet]
        public IActionResult Get()
        {
            List<StoreModel> storeList = _storeService.GetList(s => s.ProductStores);
            foreach (var store in storeList)
            {
                store.ProductsDisplay = store.ProductsDisplay.RemoveHtmlTags();
            }
            return Ok(storeList);
        }

        // GET: api/Stores/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            StoreModel store = _storeService.GetItem(id);
			if (store == null)
            {
                return NotFound();
            }
			return Ok(store);
        }

		// POST: api/Stores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public IActionResult Post(StoreModel store)
        {
            if (ModelState.IsValid)
            {
                var result = _storeService.Add(store);
                if (result.IsSuccessful)
                {
			        //return CreatedAtAction("Get", new { id = store.Id }, store);
                    return Ok(store);
                }
                ModelState.AddModelError("", result.Message);
            }
            return BadRequest(ModelState);
        }

        // PUT: api/Stores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult Put(int id, StoreModel store)
        {
            if (ModelState.IsValid)
            {
                store.Id = id;
                var result = _storeService.Update(store);
                if (result.IsSuccessful)
                {
			        //return NoContent();
                    return Ok(store);
                }
                ModelState.AddModelError("", result.Message);
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/Stores/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _storeService.Delete(id);
            if (result.IsSuccessful)
            {
                //return NoContent();
                return Ok(id);
            }
            ModelState.AddModelError("", result.Message);
            return BadRequest(ModelState);
        }
	}
}
