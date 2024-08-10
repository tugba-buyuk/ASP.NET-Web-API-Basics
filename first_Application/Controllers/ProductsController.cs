using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using first_Application.Models;
using first_Application.Data;
using Microsoft.AspNetCore.JsonPatch;

namespace first_Application.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ILogger<ProductsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var products = ApplicationContext.Products;
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetOneProduct([FromRoute(Name = "id")] int id)
        {
            var product = ApplicationContext.Products.Where(p => p.Id.Equals(id)).FirstOrDefault();
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);

        }

        [HttpPost]
        public IActionResult CreateOneProduct([FromBody] Product product)
        {
            try
            {
                if (product is null)
                {
                    return BadRequest();
                }
                ApplicationContext.Products.Add(product);
                return StatusCode(201, product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateOneProduct([FromRoute(Name = "id")] int id, [FromBody] Product product)
        {
            var entity = ApplicationContext.Products.Find(p => p.Id.Equals(product.Id));
            if (entity is null)
            {
                return NotFound(); //404
            }
            if (id != entity.Id)
            {
                return BadRequest(); //400
            }
            ApplicationContext.Products.Remove(entity);
            product.Id = entity.Id;
            ApplicationContext.Products.Add(product);
            return Ok(product);

        }

        [HttpDelete]
        public IActionResult DeleteAllProducts()
        {
            ApplicationContext.Products.Clear();
            return NoContent(); //204
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneProduct([FromRoute(Name = "id")] int id)
        {
            var entity = ApplicationContext.Products.Find(p => p.Id.Equals(id));
            if (entity is null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = $"Product with {id} id is not found"
                });
            }
            ApplicationContext.Products.Remove(entity);
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public IActionResult PartiallyUpdateOneProduct([FromRoute(Name="id")]int id,[FromBody] JsonPatchDocument<Product> pathProduct)
        {
            var entity = ApplicationContext.Products.Find(p => p.Id.Equals(id));
            if(entity is null){
                return NotFound();
            }
            pathProduct.ApplyTo(entity);
            return NoContent();
        }
    }
}
