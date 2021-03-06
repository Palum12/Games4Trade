﻿using System;
using System.Collections.Generic;

namespace Games4TradeAPI.Dtos
{
    public class AdvertisementWithoutItemDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? MainPhotoId { get; set; }
        public string Title { get; set; }      
        public DateTime? DateCreated { get; set; }
        public bool ExchangeActive { get; set; }
        public bool IsActive { get; set; }
        public decimal Price { get; set; }
    }

    public class AdvertisementBasicDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Title { get; set; }
        public string Discriminator { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool ExchangeActive { get; set; }
        public bool IsActive { get; set; }
        public decimal Price { get; set; }
        public bool ShowEmail { get; set; }
        public bool ShowPhone { get; set; }

        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string Description { get; set; }
        public DateTime? DateReleased { get; set; }
        public StateDto State { get; set; }                
        public SystemDto System { get; set; }

        public UserDto User { get; set; }
        public ICollection<PhotoDto> Photos { get; set; }
    }

    public class AdvertisementGameDto : AdvertisementBasicDto
    {       
        public string Developer { get; set; }
        public GenreDto Genre { get; set; }
        public RegionDto Region { get; set; }
    }

    public class AdvertisementAccessoryDto : AdvertisementBasicDto
    {
        public string AccessoryManufacturer { get; set; }
        public string AccessoryModel { get; set; }
    }

    public class AdvertisementConsoleDto : AdvertisementBasicDto
    {
        public RegionDto Region { get; set; }
    }

    public class AdvertisementSaveDto
    {
        public string Title { get; set; }
        public string Discriminator { get; set; }
        public bool ExchangeActive {get;set;}
        public decimal Price { get; set; }
        public int StateId { get; set; }
        public int SystemId { get; set; }
        public string Description { get; set; }
        public DateTime? DateReleased { get; set; }
        public bool ShowEmail { get; set; }
        public bool ShowPhone { get; set; }
        // game & console
        public int? RegionId { get; set; }
        //game part       
        public string Developer { get; set; }
        public int? GenreId { get; set; }
        //accessory part
        public string AccessoryManufacturer { get; set; }
        public string AccessoryModel { get; set; }
    }
}
