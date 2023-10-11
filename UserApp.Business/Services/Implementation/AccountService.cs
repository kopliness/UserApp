using AutoMapper;
using BusinessLayer.DTO;
using DataLayer.EFCore;
using DataLayer.Entities;
using Microsoft.Extensions.Logging;

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
        
        var accountModel = _mapper.Map<AccountDto, Account>(accountDto);
        
        var account = await _context.Accounts.AddAsync(accountModel, cancellationToken);
        
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("New user registered {EntityLogin}", account.Entity.Login);

        return account.Entity;
    }
}