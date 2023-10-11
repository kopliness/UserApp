using System.Security.Claims;
using AutoMapper;
using BusinessLayer.DTO;
using DataLayer.EFCore;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using UserApp.Common.Extensions;

namespace BusinessLayer.Services.Implementation;

public class AuthenticationService : IAuthenticationService
{
    private readonly ITokenService _tokenService;

    private readonly IMapper _mapper;

    private readonly UserAppDbContext _context;

    public AuthenticationService(ITokenService tokenService, IMapper mapper, UserAppDbContext context)
    {
        _tokenService = tokenService;
        _mapper = mapper;
        _context = context;
    }

    public async Task<string?> GetUserTokenAsync(AccountDto accountDto, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var accountModel = _mapper.Map<AccountDto, Account>(accountDto);

        var user = await _context.Accounts.FirstOrDefaultAsync(
            user => user.Login.Equals(accountModel.Login) && user.Password.Equals(accountModel.Password),
            cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Login)
        };

        var token = await _tokenService.GenerateTokenAsync(claims);

        if (token == null)
        {
            throw new NotFoundException("Token not found.");
        }

        return token;
    }
}