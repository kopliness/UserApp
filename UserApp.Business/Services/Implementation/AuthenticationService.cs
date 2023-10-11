using System.Security.Claims;
using AutoMapper;
using BusinessLayer.DTO;
using DataLayer.EFCore;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserApp.Common.Extensions;

namespace BusinessLayer.Services.Implementation;

public class AuthenticationService : IAuthenticationService
{
    private readonly ITokenService _tokenService;

    private readonly IMapper _mapper;

    private readonly UserAppDbContext _context;

    private readonly ILogger<AccountService> _logger;

    public AuthenticationService(ITokenService tokenService, IMapper mapper, UserAppDbContext context, ILogger<AccountService> logger)
    {
        _tokenService = tokenService;
        _mapper = mapper;
        _context = context;
        _logger = logger;
    }

    public async Task<string?> GetUserTokenAsync(AccountDto accountDto, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.LogInformation("Getting token.");
        var accountModel = _mapper.Map<AccountDto, Account>(accountDto);

        var user = await _context.Accounts.FirstOrDefaultAsync(
            user => user.Login.Equals(accountModel.Login) && user.Password.Equals(accountModel.Password),
            cancellationToken);

        if (user == null)
        {
            _logger.LogError("Account not found.");
            throw new NotFoundException("Account not found.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Login)
        };

        var token = await _tokenService.GenerateTokenAsync(claims);

        if (token == null)
        {     
            _logger.LogError("Token not found.");
            throw new NotFoundException("Token not found.");
        }
        _logger.LogInformation("Return token.");
        
        return token;
    }
}