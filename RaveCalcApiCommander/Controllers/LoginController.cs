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
using RaveCalcApiCommander.Abstraction;
using RaveCalcApiCommander.Models;
using TimeZoneCorrectorLibrary.Abstraction;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace RaveCalcApiCommander.Controllers
{
    [Route("apiv2")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IConfiguration configuration, IUserRepository userRepository, ILogger<LoginController> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] User userInfo)
        {
            var user = await _userRepository.IsAuthentificate(userInfo.UserName, userInfo.Password);

            if (user != null)
            {
                var jwtToken = CreateJwtToken(user);
                var refreshToken = CreateRefreshToken(ipAddress());
                if(!await _userRepository.SaveRefreshToken(user, refreshToken))
                {
                    return new UnauthorizedObjectResult(new ResponseError()
                    {
                        error = true,
                        message = "Wrong password or user not exist"
                    });
                }
                return Ok(new ResponseResult<UserToken>()
                {
                    error = false,
                    result = new UserToken()
                    {
                        UserName = user.UserName,
                        tokens = new Tokens()
                        {
                            jwtToken = jwtToken,
                            refreshToken = refreshToken
                        }
                    }
                });
            }

            return new UnauthorizedObjectResult(new ResponseError()
            {
                error = true,
                message = "Wrong password or user not exist"
            });
        }

        [HttpPost]
        [Route("logout_command")]
        public async Task<IActionResult> Logout([FromBody] RefreshToken refreshToken)
        {
            if (!await _userRepository.RemoveRefreshToken(refreshToken))
            {
                _logger.LogError("Error remove refresh token", refreshToken);  
            }
            return Ok(new ResponseResult<string>()
            {
                error = false,
                result = "Logout success"
            });
        }

        [HttpPost]
        [Route("refresh_token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshToken refreshToken)
        {
            var user = await _userRepository.CheckRefreshToken(refreshToken);
            if (user != null)
            {
                var jwtToken = CreateJwtToken(user);

                return Ok(new ResponseResult<Tokens>()
                {
                    error = false,
                    result = new Tokens()
                    {
                        jwtToken = jwtToken,
                        refreshToken = refreshToken
                    }
                });
            }
            return Ok(new ResponseResult<Tokens>()
            {
                error = false,
                result = null
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

        private string CreateJwtToken(User userInfo)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userInfo.UserName),
                new Claim(ClaimTypes.Role, userInfo.Role.ToString())
            };
            string tokenKey = _configuration.GetSection("AutorizationSettings:TokenKey").Value;
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = jwtTokenHandler.CreateJwtSecurityToken(
                null,
                null,
                new ClaimsIdentity(claims),
                null,
                DateTime.Now.AddMinutes(1),
                null,
                creds);
            return jwtTokenHandler.WriteToken(token);
        }

        private RefreshToken CreateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}