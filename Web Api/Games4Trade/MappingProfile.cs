﻿using AutoMapper.Configuration;
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

            CreateMap<AdvertisementSaveDto, Game>().ForMember(g => g.GameRegionId, opt => opt.MapFrom(a => a.RegionId));
            CreateMap<AdvertisementSaveDto, Console>().ForMember(c => c.ConsoleRegionId, opt => opt.MapFrom(a => a.RegionId));
            CreateMap<AdvertisementSaveDto, Accessory>();

        }
    }
}
