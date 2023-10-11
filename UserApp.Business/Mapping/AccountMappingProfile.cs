using AutoMapper;
using BusinessLayer.DTO;
using DataLayer.Entities;

namespace BusinessLayer.Mapping;

public class AccountMappingProfile : Profile
{
    public AccountMappingProfile() => CreateMap<Account, AccountDto>()
        .ReverseMap();
}