using BusinessLayer.DTO;
using BusinessLayer.Services;
using BusinessLayer.Validation;
using Microsoft.AspNetCore.Mvc;

namespace UserApp.API.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    private readonly IAccountService _accountService;

    private readonly AccountValidator _accountValidator;

    public AccountController(IAuthenticationService authenticationService, IAccountService accountService,
        AccountValidator accountValidator)
    {
        _authenticationService = authenticationService;
        _accountService = accountService;
        _accountValidator = accountValidator;
    }

    [HttpGet("sign-in")]
    public async Task<IActionResult> GetToken([FromQuery] AccountDto accountDto,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validationResult = _accountValidator.Validate(accountDto);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var token = await _authenticationService.GetUserTokenAsync(accountDto, cancellationToken);

        return Ok(token);
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> RegisterUser(AccountDto accountDto, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validationResult = _accountValidator.Validate(accountDto);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        var account = await _accountService.RegisterUserAsync(accountDto, cancellationToken);

        return Ok(account);
    }
}