﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DotNet6.JwtTokenExample.Controllers.Login
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public IActionResult Post([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Invalid client request");
            }

            // Implement your logic for verifying username and password
            if (IsValidUser(loginRequest.Username, loginRequest.Password))
            {
                var token = GenerateJwtToken(loginRequest);
                return Ok(new { Token = token });
            }
            else
            {
                return Unauthorized();
            }
        }

        private bool IsValidUser(string username, string password)
        {
            // Your logic to validate the username and password
            // This is just a placeholder for actual user validation logic
            return username == "validUser" && password == "validPassword";
        }

        private string GenerateJwtToken(LoginRequest reqModel)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var clamis = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, reqModel.Username),
                new Claim(JwtRegisteredClaimNames.CHash, reqModel.Password),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims: clamis,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}