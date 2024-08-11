using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using first_Application.Data;
using Microsoft.AspNetCore.JsonPatch;

using Entities.Models;
using Repositories.EFCore;
using Repositories.Contracts;

namespace first_Application.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IRepositoryManager _manager;

        public ProductsController(IRepositoryManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            try
            {
                var products = _manager.Product.FindAll(false);
                return Ok(products);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpGet("{id:int}")]
        public IActionResult GetOneProduct([FromRoute(Name = "id")] int id)
        {
            try
            {
                var entity =_manager.Product.GetOneProductById(id,false);
                if (entity is null)
                {
                    return NotFound();//404
                }
                return Ok(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
                _manager.Product.CreateOneProduct(product);
                _manager.Save();
                return StatusCode(201, product);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateOneProduct([FromRoute(Name = "id")] int id, [FromBody] Product product)
        {
            try
            {
                var entity = _manager.Product.GetOneProductById(id,false);
                if (entity is null)
                {
                    return NotFound();
                }
                if (entity.Id != id)
                {
                    return BadRequest();
                }
                entity.ProductName = product.ProductName;
                entity.Price = product.Price;
                _manager.Save();
                return Ok(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneProduct([FromRoute(Name = "id")] int id)
        {
            try
            {
                var entity = _manager.Product.GetOneProductById(id, false);
                if (entity is null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = $"Product with {id} id is not found"
                    });
                }
                _manager.Product.DeleteOneProduct(entity);
                _manager.Save();

                return NoContent();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPatch("{id:int}")]
        public IActionResult PartiallyUpdateOneProduct([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<Product> pathProduct)
        {
            try
            {
                var entity = _manager.Product.GetOneProductById(id, false);
                if (entity is null)
                {
                    return NotFound();
                }
                pathProduct.ApplyTo(entity);
                _manager.Product.Update(entity);
                _manager.Save();
                return NoContent();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
