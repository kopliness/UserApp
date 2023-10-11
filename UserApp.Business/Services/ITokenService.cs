using System.Security.Claims;

namespace BusinessLayer.Services;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(List<Claim> claims);
}