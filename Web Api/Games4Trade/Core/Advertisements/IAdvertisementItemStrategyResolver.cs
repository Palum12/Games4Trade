using Games4TradeAPI.Models;

namespace Games4TradeAPI.Core.Advertisements;

public interface IAdvertisementItemStrategyResolver
{
    bool TryResolve(string? discriminator, out IAdvertisementItemStrategy strategy);
    IAdvertisementItemStrategy Resolve(AdvertisementItem item);
}
