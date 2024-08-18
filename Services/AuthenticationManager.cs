using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Entities.Exceptions;
using Microsoft.AspNetCore.Http;


namespace Services
{
    public class AuthenticationManager : IAuthenticationService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly ILoggerService _logger;
        private User? _user;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<User> _signinManager;
        public AuthenticationManager(IMapper mapper, IConfiguration configuration, UserManager<User> userManager, ILoggerService logger, IHttpContextAccessor httptAccessor, SignInManager<User> signInManager)
        {
            _mapper = mapper;
            _configuration = configuration;
            _userManager = userManager;
            _logger = logger;
            _httpContextAccessor = httptAccessor;
            _signinManager = signInManager;
        }

        public async Task<TokenDTO> CreateToken(bool populateExp)
        {
            var signinCredentials = GetSigninCredentials();
            var claims = await GetClaims();
            var tokenOptions=GenerateTokenOptions(signinCredentials, claims);

            var refreshToken = GenerateRefreshToken();
            _user.RefreshToken=refreshToken;
            if (populateExp)
            {
                _user.RefreshTokenExpireTime = DateTime.Now.AddDays(7);

            }

            await _userManager.UpdateAsync(_user);
            var accesToken= new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new TokenDTO
            {
                AccessToken = accesToken,
                RefreshToken = refreshToken
            };
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
        private string GenerateRefreshToken()
        {
            var randomNumber=new byte[32];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalsFromExpireToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["secretKey"];
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
            var tokenHandler= new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal=tokenHandler.ValidateToken(token,tokenValidationParameters, out securityToken);
            var jwtSecurityToken= securityToken as JwtSecurityToken;
            if(jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }

        public async Task<TokenDTO> RefreshToken(TokenDTO tokenDTO)
        {
            var principal = GetPrincipalsFromExpireToken(tokenDTO.AccessToken);
            var user = await _userManager.FindByNameAsync(principal.Identity.Name);
            if(user is null || user.RefreshToken != tokenDTO.RefreshToken || user.RefreshTokenExpireTime <= DateTime.Now)
            {
                throw new RefreshTokenBadRequestException();
            }
            _user = user;
            return await CreateToken(populateExp: false);
        }

        public async Task<bool> Logout()
        {
            var userPrincipal = _httpContextAccessor.HttpContext.User;
            var user = await _userManager.FindByNameAsync(userPrincipal.Identity.Name);
            var signOutResult = _signinManager.SignOutAsync();

            await signOutResult;
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpireTime = null;
                await _userManager.UpdateAsync(user);
            }

            return signOutResult.IsCompletedSuccessfully;
        }
    }
}
