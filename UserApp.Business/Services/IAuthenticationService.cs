using BusinessLayer.DTO;

namespace BusinessLayer.Services;

public interface IAuthenticationService
{
    Task<string?> GetUserTokenAsync(AccountDto accountDto, CancellationToken cancellationToken = default);
}