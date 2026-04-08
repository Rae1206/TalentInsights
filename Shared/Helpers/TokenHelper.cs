using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JwtCfg = Shared.Constants;

namespace Shared.Helpers;

public static class TokenHelper
{
    public static string GenerateJwtToken(
        Guid userId,
        string fullName,
        string role,
        string secretKey)
    {
        var key = Encoding.UTF8.GetBytes(secretKey);
        var expiresAt = DateTime.UtcNow.AddMinutes(JwtCfg.JwtConstants.ExpirationMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, fullName),
            new Claim("role", role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: JwtCfg.JwtConstants.Issuer,
            audience: JwtCfg.JwtConstants.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
