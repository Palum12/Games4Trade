using Games4TradeAPI.Models;

namespace Games4TradeAPI.Core.Advertisements;

public sealed class AdvertisementItemStrategyResolver : IAdvertisementItemStrategyResolver
{
    private readonly IReadOnlyDictionary<string, IAdvertisementItemStrategy> strategiesByDiscriminator;
    private readonly IReadOnlyDictionary<Type, IAdvertisementItemStrategy> strategiesByItemType;

    public AdvertisementItemStrategyResolver(IEnumerable<IAdvertisementItemStrategy> strategies)
    {
        var registeredStrategies = strategies.ToArray();
        strategiesByDiscriminator = registeredStrategies.ToDictionary(
            strategy => strategy.Discriminator,
            StringComparer.OrdinalIgnoreCase);
        strategiesByItemType = registeredStrategies.ToDictionary(strategy => strategy.ItemType);
    }

    public bool TryResolve(string? discriminator, out IAdvertisementItemStrategy strategy)
    {
        if (!string.IsNullOrWhiteSpace(discriminator)
            && strategiesByDiscriminator.TryGetValue(discriminator, out var resolvedStrategy))
        {
            strategy = resolvedStrategy;
            return true;
        }

        strategy = null!;
        return false;
    }

    public IAdvertisementItemStrategy Resolve(AdvertisementItem item)
    {
        if (strategiesByItemType.TryGetValue(item.GetType(), out var strategy))
        {
            return strategy;
        }

        strategy = strategiesByItemType.Values.SingleOrDefault(candidate =>
            candidate.ItemType.IsInstanceOfType(item));
        if (strategy != null)
        {
            return strategy;
        }

        throw new NotSupportedException($"Advertisement item type '{item.GetType().Name}' is not supported.");
    }
}
