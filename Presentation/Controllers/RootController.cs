using Entities.LinkModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController : ControllerBase
    {
        private readonly LinkGenerator _linkGenerator;

        public RootController(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        [HttpGet(Name ="GetRoot")]
        public async Task<IActionResult> GetRoot([FromHeader(Name ="accept")] string mediaType)
        {
            if (mediaType.Contains("application/vnd.tb.apiroot"))
            {
                var list = new List<Link>()
                {
                    new Link()
                    {
                        Href= _linkGenerator.GetUriByName(HttpContext, nameof(GetRoot), new {}),
                        Rel="_self",
                        Method="GET"
                    },
                     new Link()
                    {
                        Href= _linkGenerator.GetUriByName(HttpContext, nameof(ProductsController.GetAllProductsAsync), new {}),
                        Rel="products",
                        Method="GET"
                    },
                      new Link()
                    {
                        Href= _linkGenerator.GetUriByName(HttpContext, nameof(ProductsController.CreateOneProductAsync), new {}),
                        Rel="products",
                        Method="POST"
                    }
                };
                return Ok(list);

            }
            else
            {
                return NoContent();
            }
            
        }
    }
}
