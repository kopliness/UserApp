using BusinessLayer.DTO;
using BusinessLayer.Services;
using BusinessLayer.Validation;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace UserApp.API.Controllers
{
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
        [SwaggerOperation(Summary = "Sign in", Description = "Sign in with account credentials")]
        [SwaggerResponse(200, "Returns the user token", typeof(string))]
        [SwaggerResponse(400, "If the request is malformed or the input is invalid")]
        [SwaggerResponse(404, "If a account with the specified login is not found")]
        [SwaggerResponse(500, "If there is an internal server error")]
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
        [SwaggerOperation(Summary = "Sign up", Description = "Register a new user")]
        [SwaggerResponse(200, "Returns the newly registered user", typeof(AccountDto))]
        [SwaggerResponse(400, "If the request is malformed or the input is invalid")]
        [SwaggerResponse(500, "If there is an internal server error")]
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
}
