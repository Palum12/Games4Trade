using AutoMapper.Configuration;
using Games4Trade.Dtos;
using Games4Trade.Models;

namespace Games4Trade
{
    public class MappingProfile : MapperConfigurationExpression
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserLoginDto>().ReverseMap();
            CreateMap<User, UserRegisterDto>().ReverseMap();
        }
    }
}
