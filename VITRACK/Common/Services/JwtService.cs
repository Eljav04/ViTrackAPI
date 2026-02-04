
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VITRACK.Api.DTOs.Auth;
using VITRACK.Infrastructure.Entities;


namespace VITRACK.Common.Services
{
    public class JwtService
    {
        public static string GenerateToken(User user, string role, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(user);

            string? secretKey = configuration.GetSection("AppSettings:jwt_secret_key").Value;

            if (string.IsNullOrEmpty(secretKey))
                throw new Exception("Jwt Secret Key not found!");

            byte[] key = Encoding.ASCII.GetBytes(secretKey);

            JwtSecurityTokenHandler tokenHandler = new();

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.Sid, user.Id ?? ""),
                        new Claim(ClaimTypes.Name, user.Firstname ?? ""),
                        new Claim(ClaimTypes.Role, role ?? ""),
                    }),

                Expires = DateTime.UtcNow.AddDays(30),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                    )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }

        public static UserInfo? GetCurrentUserInfo(ClaimsIdentity? identity)
        {
            if (identity is not null)
            {
                var userClaims = identity.Claims;
                return new UserInfo
                {
                    Id = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value,
                    Name = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value,
                    Role = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value,
                };
            }
            return null;
        }
    }
}