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
    [Authorize]
    public class ProductsController : ControllerBase
    {
        // Add service injections here
        private readonly ProductServiceBase _productService;

        public ProductsController(ProductServiceBase productService)
        {
            _productService = productService;
            _productService.Set(Languages.English);
        }

        // GET: api/Products
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            List<ProductModel> productList = _productService.GetList();
            foreach (var product in productList)
            {
                product.StoresDisplay = product.StoresDisplay.RemoveHtmlTags();
            }
            return Ok(productList);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            ProductModel product = _productService.GetItem(id);
			if (product == null)
            {
                return NotFound();
            }
			return Ok(product);
        }

		// POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Post(ProductModel product)
        {
            if (ModelState.IsValid)
            {
                var result = _productService.Add(product);
                if (result.IsSuccessful)
                {
			        //return CreatedAtAction("Get", new { id = product.Id }, product);
                    return Ok(product);
                }
                ModelState.AddModelError("", result.Message);
            }
            return BadRequest(ModelState);
        }

        // PUT: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Put(int id, ProductModel product)
        {
            if (ModelState.IsValid)
            {
                product.Id = id;
                var result = _productService.Update(product);
                if (result.IsSuccessful)
                {
			        //return NoContent();
                    return Ok(product);
                }
                ModelState.AddModelError("", result.Message);
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            var result = _productService.Delete(id);
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
