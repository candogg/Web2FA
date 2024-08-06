using AutoMapper;
using Web2FA.Backend.Model.Models.Derived;
using Web2FA.Backend.Shared.Dto.Derived;

namespace Web2FA.Backend.Shared.MapperConfig
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
