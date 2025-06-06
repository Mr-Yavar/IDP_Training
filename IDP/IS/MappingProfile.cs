using AutoMapper;
using IS.Entities;
using IS.Entities.ViewModels;

namespace IS;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserRegistrationModel, ApplicationUser>()
            .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
    }
}
