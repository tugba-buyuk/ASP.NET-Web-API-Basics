using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
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
        public async Task<IActionResult> GetAllProductsAsync([FromQuery] ProductParameters productParameters)
        {

            var pagedResult = await _manager.ProductService.AllProductsAsync(productParameters,false);
            Response.Headers.Add("X-Pagination",JsonSerializer.Serialize(pagedResult.metaData));
            return Ok(pagedResult.productDto);


        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneProductAsync([FromRoute(Name = "id")] int id)
        {

            var entity =await _manager.ProductService.OneProductwithIDAsync(id, false);
            return Ok(entity);

        }

        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost]
        public async Task<IActionResult> CreateOneProductAsync([FromBody] ProductDTOForInsertion productDto)
        {

            await _manager.ProductService.CreateProductAsync(productDto);
            return StatusCode(201, productDto);

        }


        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOneProductAsync([FromRoute(Name = "id")] int id, [FromBody] ProductDTOForUpdate productDto)
        {
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
