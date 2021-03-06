﻿using System;
using System.Linq;
using AutoMapper.Configuration;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Models;
using Console = Games4TradeAPI.Models.Console;

namespace Games4TradeAPI
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

            CreateMap<Advertisement, AdvertisementWithoutItemDto>()
                .ForMember(a => a.MainPhotoId, opt =>
                {
                    opt.PreCondition(a => a.Photos != null && a.Photos.Any());
                    opt.MapFrom(a => a.Photos.FirstOrDefault().Id);
                });

            CreateMap<AdvertisementSaveDto, Advertisement>()
                .ForMember(a => a.DateCreated, opt => opt.MapFrom(a => DateTime.Now))
                .ForMember(a => a.ExchangeActive, opt => opt.MapFrom(a => true))
                .ForMember(a => a.UserId, opt => opt.Ignore());
            CreateMap<AdvertisementSaveDto, Game>().ForMember(g => g.GameRegionId, opt => opt.MapFrom(a => a.RegionId));
            CreateMap<AdvertisementSaveDto, Console>().ForMember(c => c.ConsoleRegionId, opt => opt.MapFrom(a => a.RegionId));
            CreateMap<AdvertisementSaveDto, Accessory>();

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
    }
}
