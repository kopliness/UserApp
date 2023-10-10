using AutoMapper;
using BusinessLayer.DTO;
using DataLayer.Entities;

namespace BusinessLayer.Mapping;

public class RoleMappingProfile : Profile
{
    public RoleMappingProfile() => CreateMap<UserRole, RoleDto>()
        .ForPath(x => x.Name, opt => opt.MapFrom(src => src.Role.Name))
        .ReverseMap();
}