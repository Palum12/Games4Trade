using Games4TradeAPI.Dtos;
using Games4TradeAPI.Models;

namespace Games4TradeAPI.Core.Mapping;

public static class MappingExtensions
{
    public static UserDto ToDto(this User source)
    {
        return new UserDto
        {
            Id = source.Id,
            Login = source.Login,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber!,
            Description = source.Description!
        };
    }

    public static UserSimpleDto ToSimpleDto(this User source)
    {
        return new UserSimpleDto
        {
            Id = source.Id,
            Login = source.Login
        };
    }

    public static User ToModel(this UserRegisterDto source)
    {
        return new User
        {
            Login = source.Login,
            Email = source.Email
        };
    }

    public static GenreDto ToDto(this Genre source)
    {
        return new GenreDto
        {
            Id = source.Id,
            Value = source.Value
        };
    }

    public static Genre ToModel(this GenreCreateOrUpdateDto source)
    {
        return new Genre
        {
            Value = source.Value
        };
    }

    public static SystemDto ToDto(this Models.System source)
    {
        return new SystemDto
        {
            Id = source.Id,
            Manufacturer = source.Manufacturer,
            Model = source.Model
        };
    }

    public static Models.System ToModel(this SystemCreateOrUpdateDto source)
    {
        return new Models.System
        {
            Manufacturer = source.Manufacturer,
            Model = source.Model
        };
    }

    public static AnnouncementGetDto ToDto(this Announcement source)
    {
        return new AnnouncementGetDto
        {
            Id = source.Id,
            Title = source.Title,
            Author = source.User?.Login!,
            Content = source.Content,
            IsActive = source.IsActive,
            DateCreated = source.DateCreated
        };
    }

    public static Announcement ToModel(this AnnouncementSaveDto source)
    {
        return new Announcement
        {
            Title = source.Title,
            Content = source.Content
        };
    }

    public static MessageDto ToDto(this Message source)
    {
        return new MessageDto
        {
            Id = source.Id,
            Content = source.Content,
            IsDelivered = source.IsDelivered,
            DateCreated = source.DateCreated,
            SenderId = source.SenderId,
            ReceiverId = source.ReceiverId
        };
    }

    public static PhotoDto ToDto(this Photo source)
    {
        return new PhotoDto
        {
            Id = source.Id,
            Path = source.Path
        };
    }

    public static RegionDto ToDto(this Region source)
    {
        return new RegionDto
        {
            Id = source.Id,
            Value = source.Value
        };
    }

    public static StateDto ToDto(this State source)
    {
        return new StateDto
        {
            Id = source.Id,
            Value = source.Value
        };
    }

    public static Advertisement ToModel(this AdvertisementSaveDto source)
    {
        return new Advertisement
        {
            DateCreated = DateTime.UtcNow,
            Title = source.Title,
            IsActive = true,
            ExchangeActive = source.ExchangeActive,
            ShowEmail = source.ShowEmail,
            ShowPhone = source.ShowPhone,
            Price = source.Price
        };
    }

    public static AdvertisementWithoutItemDto ToSummaryDto(this Advertisement source)
    {
        return new AdvertisementWithoutItemDto
        {
            Id = source.Id,
            UserId = source.UserId,
            MainPhotoId = source.Photos?.FirstOrDefault()?.Id,
            Title = source.Title,
            DateCreated = source.DateCreated,
            ExchangeActive = source.ExchangeActive ?? false,
            IsActive = source.IsActive,
            Price = source.Price
        };
    }

    public static void MapTo(this Advertisement source, AdvertisementBasicDto destination)
    {
        destination.Id = source.Id;
        destination.UserId = source.UserId;
        destination.Title = source.Title;
        destination.DateCreated = source.DateCreated;
        destination.ExchangeActive = source.ExchangeActive ?? false;
        destination.IsActive = source.IsActive;
        destination.Price = source.Price;
        destination.ShowEmail = source.ShowEmail;
        destination.ShowPhone = source.ShowPhone;
        destination.User = source.User.ToDto();
        destination.Photos = source.Photos?.Select(photo => photo.ToDto()).ToList()
            ?? new List<PhotoDto>();
    }

    public static void MapTo(this AdvertisementItem source, AdvertisementBasicDto destination)
    {
        destination.Description = source.Description;
        destination.DateReleased = source.DateReleased;
    }

    public static void MapTo(this Game source, AdvertisementGameDto destination)
    {
        ((AdvertisementItem)source).MapTo(destination);
        destination.Developer = source.Developer;
    }

    public static void MapTo(this Models.Console source, AdvertisementConsoleDto destination)
    {
        ((AdvertisementItem)source).MapTo(destination);
    }

    public static void MapTo(this Accessory source, AdvertisementAccessoryDto destination)
    {
        ((AdvertisementItem)source).MapTo(destination);
        destination.AccessoryManufacturer = source.AccessoryManufacturer;
        destination.AccessoryModel = source.AccessoryModel;
    }

    public static DateTime? AsUtc(this DateTime? source)
    {
        return source.HasValue
            ? DateTime.SpecifyKind(source.Value, DateTimeKind.Utc)
            : null;
    }
}
