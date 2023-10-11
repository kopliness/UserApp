using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataLayer.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLayer.Services.Implementation;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;

    public TokenService(IOptions<JwtOptions> jwtOptions) => _jwtOptions = jwtOptions.Value;

    public Task<string> GenerateTokenAsync(List<Claim> claims)
    {
        var singingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

        var jwt = new JwtSecurityToken(_jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: DateTime.Now.Add(TimeSpan.FromHours(1)),
            notBefore: DateTime.Now,
            signingCredentials: new(singingKey, SecurityAlgorithms.HmacSha256));

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(jwt));
    }
}