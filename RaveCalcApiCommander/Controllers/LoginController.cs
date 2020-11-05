using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RaveCalcApiCommander.Models;

namespace RaveCalcApiCommander.Controllers
{
    [Route("apiv2/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Login([FromBody] User userInfo)
        {
            if(IsAuthUser(userInfo))
            {
                var token = CreateToken(userInfo);
                return new OkObjectResult(new ResponseResult<UserToken>()
                {
                    error = false,
                    result = new UserToken()
                    {
                        UserName = userInfo.UserName,
                        Token = token
                    }
                });
            }
            return new UnauthorizedObjectResult(new ResponseResult<UnauthorizedResult>()
            {
                error = true,
                result = Unauthorized()
            });
        }

        private bool IsAuthUser(User userInfo)
        {
            var userName = _configuration.GetSection("AutorizationSettings:User").Value;
            var passwordSalt = Convert.FromBase64String(_configuration.GetSection("AutorizationSettings:Salt").Value);
            var passwordHash = Convert.FromBase64String(_configuration.GetSection("AutorizationSettings:PasswordHash").Value);

            if (userName != userInfo.UserName) return false;

            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(userInfo.Password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private string CreateToken(User userInfo)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userInfo.UserName),
            };
            string tokenKey = _configuration.GetSection("AutorizationSettings:TokenKey").Value;
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials:creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}