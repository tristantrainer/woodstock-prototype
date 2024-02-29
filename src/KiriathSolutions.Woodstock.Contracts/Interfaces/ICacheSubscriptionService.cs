using KiriathSolutions.Woodstock.Contracts.Constants;

namespace KiriathSolutions.Woodstock.Contracts.Interfaces;

public interface ICacheSubscriptionService 
{
    Task Notify(ICacheSubscriptionMessage message, CancellationToken token = default);
}

public interface ICacheSubscriptionMessage 
{
    CacheUpdateEntity Entity { get; }
    CacheUpdateType Type { get; }
    Guid Id { get; }
}