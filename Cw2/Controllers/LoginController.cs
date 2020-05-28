using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cw2.DTO;
using Cw2.DTO.Requests;
using Cw2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Cw2.Controllers

{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        //  private const string ConString = "Data Source=db-mssql;Initial Catalog=s16535;Integrated Security=True";
        private readonly SqlServerStudentDbService _service;
        private readonly IConfiguration _configuration;

        public LoginController(SqlServerStudentDbService service, IConfiguration configuration)
        {
            _configuration = configuration;
            _service = service;
        }

        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {
/*            Response isAuth;
            try
            {
                isAuth = _service.checkCredentials(request);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

            if (!isAuth.Type.Equals("200 Ok"))
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "user"),
                new Claim(ClaimTypes.Role, "employee")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "s16535",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: creds
            );

            return Ok(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = Guid.NewGuid()
            });*/

            // ===========================================================================

            Response isAuth;
            try
            {
                isAuth = _service.CheckCredentials(request);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

            if (!isAuth.Type.Equals("200 Ok"))
            {
                return Unauthorized();
            }

            var tokens = GenerateAuthToken("employee");

            try
            {
                _service.SaveToken(new SaveTokenRequest
                {
                    IndexNumber = request.Login,
                    Token = tokens.refreshToken.ToString()
                });
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

            return Ok(new
            {
                tokens.accessToken,
                tokens.refreshToken
            });
        }

        [HttpPost("refresh-token/{refToken}")]
        public IActionResult RefreshToken(string token)
        {
            bool isAuth;
            try
            {
                isAuth = _service.IsTokenAuth(token);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
            if (!isAuth)
            {
                return Unauthorized();
            }

            var tokens = GenerateAuthToken("employee");

            try
            {
                _service.SaveToken(token, tokens.refreshToken.ToString());
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }

            return Ok(new
            {
                tokens.accessToken,
                tokens.refreshToken
            });
        }

        private (string accessToken, Guid refreshToken) GenerateAuthToken(string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "user"),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
            (
                issuer: "s16535",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: creds
            );

            return (
                new JwtSecurityTokenHandler().WriteToken(token),
                Guid.NewGuid()
            );
        }
    }
} 