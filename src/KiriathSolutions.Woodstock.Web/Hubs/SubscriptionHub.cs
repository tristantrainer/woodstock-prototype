using KiriathSolutions.Woodstock.Contracts.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Extensions;

namespace KiriathSolutions.Woodstock.Web.Hubs;

internal sealed class CacheSubscriptionService(IHubContext<CacheSubscriptionHub> hubContext) : ICacheSubscriptionService
{
    private readonly IHubContext<CacheSubscriptionHub> _hubContext = hubContext;

    public Task Notify(ICacheSubscriptionMessage message, CancellationToken token = default) 
    {
        return _hubContext.Clients.All.SendAsync("CacheUpdated", new { 
            Entity = message.Entity.GetDisplayName().ToUpper(), 
            Type = message.Entity.GetDisplayName().ToUpper() 
        }, token);
    }
}

internal sealed class CacheSubscriptionHub : Hub { }