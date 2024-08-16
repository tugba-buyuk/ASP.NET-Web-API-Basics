using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
        private User? _user;

        public AuthenticationManager(IMapper mapper, IConfiguration configuration, UserManager<User> userManager, ILoggerService logger)
        {
            _mapper = mapper;
            _configuration = configuration;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<string> CreateToken()
        {
            var signinCredentials = GetSigninCredentials();
            var claims = await GetClaims();
            var tokenOptions=GenerateTokenOptions(signinCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
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

        public async Task<bool> ValidateUser(UserDTOForAuthentication userDTOForAuthentication)
        {
            _user = await _userManager.FindByNameAsync(userDTOForAuthentication.UserName);
            var result = (_user != null && await _userManager.CheckPasswordAsync(_user, userDTOForAuthentication.Password));
            if (!result)
            {
                _logger.LogWarning($"{nameof(ValidateUser)}: Validating user failed. Username or password is wrong");
            }
            return result;
        }

        private SigningCredentials GetSigninCredentials()
        {
            var jwtSettingsw = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettingsw["secretKey"]);
            var secret=new SymmetricSecurityKey(key);
            return new SigningCredentials(secret,SecurityAlgorithms.HmacSha256);
        }
        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,_user.UserName)
            };
            var roles=await _userManager.GetRolesAsync(_user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signinCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings["validIssuer"],
                audience: jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
                signingCredentials: signinCredentials);
            return tokenOptions;
        }
    }
}
