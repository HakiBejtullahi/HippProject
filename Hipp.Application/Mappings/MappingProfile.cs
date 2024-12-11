using AutoMapper;
using Hipp.Application.DTOs.Users;
using Hipp.Domain.Entities.Identity;

namespace Hipp.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.Ignore());
    }
} 