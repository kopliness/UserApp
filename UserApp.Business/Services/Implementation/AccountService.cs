using AutoMapper;
using BusinessLayer.DTO;
using DataLayer.EFCore;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserApp.Common.Extensions;

namespace BusinessLayer.Services.Implementation;

public class AccountService : IAccountService
{
    private readonly UserAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<AccountService> _logger;

    public AccountService(UserAppDbContext context, IMapper mapper, ILogger<AccountService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<Account> RegisterUserAsync(AccountDto accountDto, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _logger.LogInformation("Register started.");
        
        var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Login == accountDto.Login, cancellationToken);
        if (existingAccount != null)
        {
            _logger.LogError("Account with the specified login already exists");
            throw new AccountExistsException("Account with the specified login already exists.");
        }

        var accountModel = _mapper.Map<AccountDto, Account>(accountDto);

        var account = await _context.Accounts.AddAsync(accountModel, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("New user registered {EntityLogin}", account.Entity.Login);

        return account.Entity;
    }
}