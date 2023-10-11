using BusinessLayer.DTO;
using BusinessLayer.Services;
using BusinessLayer.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserApp.Common;

namespace UserApp.API.Controllers;

[ApiController]
[Authorize]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly UserValidator _userValidator;
    private readonly UserParametersValidator _userParametersValidator;

    public UserController(IUserService userService, UserValidator userValidator,
        UserParametersValidator userParametersValidator)
    {
        _userService = userService;
        _userValidator = userValidator;
        _userParametersValidator = userParametersValidator;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all users", Description = "Get a list of all users")]
    [SwaggerResponse(200, "Returns a list of UserReadDto", typeof(List<UserReadDto>))]
    [SwaggerResponse(400, "If incorrect page size")]
    [SwaggerResponse(401, "If user is not authenticated")]
    [SwaggerResponse(422, "If incorrect age range")]
    [SwaggerResponse(500, "If there is an internal server error")]
    public async Task<ActionResult<List<UserReadDto>>> GetUsers([FromQuery] UserParameters userParameters, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var validationResult = _userParametersValidator.Validate(userParameters);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var users = await _userService.GetUsers(userParameters);

        return users;
    }


    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get a user by ID", Description = "Get a specific user by ID")]
    [SwaggerResponse(200, "Returns a user with the specified ID", typeof(UserReadDto))]
    [SwaggerResponse(404, "If a user with the specified ID is not found")]
    [SwaggerResponse(500, "If there is an internal server error")]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var user = await _userService.GetUser(id);

        return Ok(user);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new user", Description = "Create a new user")]
    [SwaggerResponse(200, "Returns the newly created user", typeof(UserReadDto))]
    [SwaggerResponse(400, "If the request is malformed or the input is invalid")]
    [SwaggerResponse(500, "If there is an internal server error")]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto newUserCreateDto, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var validationResult = _userValidator.Validate(newUserCreateDto);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var user = await _userService.CreateUser(newUserCreateDto);
        
        return Ok(user);
    }

    [HttpPost("set-roles")]
    [SwaggerOperation(Summary = "Add roles to user", Description = "Add roles to a specific user")]
    [SwaggerResponse(200, "Roles added successfully")]
    [SwaggerResponse(400, "If the request is malformed or the input is invalid")]
    [SwaggerResponse(500, "If there is an internal server error")]
    public async Task<IActionResult> AddRolesToUser([FromForm] UserRoleDto userRoleDto, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await _userService.AddRoleToUser(userRoleDto.UserId, userRoleDto.RoleIds);
        
        return Ok();
    }

    [HttpDelete("delete-roles")]
    [SwaggerOperation(Summary = "Delete roles from user", Description = "Delete roles from a specific user")]
    [SwaggerResponse(200, "Roles deleted successfully")]
    [SwaggerResponse(400, "If the request is malformed or the input is invalid")]
    [SwaggerResponse(500, "If there is an internal server error")]
    public async Task<IActionResult> DeleteRolesFromUser([FromForm] UserRoleDto userRoleDto, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await _userService.DeleteRoleFromUser(userRoleDto.UserId, userRoleDto.RoleIds);
        
        return Ok();
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update a user", Description = "Update a specific user(deletes old roles)")]
    [SwaggerResponse(200, "Returns the updated user", typeof(UserReadDto))]
    [SwaggerResponse(400, "If the request is malformed or the input is invalid")]
    [SwaggerResponse(404, "If a user with the specified ID is not found")]
    [SwaggerResponse(500, "If there is an internal server error")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserCreateDto newUserCreateDto, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var validationResult = _userValidator.Validate(newUserCreateDto);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var user = await _userService.UpdateUser(id, newUserCreateDto);

        return Ok(user);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete a user", Description = "Delete a specific user")]
    [SwaggerResponse(200, "User deleted successfully")]
    [SwaggerResponse(404, "If a user with the specified ID is not found")]
    [SwaggerResponse(500, "If there is an internal server error")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var user = await _userService.DeleteUser(id);

        return Ok(user);
    }
}