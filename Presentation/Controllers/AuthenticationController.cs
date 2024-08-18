using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
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
    [ApiExplorerSettings(GroupName = "v1")]

    public class AuthenticationController : ControllerBase
    {
        private readonly IServiceManager _manager;

        public AuthenticationController(IServiceManager manager)
        {
            _manager = manager;
        }

        [HttpPost("signup")]
        [ServiceFilter(typeof(ValidationFilterAttribute))] // Controllerda dto'nun null olup olmadığını birden fazla kez kontrol ediyorum bu yüzden bir action filter yazmak istedim
        public async Task<IActionResult> RegisterUser([FromBody] UserDTOForRegistration userDTOForRegistration)
        {
            var result= await _manager.AuthenticationService.RegisterUser(userDTOForRegistration); //Burada ise önce dto'yu user olacak şekilde map'liyorum, daha sonra create edip rol ataması yapıyorum.
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

        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UserDTOForAuthentication userDTOForAuthentication)
        {
            if(! await _manager.AuthenticationService.ValidateUser(userDTOForAuthentication)) // Dto'daki bilgiler ile db'deki user'ın eşlenip eşlenmediğini kontrol ediyorum.
                return Unauthorized(); //401
            var tokenDto =await _manager.AuthenticationService.CreateToken(true); // access ve refresh token oluşturuyorum ve bu tokenları dönüyorum.

            return Ok(tokenDto);
        }

        [HttpPost("refresh")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Refresh([FromBody] TokenDTO tokenDto)
        {
            var returnTokenDto= await _manager.AuthenticationService.RefreshToken(tokenDto);
            return Ok(returnTokenDto);
        }


        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _manager.AuthenticationService.Logout(); // önce user'ı buluyorum sonra  user'ı logout yapıyorum daha sonra da user'a ait refreshtoken ve refreshexpiretime biilgilerini null'a çekiyorum.
            if (!result)
            {
                throw new Exception("Logout is unsuccessful");
            }
            return Ok();
        }
    }
}
