// In Services/TokenService.cs

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HRWorkForceSystemBackend.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // REPLACEMENT METHOD: This method now accepts a dynamic user object.
        // This is the key change that will fix your problem.
        public string CreateToken(dynamic user, string role)
        {
            // This list of claims is now more complete.
            var claims = new List<Claim>
            {
                // This is the CRITICAL claim your TrainingProgramController needs.
                // It gets the user's ID and stores it as the "NameIdentifier".
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                // This is the CRITICAL claim for [Authorize(Roles = "...")]
                new Claim(ClaimTypes.Role, role),

                // These are useful for the frontend (e.g., jwtDecode).
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("name", $"{user.FirstName} {user.LastName}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8), // Or your preferred expiration
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}