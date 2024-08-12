using Entities.DataTransferObjects;
using Entities.Exceptions;
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

            var products = _manager.ProductService.AllProducts(false);
            return Ok(products);


        }

        [HttpGet("{id:int}")]
        public IActionResult GetOneProduct([FromRoute(Name = "id")] int id)
        {

            var entity = _manager.ProductService.OneProductwithID(id, false);
            return Ok(entity);

        }

        [HttpPost]
        public IActionResult CreateOneProduct([FromBody] Product product)
        {

            if (product is null)
            {
                return BadRequest();
            }
            _manager.ProductService.CreateProduct(product);
            return StatusCode(201, product);

        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateOneProduct([FromRoute(Name = "id")] int id, [FromBody] ProductDTOForUpdate productDto)
        {

            if (productDto is null)
            {
                return BadRequest();
            }

            _manager.ProductService.UpdateProduct(id, productDto, true);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneProduct([FromRoute(Name = "id")] int id)
        {

            _manager.ProductService.DeleteProduct(id, false);
            return NoContent();

        }

        [HttpPatch("{id:int}")]
        public IActionResult PartiallyUpdateOneProduct([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<Product> pathProduct)
        {

            var entity = _manager.ProductService.OneProductwithID(id, true);
            if (entity is null)
            {
                throw new ProductNotFoundException(id);
            }
            pathProduct.ApplyTo(entity);
            _manager.ProductService.UpdateProduct(id, new ProductDTOForUpdate(entity.Id,entity.ProductName,entity.Price), true);
            return NoContent();

        }
    }
}
