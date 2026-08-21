using Games4TradeAPI.Core.Mapping;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Interfaces.Repositories;
using Games4TradeAPI.Models;
using Console = Games4TradeAPI.Models.Console;

namespace Games4TradeAPI.Core.Advertisements;

public sealed class ConsoleAdvertisementItemStrategy : AdvertisementItemStrategy<Console>
{
    private readonly IRepository<Region> regionRepository;

    public ConsoleAdvertisementItemStrategy(
        IRepository<State> stateRepository,
        ISystemRepository systemRepository,
        IRepository<Region> regionRepository)
        : base(stateRepository, systemRepository)
    {
        this.regionRepository = regionRepository;
    }

    public override string Discriminator => nameof(Console);

    protected override Console CreateItem()
    {
        return new Console();
    }

    protected override void UpdateSpecificFields(Console target, AdvertisementSaveDto source)
    {
        target.ConsoleRegionId = source.RegionId;
    }

    protected override async Task<string?> ValidateSpecificRelationshipsAsync(AdvertisementSaveDto source)
    {
        if (!source.RegionId.HasValue)
        {
            return "Invalid data";
        }

        return await regionRepository.GetAsync(source.RegionId.Value) == null
            ? "Invalid data"
            : null;
    }

    protected override async Task<AdvertisementBasicDto> CreateDtoAsync(Console source)
    {
        var result = new AdvertisementConsoleDto();
        source.Advertisement.MapTo(result);
        source.MapTo(result);

        var region = await regionRepository.GetAsync(source.ConsoleRegionId
            ?? throw new InvalidOperationException("Console advertisement is missing a region."));
        result.Region = (region
            ?? throw new InvalidOperationException("Console advertisement references a missing region."))
            .ToDto();
        return result;
    }
}
