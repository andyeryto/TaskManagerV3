using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using Microsoft.Extensions.Configuration;


namespace TaskManager.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : Controller
    {

        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new { message = "User API is working" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            // Hash the password before storing
            // user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            await _userRepository.AddAsync(user);
            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            // Validate user exists in database
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            //if (existingUser == null || !BCrypt.Net.BCrypt.Verify(user.PasswordHash, existingUser.PasswordHash))
            if (existingUser == null || existingUser.PasswordHash != user.PasswordHash)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Generate JWT token
            var token = GenerateJwtToken(existingUser);
            return Ok(new { token });
        }


        private string GenerateJwtToken(User user)
        {
            var securityKeyConf = _configuration["JwtSettings:SecretKey"];
            var issuerConf = _configuration["JwtSettings:Issuer"];
            var audienceConf = _configuration["JwtSettings:Audience"];
            var accessTokenExpirationConf = _configuration["JwtSettings:AccessTokenExpiration"];

            if (string.IsNullOrEmpty(issuerConf))
            {
                throw new InvalidOperationException("JWT Issuer is missing or empty.");
            }
            if (string.IsNullOrEmpty(audienceConf))
            {
                throw new InvalidOperationException("JWT Audience is missing or empty.");
            }
            if (string.IsNullOrEmpty(securityKeyConf))
            {
                throw new InvalidOperationException("JWT SecretKey is missing or empty.");
            }
            if (string.IsNullOrEmpty(accessTokenExpirationConf))
            {
                throw new InvalidOperationException("JWT access token expiration time is missing or empty.");
            }


            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKeyConf));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // In next line there is a variable called issuer that comes from the appsettings.json file

            var token = new JwtSecurityToken(
                issuer: issuerConf,
                audience: audienceConf,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(double.TryParse(accessTokenExpirationConf, out double expirationHours) ? expirationHours : 1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
