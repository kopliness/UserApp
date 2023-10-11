using BusinessLayer.DTO;
using DataLayer.Entities;

namespace BusinessLayer.Services;

public interface IAccountService
{
    Task<Account> RegisterUserAsync(AccountDto accountDto, CancellationToken cancellationToken = default);
}