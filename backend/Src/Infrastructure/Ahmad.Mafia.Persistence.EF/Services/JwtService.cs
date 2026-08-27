using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ahmad.Mafia.Application.Contract.Identity.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Ahmad.Mafia.Persistence.EF.Services;

public sealed class JwtService(IConfiguration configuration) : IJwtService
{
    // کلید از JWTOptions:Secret می‌آید (همان چیزی که docker-compose ست می‌کند).
    // اگر نبود، بالا نمی‌آییم: توکنِ با کلید پیش‌فرض بدتر از نداشتنِ توکن است.
    private readonly string _secret = configuration["JWTOptions:Secret"]
        ?? throw new InvalidOperationException("JWTOptions:Secret تنظیم نشده است.");

    private readonly string _issuer = configuration["JWTOptions:ValidIssuer"] ?? "Mafia";
    private readonly string _audience = configuration["JWTOptions:ValidAudience"] ?? "MafiaClient";
    private readonly int _expireMinutes =
        int.TryParse(configuration["JWTOptions:TokenExpireMinutes"], out var m) ? m : 60 * 24 * 30;

    public string GenerateToken(long playerId, string mobile, string displayName)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, playerId.ToString()),
            new Claim(ClaimTypes.MobilePhone, mobile),
            new Claim(ClaimTypes.Name, displayName),
        };

        var jwt = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expireMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
