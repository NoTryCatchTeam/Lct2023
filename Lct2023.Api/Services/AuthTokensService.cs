using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Lct2023.Api.Definitions.Constants;
using Lct2023.Api.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Lct2023.Api.Services;

public class AuthTokensService : IAuthTokensService
{
    private readonly IConfiguration _configuration;

    public AuthTokensService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateAccessToken(int userId)
    {
        var authClaims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, userId.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>(ConfigurationConstants.Secrets.JWT_ISSUER),
            audience: _configuration.GetValue<string>(ConfigurationConstants.Secrets.JWT_AUDIENCE),
            expires: DateTime.Now.AddDays(int.Parse(_configuration.GetValue<string>(ConfigurationConstants.Secrets.JWT_ACCESS_TOKEN_EXPIRES_IN_DAYS))),
            claims: authClaims,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>(ConfigurationConstants.Secrets.JWT_SECRET))),
                SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (string Token, DateTimeOffset ExpiresAt) CreateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return (Convert.ToBase64String(randomNumber),
            DateTimeOffset.UtcNow.AddDays(int.Parse(_configuration.GetValue<string>(ConfigurationConstants.Secrets.JWT_REFRESH_TOKEN_EXPIRES_IN_DAYS))));
    }

    public bool ValidateAccessToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = _configuration.GetValue<string>(ConfigurationConstants.Secrets.JWT_AUDIENCE),
            ValidIssuer = _configuration.GetValue<string>(ConfigurationConstants.Secrets.JWT_ISSUER),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>(ConfigurationConstants.Secrets.JWT_SECRET))),
            ValidateLifetime = false,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var validateTokenPrincipal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return validateTokenPrincipal != null;
    }
}
