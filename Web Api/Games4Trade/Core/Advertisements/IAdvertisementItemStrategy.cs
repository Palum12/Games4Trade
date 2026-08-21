using Games4TradeAPI.Dtos;
using Games4TradeAPI.Models;

namespace Games4TradeAPI.Core.Advertisements;

public interface IAdvertisementItemStrategy
{
    string Discriminator { get; }
    Type ItemType { get; }

    AdvertisementItem Create(AdvertisementSaveDto source);
    void Update(AdvertisementItem target, AdvertisementSaveDto source);
    Task<string?> ValidateRelationshipsAsync(AdvertisementSaveDto source);
    Task<AdvertisementBasicDto> ToDtoAsync(AdvertisementItem source);
}
