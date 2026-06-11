using Chatry.Data;
using Chatry.DTOs.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Chatry.Services
{
    public class JwtService
    {

        private readonly IConfiguration _configuration;
        private readonly ChatryDbContext _context;


        public JwtService(IConfiguration configuration , ChatryDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public LoginResponseModel Authenticate(LoginRequestModel request)
        {
            

            // 4. Konfigürasyondan JWT ayarlarını oku
            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var key = _configuration["JwtConfig:Key"];
            var tokenValidityMins = _configuration.GetValue<int>("JwtConfig:TokenValidityMins");

            // 5. Token bitiş zamanını hesapla
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);


            // 6. Token içeriğini ve imzalama yöntemini tanımla
            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name , request.Username),
            new Claim(ClaimTypes.NameIdentifier , request.UserID.ToString())
        }),
                Expires = tokenExpiryTimeStamp,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256Signature),
            };

            // 7. Token'ı oluştur ve string'e dönüştür
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            // 8. Sonucu model olarak geri döndür
            return new LoginResponseModel
            {
                AccesToken = accessToken,
                Username = request.Username,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds
            };
        }



    }
}
