using Games4TradeAPI.Core.Mapping;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Interfaces.Repositories;
using Games4TradeAPI.Models;

namespace Games4TradeAPI.Core.Advertisements;

public sealed class GameAdvertisementItemStrategy : AdvertisementItemStrategy<Game>
{
    private readonly IGenreRepository genreRepository;
    private readonly IRepository<Region> regionRepository;

    public GameAdvertisementItemStrategy(
        IRepository<State> stateRepository,
        ISystemRepository systemRepository,
        IGenreRepository genreRepository,
        IRepository<Region> regionRepository)
        : base(stateRepository, systemRepository)
    {
        this.genreRepository = genreRepository;
        this.regionRepository = regionRepository;
    }

    public override string Discriminator => nameof(Game);

    protected override Game CreateItem()
    {
        return new Game();
    }

    protected override void UpdateSpecificFields(Game target, AdvertisementSaveDto source)
    {
        target.Developer = source.Developer;
        target.GameRegionId = source.RegionId;
        target.GenreId = source.GenreId;
    }

    protected override async Task<string?> ValidateSpecificRelationshipsAsync(AdvertisementSaveDto source)
    {
        if (!source.GenreId.HasValue || !source.RegionId.HasValue)
        {
            return "Invalid data";
        }

        var genre = await genreRepository.GetAsync(source.GenreId.Value);
        var region = await regionRepository.GetAsync(source.RegionId.Value);
        return genre == null || region == null ? "Invalid data" : null;
    }

    protected override async Task<AdvertisementBasicDto> CreateDtoAsync(Game source)
    {
        var result = new AdvertisementGameDto();
        source.Advertisement.MapTo(result);
        source.MapTo(result);

        var genre = await genreRepository.GetAsync(source.GenreId
            ?? throw new InvalidOperationException("Game advertisement is missing a genre."));
        var region = await regionRepository.GetAsync(source.GameRegionId
            ?? throw new InvalidOperationException("Game advertisement is missing a region."));

        result.Genre = (genre
            ?? throw new InvalidOperationException("Game advertisement references a missing genre."))
            .ToDto();
        result.Region = (region
            ?? throw new InvalidOperationException("Game advertisement references a missing region."))
            .ToDto();
        return result;
    }
}
