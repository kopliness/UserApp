using BusinessLayer.DTO;
using DataLayer.Entities;
//using PagedList.Core;
using UserApp.Common;
using X.PagedList;

namespace BusinessLayer.Services;

public interface IUserService
{
    public Task<List<UserReadDto>> GetUsers(UserParameters userParameters);
    public Task<UserReadDto> GetUser(Guid id);
    public Task AddRoleToUser(Guid userId, List<int> roleIds);
    public Task DeleteRoleFromUser(Guid userId, List<int> roleIds);
    public Task<UserCreateDto> CreateUser(UserCreateDto newUserCreateDto);
    public Task<UserCreateDto> UpdateUser(Guid id, UserCreateDto updatedUserCreateDto);
    public Task<UserReadDto> DeleteUser(Guid id);
}