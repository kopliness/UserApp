using AutoMapper;
using BusinessLayer.DTO;
using DataLayer.EFCore;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserApp.Common;
using UserApp.Common.Exceptions;
using X.PagedList;

namespace BusinessLayer.Services.Implementation;

public class UserService : IUserService
{
    private readonly UserAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(UserAppDbContext context, IMapper mapper, ILogger<UserService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<UserReadDto>> GetUsers(UserParameters userParameters, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _logger.LogInformation("GetUsers method called with parameters: {UserParameters}", userParameters);

        IQueryable<User> users = _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role);

        if (!string.IsNullOrEmpty(userParameters.Name))
            users = users.Where(u => u.Name.ToLower() == userParameters.Name.ToLower());

        if (userParameters.AgeFrom != null)
        {
            users = users.Where(u => u.Age >= userParameters.AgeFrom);
        }

        if (userParameters.AgeTo != null)
        {
            users = users.Where(u => u.Age <= userParameters.AgeTo);

            if (userParameters.AgeTo < userParameters.AgeFrom)
            {
                _logger.LogError("AgeTo must be greater or equal than AgeFrom");
                throw new AgeRangeException("AgeTo must be greater or equal than AgeFrom");
            }
        }

        if (!string.IsNullOrEmpty(userParameters.Email))
        {
            users = users.Where(u => u.Email.ToLower() == userParameters.Email.ToLower());
        }

        if (!string.IsNullOrEmpty(userParameters.RoleName))
        {
            users = users.Where(u =>
                u.UserRoles.Any(ur => ur.Role.Name.ToLower() == userParameters.RoleName.ToLower()));
        }

        if (!string.IsNullOrEmpty(userParameters.OrderBy))
        {
            users = userParameters.OrderBy.ToLower() switch
            {
                "name" => users.OrderBy(u => u.Name),
                "age" => users.OrderBy(u => u.Age),
                "email" => users.OrderBy(u => u.Email),
                "roleName" => users.OrderBy(u => u.UserRoles),
                _ => users
            };
        }

        var pagedUsers = (await users.ToPagedListAsync(userParameters.PageNumber, userParameters.PageSize)).ToList();

        _logger.LogInformation("Returning users matching parameters: {UserParameters}", userParameters);

        return _mapper.Map<List<UserReadDto>>(pagedUsers);
    }

    public async Task<UserReadDto> GetUser(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _logger.LogInformation("Getting user with ID: {id}", id);

        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id,cancellationToken);

        if (user == null)
        {
            _logger.LogError("User with ID: {id} does not exist.", id);
            throw new NotFoundException("User with this ID does not exist.");
        }

        var userDto = _mapper.Map<User, UserReadDto>(user);

        userDto.Roles = user.UserRoles
            .Select(ur => new RoleDto
            {
                Id = ur.Role.Id,
                Name = ur.Role.Name
            })
            .Distinct()
            .ToList();

        _logger.LogInformation("Returning user with ID: {id}", id);

        return userDto;
    }

    public async Task AddRoleToUser(Guid userId, List<int> roleIds, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _logger.LogInformation("AddRoleToUser method called with userId: {UserId} and roleIds: {RoleIds}", userId,
            roleIds);

        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            _logger.LogError("User with ID: {userId} does not exist.", userId);
            throw new NotFoundException("User with this ID does not exist.");
        }

        if (roleIds.Count == 0)
        {
            _logger.LogError("Role list is empty.");
            throw new IncorrectRolesException("Role list is empty.");
        }

        var assignedRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();

        roleIds.RemoveAll(id => assignedRoleIds.Contains(id));

        var distinctRoleIds = roleIds.Distinct().ToList();
        if (distinctRoleIds.Count != roleIds.Count)
        {
            _logger.LogError("Role list contains duplicates.");
            throw new IncorrectRolesException("Role list contains duplicates.");
        }

        var existingRoles = await _context.Roles.Where(r => distinctRoleIds.Contains(r.Id)).ToListAsync();
        if (existingRoles.Count != distinctRoleIds.Count)
        {
            var missingRoleId = distinctRoleIds.First(id => !existingRoles.Any(r => r.Id == id));
            _logger.LogError("Role with ID {MissingRoleId} does not exist.", missingRoleId);
            throw new NotFoundException($"Role with ID {missingRoleId} does not exist.");
        }

        _context.UserRoles.AddRange(roleIds.Select(x => new UserRole
        {
            UserId = userId,
            RoleId = x
        }));

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Roles {RoleIds} successfully added to user with ID {UserId}.", roleIds, userId);
    }

    public async Task DeleteRoleFromUser(Guid userId, List<int> roleIds, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _logger.LogInformation("DeleteRoleFromUser method called with userId: {UserId} and roleIds: {RoleIds}", userId,
            roleIds);

        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId,cancellationToken);

        if (user == null)
        {
            _logger.LogError("User with ID {UserId} does not exist.", userId);
            throw new NotFoundException("User with this ID does not exist.");
        }

        if (roleIds.Count == 0)
        {
            _logger.LogError("Role list is empty.");
            throw new IncorrectRolesException("Role list is empty.");
        }

        var existingRoles = await _context.Roles.Where(r => roleIds.Contains(r.Id)).ToListAsync();
        if (existingRoles.Count != roleIds.Count)
        {
            var missingRoleId = roleIds.First(id => !existingRoles.Any(r => r.Id == id));
            _logger.LogError("Role with ID {MissingRoleId} does not exist.", missingRoleId);
            throw new NotFoundException($"Role with ID {missingRoleId} does not exist.");
        }

        if (user.UserRoles?.Any(ur => roleIds.Contains(ur.Role.Id)) == false)
        {
            _logger.LogError("User has no roles that match the given roles.");
            throw new IncorrectRolesException("User has no roles that match the given roles.");
        }

        var rolesToRemove = user.UserRoles.Where(ur => ur.Role != null && roleIds.Contains(ur.Role.Id)).ToList();

        if (rolesToRemove.Count > 0)
        {
            _context.UserRoles.RemoveRange(rolesToRemove);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Roles {RoleIds} successfully removed from user with ID {UserId}.", roleIds, userId);
        }
    }

    public async Task<UserCreateDto> CreateUser(UserCreateDto newUserCreateDto, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _logger.LogInformation("CreateUser method called with user data: {NewUserData}", newUserCreateDto);

        var userExists = await _context.Users.AnyAsync(u => u.Email == newUserCreateDto.Email);
        if (userExists)
        {
            _logger.LogError("A user with this email already exists.");
            throw new UserExistsException("A user with this email already exists.");
        }

        var user = _mapper.Map<UserCreateDto, User>(newUserCreateDto);
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        await AddRoleToUser(user.Id, newUserCreateDto.Roles, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User with ID {UserId} was successfully created.", user.Id);

        return newUserCreateDto;
    }

    public async Task<UserCreateDto> UpdateUser(Guid id, UserCreateDto updatedUserCreateDto, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _logger.LogInformation(
            "UpdateUser method called with user ID: {UserId} and updated user data: {UpdatedUserData}", id,
            updatedUserCreateDto);

        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user == null)
        {
            _logger.LogError("User with ID {id} does not exist.", id);
            throw new NotFoundException("User with this ID does not exist.");
        }

        user.Name = updatedUserCreateDto.Name;
        user.Email = updatedUserCreateDto.Email;
        user.Age = updatedUserCreateDto.Age;

        var existingUserWithSameEmail =
            await _context.Users.FirstOrDefaultAsync(u => u.Email == updatedUserCreateDto.Email, cancellationToken);
        if (existingUserWithSameEmail != null && existingUserWithSameEmail.Id != id)
        {
            _logger.LogError("Email {Email} already belongs to another user.", updatedUserCreateDto.Email);
            throw new UserExistsException("Email already belongs to another user.");
        }

        await AddRoleToUser(user.Id, updatedUserCreateDto.Roles);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User with ID {UserId} was successfully updated.", user.Id);

        return _mapper.Map<UserCreateDto>(user);
    }

    public async Task<UserReadDto> DeleteUser(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _logger.LogInformation("DeleteUser method called with user ID: {UserId}", id);

        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user == null)
        {
            _logger.LogError("User with ID {id} does not exist.", id);
            throw new NotFoundException("User with this ID does not exist.");
        }

        var userDto = _mapper.Map<User, UserReadDto>(user);

        userDto.Roles = user.UserRoles
            .Select(ur => new RoleDto
            {
                Id = ur.Role.Id,
                Name = ur.Role.Name
            })
            .Distinct()
            .ToList();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User with ID {UserId} was successfully deleted.", user.Id);

        return userDto;
    }
}