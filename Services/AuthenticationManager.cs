using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AuthenticationManager : IAuthenticationService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly ILoggerService _logger;

        public AuthenticationManager(IMapper mapper, IConfiguration configuration, UserManager<User> userManager, ILoggerService logger)
        {
            _mapper = mapper;
            _configuration = configuration;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IdentityResult> RegisterUser(UserDTOForRegistration userDTOForRegistration)
        {
            var user= _mapper.Map<User>(userDTOForRegistration);
            var result= await _userManager.CreateAsync(user,userDTOForRegistration.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user,userDTOForRegistration.Roles);
            }
            return result;
        }
    }
}
