using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hipp.Application.Interfaces;
using Hipp.Domain.Entities.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Hipp.Common.Configuration;

namespace Hipp.Application.Services.Auth;

public class TokenService : ITokenService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationInMinutes;

    public TokenService(IConfiguration configuration)
    {
        _secretKey = EnvironmentConfiguration.GetJwtSecret();
        _issuer = EnvironmentConfiguration.GetJwtIssuer();
        _audience = EnvironmentConfiguration.GetJwtAudience();
        _expirationInMinutes = EnvironmentConfiguration.GetJwtExpirationMinutes();

        if (string.IsNullOrEmpty(_secretKey))
            throw new InvalidOperationException("JWT secret key is not configured");
        if (string.IsNullOrEmpty(_issuer))
            throw new InvalidOperationException("JWT issuer is not configured");
        if (string.IsNullOrEmpty(_audience))
            throw new InvalidOperationException("JWT audience is not configured");
    }

    public string GenerateToken(ApplicationUser user, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return false;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GetUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        return jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
    }

    public string GetRoleFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        return jwtToken.Claims.First(x => x.Type == ClaimTypes.Role).Value;
    }
} 