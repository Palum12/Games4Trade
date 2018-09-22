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

            CreateMap<Genre, GenreCreateOrUpdateDto>();
            CreateMap<GenreCreateOrUpdateDto, Genre>().ForMember(s => s.Id, opt => opt.Ignore());
            CreateMap<Genre, GenreGetDto>().ReverseMap();

            CreateMap<Models.System, SystemGetDto>().ReverseMap();
            CreateMap<Models.System, SystemCreateOrUpdateDto>();
            CreateMap<SystemCreateOrUpdateDto, Models.System>().ForMember(s => s.Id, opt => opt.Ignore());

            CreateMap<AnnouncementSaveDto, Announcement>();

            CreateMap<Announcement, AnnouncementGetDto>()
                .ForMember(a => a.Author,
                    opt => opt.MapFrom(src => src.User.Login));

        }
    }
}
