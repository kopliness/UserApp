using BusinessLayer.DTO;
using DataLayer.Entities;
//using PagedList.Core;
using UserApp.Common;
using X.PagedList;

namespace BusinessLayer.Services;

public interface IUserService
{
    public Task<List<UserReadDto>> GetUsers(UserParameters userParameters, CancellationToken cancellationToken = default);
    public Task<UserReadDto> GetUser(Guid id, CancellationToken cancellationToken = default);
    public Task AddRoleToUser(Guid userId, List<int> roleIds, CancellationToken cancellationToken = default);
    public Task DeleteRoleFromUser(Guid userId, List<int> roleIds, CancellationToken cancellationToken = default);
    public Task<UserCreateDto> CreateUser(UserCreateDto newUserCreateDto, CancellationToken cancellationToken = default);
    public Task<UserCreateDto> UpdateUser(Guid id, UserCreateDto updatedUserCreateDto, CancellationToken cancellationToken = default);
    public Task<UserReadDto> DeleteUser(Guid id, CancellationToken cancellationToken = default);
}