#nullable disable
using AppCoreLite.Enums;
using DataAccessDemo.Entities;
using DataAccessDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class CategoriesController : ControllerBase
    {
        // Add service injections here
        private readonly CategoryServiceBase _categoryService;

        public CategoriesController(CategoryServiceBase categoryService)
        {
            _categoryService = categoryService;
            _categoryService.Set(Languages.English);
        }

        // GET: api/Categories
        [HttpGet]
        public IActionResult Get()
        {
            List<CategoryModel> categoryList = _categoryService.GetList(c => c.Products);
            return Ok(categoryList);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            CategoryModel category = _categoryService.GetItem(id);
			if (category == null)
            {
                return NotFound();
            }
			return Ok(category);
        }

		// POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public IActionResult Post(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                var result = _categoryService.Add(category);
                if (result.IsSuccessful)
                {
			        //return CreatedAtAction("Get", new { id = category.Id }, category);
                    return Ok(category);
                }
                ModelState.AddModelError("", result.Message);
            }
            return BadRequest(ModelState);
        }

        // PUT: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult Put(int id, CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                category.Id = id;
                var result = _categoryService.Update(category);
                if (result.IsSuccessful)
                {
			        //return NoContent();
                    return Ok(category);
                }
                ModelState.AddModelError("", result.Message);
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _categoryService.Delete(id);
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
