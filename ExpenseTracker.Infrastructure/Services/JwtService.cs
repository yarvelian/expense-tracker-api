using System.Security.Claims;
using System.Text;
using ExpenseTracker.Application.Abstractions;
using ExpenseTracker.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTracker.Infrastructure.Services;

internal sealed class JwtService : IJwtService
{
    private readonly JwtOptions _options;
    private readonly JsonWebTokenHandler  _tokenHandler;
    
    public JwtService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        _tokenHandler = new JsonWebTokenHandler();
    }

    public string GenerateToken(Guid userId, string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            ]),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes),
            SigningCredentials = credentials
        };
        
        return _tokenHandler.CreateToken(descriptor);
    }
}
