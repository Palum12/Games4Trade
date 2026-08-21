using Games4TradeAPI.Core.Mapping;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Interfaces.Repositories;
using Games4TradeAPI.Models;

namespace Games4TradeAPI.Core.Advertisements;

public sealed class AccessoryAdvertisementItemStrategy : AdvertisementItemStrategy<Accessory>
{
    public AccessoryAdvertisementItemStrategy(
        IRepository<State> stateRepository,
        ISystemRepository systemRepository)
        : base(stateRepository, systemRepository)
    {
    }

    public override string Discriminator => nameof(Accessory);

    protected override Accessory CreateItem()
    {
        return new Accessory();
    }

    protected override void UpdateSpecificFields(Accessory target, AdvertisementSaveDto source)
    {
        target.AccessoryManufacturer = source.AccessoryManufacturer;
        target.AccessoryModel = source.AccessoryModel;
    }

    protected override Task<AdvertisementBasicDto> CreateDtoAsync(Accessory source)
    {
        var result = new AdvertisementAccessoryDto();
        source.Advertisement.MapTo(result);
        source.MapTo(result);
        return Task.FromResult<AdvertisementBasicDto>(result);
    }
}
