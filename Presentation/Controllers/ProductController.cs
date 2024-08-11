using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IServiceManager _manager;

        public ProductsController(IServiceManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            try
            {
                var products = _manager.ProductService.AllProducts(false);
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
                var entity = _manager.ProductService.OneProductwithID(id, false);
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
                _manager.ProductService.CreateProduct(product);
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
                if (product is null)
                {
                    return BadRequest();
                }

                _manager.ProductService.UpdateProduct(id, product, true);
                return NoContent();

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
                _manager.ProductService.DeleteProduct(id, false);
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
                var entity = _manager.ProductService.OneProductwithID(id, true);
                if (entity is null)
                {
                    return NotFound();
                }
                pathProduct.ApplyTo(entity);
                _manager.ProductService.UpdateProduct(id, entity, true);
                return NoContent();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
