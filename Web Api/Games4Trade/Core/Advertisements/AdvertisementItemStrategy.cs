using Games4TradeAPI.Core.Mapping;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Interfaces.Repositories;
using Games4TradeAPI.Models;

namespace Games4TradeAPI.Core.Advertisements;

public abstract class AdvertisementItemStrategy<TItem> : IAdvertisementItemStrategy
    where TItem : AdvertisementItem
{
    private readonly IRepository<State> stateRepository;
    private readonly ISystemRepository systemRepository;

    protected AdvertisementItemStrategy(
        IRepository<State> stateRepository,
        ISystemRepository systemRepository)
    {
        this.stateRepository = stateRepository;
        this.systemRepository = systemRepository;
    }

    public abstract string Discriminator { get; }
    public Type ItemType => typeof(TItem);

    public AdvertisementItem Create(AdvertisementSaveDto source)
    {
        var item = CreateItem();
        Update(item, source);
        return item;
    }

    public void Update(AdvertisementItem target, AdvertisementSaveDto source)
    {
        if (target is not TItem typedTarget)
        {
            throw new ArgumentException(
                $"Strategy '{GetType().Name}' cannot update '{target.GetType().Name}'.",
                nameof(target));
        }

        typedTarget.DateReleased = source.DateReleased.AsUtc();
        typedTarget.StateId = source.StateId;
        typedTarget.SystemId = source.SystemId;
        typedTarget.Description = source.Description;
        UpdateSpecificFields(typedTarget, source);
    }

    public async Task<string?> ValidateRelationshipsAsync(AdvertisementSaveDto source)
    {
        var state = await stateRepository.GetAsync(source.StateId);
        var system = await systemRepository.GetAsync(source.SystemId);
        if (state == null || system == null)
        {
            return "Invalid data";
        }

        return await ValidateSpecificRelationshipsAsync(source);
    }

    public async Task<AdvertisementBasicDto> ToDtoAsync(AdvertisementItem source)
    {
        if (source is not TItem typedSource)
        {
            throw new ArgumentException(
                $"Strategy '{GetType().Name}' cannot map '{source.GetType().Name}'.",
                nameof(source));
        }

        var result = await CreateDtoAsync(typedSource);
        result.Discriminator = Discriminator;

        var state = await stateRepository.GetAsync(source.StateId)
            ?? throw new InvalidOperationException("Advertisement is missing a state.");
        var system = await systemRepository.GetAsync(source.SystemId)
            ?? throw new InvalidOperationException("Advertisement is missing a system.");

        result.State = state.ToDto();
        result.System = system.ToDto();

        if (source.Advertisement.ShowEmail)
        {
            result.Email = source.Advertisement.User.Email;
        }

        if (source.Advertisement.ShowPhone
            && !string.IsNullOrEmpty(source.Advertisement.User.PhoneNumber))
        {
            result.PhoneNumber = source.Advertisement.User.PhoneNumber;
        }

        return result;
    }

    protected abstract TItem CreateItem();
    protected abstract void UpdateSpecificFields(TItem target, AdvertisementSaveDto source);
    protected abstract Task<AdvertisementBasicDto> CreateDtoAsync(TItem source);

    protected virtual Task<string?> ValidateSpecificRelationshipsAsync(AdvertisementSaveDto source)
    {
        return Task.FromResult<string?>(null);
    }
}
