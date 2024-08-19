using Entities.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly IServiceManager _manager;

        public CategoryController(IServiceManager maanger)
        {
            _manager = maanger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            return Ok(await _manager.CategoryService.AllCategoriesAsync(false));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneCategory(int id)
        {
            var category=await _manager.CategoryService.OneCategory(id,false);
            if(category is null)
            {
                throw new CategoryNotFoundException(id);
            }
            return Ok(category);
        }
    }
}
