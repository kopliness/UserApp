using AutoMapper;
using BusinessLayer.DTO;
using DataLayer.Entities;
using X.PagedList;

namespace BusinessLayer.Mapping;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<Role, RoleDto>().ReverseMap();
        CreateMap<User, UserCreateDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role)))
            .ReverseMap();
        CreateMap<User, UserReadDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role)))
            .ReverseMap();
        CreateMap<PagedList<User>, PagedList<UserReadDto>>()
            .ReverseMap();
    }
}