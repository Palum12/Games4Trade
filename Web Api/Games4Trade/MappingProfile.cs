using System;
using System.Linq;
using AutoMapper;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Models;
using Console = Games4TradeAPI.Models.Console;

namespace Games4TradeAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserLoginDto>().ReverseMap();
            CreateMap<User, UserRegisterDto>().ReverseMap();
            CreateMap<User, UserSimpleDto>();

            CreateMap<Genre, GenreCreateOrUpdateDto>();
            CreateMap<GenreCreateOrUpdateDto, Genre>().ForMember(s => s.Id, opt => opt.Ignore());
            CreateMap<Genre, GenreDto>().ReverseMap();

            CreateMap<Models.System, SystemDto>().ReverseMap();
            CreateMap<Models.System, SystemCreateOrUpdateDto>();
            CreateMap<SystemCreateOrUpdateDto, Models.System>().ForMember(s => s.Id, opt => opt.Ignore());

            CreateMap<AnnouncementSaveDto, Announcement>();
            CreateMap<Announcement, AnnouncementGetDto>()
                .ForMember(a => a.Author,
                    opt => opt.MapFrom(src => src.User.Login));

            CreateMap<Message, MessageDto>();

            CreateMap<Photo, PhotoDto>();
            CreateMap<Region, RegionDto>().ReverseMap();
            CreateMap<State, StateDto>().ReverseMap();

            CreateMap<Advertisement, AdvertisementWithoutItemDto>()
                .ForMember(a => a.MainPhotoId, opt =>
                {
                    opt.PreCondition(a => a.Photos != null && a.Photos.Any());
                    opt.MapFrom(a => a.Photos.FirstOrDefault().Id);
                });

            CreateMap<AdvertisementSaveDto, Advertisement>()
                .ForMember(a => a.DateCreated, opt => opt.MapFrom(a => DateTime.UtcNow))
                .ForMember(a => a.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(a => a.ExchangeActive, opt => opt.MapFrom(a => a.ExchangeActive))
                .ForMember(a => a.UserId, opt => opt.Ignore());
            CreateMap<AdvertisementSaveDto, Game>()
                .ForMember(g => g.GameRegionId, opt => opt.MapFrom(a => a.RegionId))
                .ForMember(g => g.DateReleased, opt => opt.MapFrom(a => ToUtc(a.DateReleased)));
            CreateMap<AdvertisementSaveDto, Console>()
                .ForMember(c => c.ConsoleRegionId, opt => opt.MapFrom(a => a.RegionId))
                .ForMember(c => c.DateReleased, opt => opt.MapFrom(a => ToUtc(a.DateReleased)));
            CreateMap<AdvertisementSaveDto, Accessory>()
                .ForMember(a => a.DateReleased, opt => opt.MapFrom(a => ToUtc(a.DateReleased)));

            CreateMap<Advertisement, AdvertisementGameDto>();
            CreateMap<Game, AdvertisementGameDto>()
                .ForMember(a => a.Photos, opt => opt.Ignore())
                .ForMember(a => a.Id, opt => opt.Ignore());

            CreateMap<Advertisement, AdvertisementAccessoryDto>();
            CreateMap<Accessory, AdvertisementAccessoryDto>()
                .ForMember(a => a.Photos, opt => opt.Ignore())
                .ForMember(a => a.Id, opt => opt.Ignore());

            CreateMap<Advertisement, AdvertisementConsoleDto>();
            CreateMap<Console, AdvertisementConsoleDto>()
                .ForMember(a => a.Photos, opt => opt.Ignore())
                .ForMember(a => a.Id, opt => opt.Ignore());
        }

        private static DateTime? ToUtc(DateTime? value)
        {
            return value.HasValue
                ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
                : null;
        }
    }
}
