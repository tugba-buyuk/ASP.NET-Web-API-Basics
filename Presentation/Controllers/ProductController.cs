using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
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
    //[ApiVersion("1.0")]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/products")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize(Roles ="Admin")]


    //[ResponseCache(CacheProfileName ="5mins")]
    //[HttpCacheExpiration(CacheLocation =CacheLocation.Public, MaxAge =75)]
    public class ProductsController : ControllerBase
    {
        private readonly IServiceManager _manager;

        public ProductsController(IServiceManager manager)
        {
            _manager = manager;
        }

        [HttpHead]
        [HttpGet(Name ="GetAllProductAsync")]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public async Task<IActionResult> GetAllProductsAsync([FromQuery] ProductParameters productParameters)
        {
            var linkParameters = new LinkParameters()
            {
                ProductParameters = productParameters,
                HttpContext = HttpContext
            };

            var result = await _manager.ProductService.AllProductsAsync(linkParameters,false);
            Response.Headers.Add("X-Pagination",JsonSerializer.Serialize(result.metaData));
            return result.linkResponse.HasLink ?
                Ok(result.linkResponse.LinkedEntites) :
                Ok(result.linkResponse.ShapedEntities);
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetAllProductsWithCategories()
        {
            return Ok( await _manager.ProductService.AllProductsWithCategories(false));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneProductAsync([FromRoute(Name = "id")] int id)
        {

            var entity =await _manager.ProductService.OneProductwithIDAsync(id, false);
            return Ok(entity);

        }

        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost(Name = "CreateOneProductAsync")]
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
        
        [HttpOptions]
        public IActionResult GetProductsOptions()
        {
            Response.Headers.Add("Allow", "GET,POST,PUT,DELETE,HEAD,OPTIONS");
            return Ok();
        }
        
    }
}
