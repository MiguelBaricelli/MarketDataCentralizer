using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Models.ApiClientSecurity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MarketDataCentralizer.Application.Services.Authorization
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwt;
        private readonly string _apiKey;

        public AuthService(
            IOptions<JwtSettings> jwtOptions,
            IConfiguration configuration)
        {
            _jwt = jwtOptions.Value;
            _apiKey = configuration["Auth:ApiKey"]!;
        }

        public bool ValidateApiKey(string apiKey)
        {
            return apiKey == _apiKey;
        }

        public JwtTokenModel GenerateToken()
        {
            var claims = new[]
            {
                new Claim("role", "system")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwt.SecretKey)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtTokenModel
            {
                access_token = new JwtSecurityTokenHandler().WriteToken(token),
                token_type = "Bearer",
                expires_in = _jwt.ExpirationMinutes * 60
            };
        }
    }
}
