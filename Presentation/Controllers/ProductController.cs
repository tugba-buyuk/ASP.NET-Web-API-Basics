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
        public async Task<IActionResult> GetAllProductsAsync()
        {

            var products = await _manager.ProductService.AllProductsAsync(false);
            return Ok(products);


        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneProductAsync([FromRoute(Name = "id")] int id)
        {

            var entity =await _manager.ProductService.OneProductwithIDAsync(id, false);
            return Ok(entity);

        }

        [HttpPost]
        public async Task<IActionResult> CreateOneProductAsync([FromBody] ProductDTOForInsertion productDto)
        {

            if (productDto is null)
            {
                return BadRequest();
            }
            if(!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            await _manager.ProductService.CreateProductAsync(productDto);
            return StatusCode(201, productDto);

        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOneProductAsync([FromRoute(Name = "id")] int id, [FromBody] ProductDTOForUpdate productDto)
        {

            if (productDto is null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            await _manager.ProductService.UpdateProductAsync(id, productDto, true);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOneProductAsync([FromRoute(Name = "id")] int id)
        {

            await _manager.ProductService.DeleteProductAsync(id, false);
            return NoContent();

        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PartiallyUpdateOneProductAsync([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<ProductDTOForUpdate> pathProduct)
        {
           if(pathProduct is null)
            {
                return BadRequest();
            }
            var result =await  _manager.ProductService.GetOneProductForPatchAsync(id, false);
            pathProduct.ApplyTo(result.productDto, ModelState);

            TryValidateModel(result.productDto);
            if(!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            await _manager.ProductService.SaveChangesForPatchAsync(result.productDto, result.product);

            return NoContent();

        }
    }
}
