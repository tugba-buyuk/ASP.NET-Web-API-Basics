using Entities.DataTransferObjects;
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
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IServiceManager _manager;

        public AuthenticationController(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IActionResult> RegisterUser([FromBody] UserDTOForRegistration userDTOForRegistration)
        {
            var result= await _manager.AuthenticationService.RegisterUser(userDTOForRegistration);
            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return StatusCode(201);
        }
    }
}
