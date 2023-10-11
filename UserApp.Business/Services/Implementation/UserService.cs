using AutoMapper;
using BusinessLayer.DTO;
using DataLayer.EFCore;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using UserApp.Common;
using UserApp.Common.Extensions;
using X.PagedList;

namespace BusinessLayer.Services.Implementation;

public class UserService : IUserService
{
    private readonly UserAppDbContext _context;
    private readonly IMapper _mapper;

    public UserService(UserAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<UserReadDto>> GetUsers(UserParameters userParameters)
    {
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
                throw new AgeRangeException("AgeTo must be greater or equal than AgeFrom");
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

        return _mapper.Map<List<UserReadDto>>(pagedUsers);
    }

    public async Task<UserReadDto> GetUser(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id) ?? throw new NotFoundException("User with this ID does not exist.");

        var userDto = _mapper.Map<User, UserReadDto>(user);

        userDto.Roles = user.UserRoles
            .Select(ur => new RoleDto
            {
                Id = ur.Role.Id,
                Name = ur.Role.Name
            })
            .Distinct()
            .ToList();

        return userDto;
    }

    public async Task AddRoleToUser(Guid userId, List<int> roleIds)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId)?? throw new NotFoundException("User with this ID does not exist.");

        if (roleIds.Count == 0)
            throw new IncorrectRolesException("Role list is empty.");

        var assignedRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();

        roleIds.RemoveAll(id => assignedRoleIds.Contains(id));

        var distinctRoleIds = roleIds.Distinct().ToList();
        if (distinctRoleIds.Count != roleIds.Count)
        {
            throw new IncorrectRolesException("Role list contains duplicates.");
        }

        var existingRoles = await _context.Roles.Where(r => distinctRoleIds.Contains(r.Id)).ToListAsync();
        if (existingRoles.Count != distinctRoleIds.Count)
        {
            var missingRoleId = distinctRoleIds.First(id => !existingRoles.Any(r => r.Id == id));
            throw new IncorrectRolesException($"Role with ID {missingRoleId} does not exist.");
        }

        _context.UserRoles.AddRange(roleIds.Select(x=> new UserRole
        {
            UserId = userId,
            RoleId = x
        }));

        await _context.SaveChangesAsync();
    }

    public async Task DeleteRoleFromUser(Guid userId, List<int> roleIds)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId)?? throw new NotFoundException("User with this ID does not exist.");

        if (roleIds.Count == 0)
            throw new IncorrectRolesException("Role list is empty.");

        var existingRoles = await _context.Roles.Where(r => roleIds.Contains(r.Id)).ToListAsync();
        if (existingRoles.Count != roleIds.Count)
        {
            var missingRoleId = roleIds.First(id => !existingRoles.Any(r => r.Id == id));
            throw new IncorrectRolesException($"Role with ID {missingRoleId} does not exist.");
        }

        if (user.UserRoles?.Any(ur => roleIds.Contains(ur.Role.Id)) == false)
            throw new IncorrectRolesException("User has no roles that match the given roles.");

        var rolesToRemove = user.UserRoles.Where(ur => ur.Role != null && roleIds.Contains(ur.Role.Id)).ToList();

        if (rolesToRemove.Count > 0)
        {
            _context.UserRoles.RemoveRange(rolesToRemove);
            await _context.SaveChangesAsync();
        }
    }


    public async Task<UserCreateDto> CreateUser(UserCreateDto newUserCreateDto)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Email == newUserCreateDto.Email);
        if (userExists)
            throw new UserExistsException("A user with this email already exists.");

        var user = _mapper.Map<UserCreateDto, User>(newUserCreateDto);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await AddRoleToUser(user.Id, newUserCreateDto.Roles);

        await _context.SaveChangesAsync();

        return newUserCreateDto;
    }

    public async Task<UserCreateDto> UpdateUser(Guid id, UserCreateDto updatedUserCreateDto)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id)?? throw new NotFoundException("User with this ID does not exist.");

        user.Name = updatedUserCreateDto.Name;
        user.Email = updatedUserCreateDto.Email;
        user.Age = updatedUserCreateDto.Age;

        _context.UserRoles.RemoveRange(user.UserRoles);

        await AddRoleToUser(user.Id, updatedUserCreateDto.Roles);

        await _context.SaveChangesAsync();

        return updatedUserCreateDto;
    }


    public async Task<UserReadDto> DeleteUser(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id) ?? throw new NotFoundException("User with this ID does not exist.");

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
        await _context.SaveChangesAsync();

        return userDto;
    }
}