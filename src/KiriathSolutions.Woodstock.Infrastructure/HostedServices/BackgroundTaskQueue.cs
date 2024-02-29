using System.Threading.Channels;
using KiriathSolutions.Woodstock.Contracts.Interfaces;
using MediatR;

namespace KiriathSolutions.Woodstock.Infrastructure.HostedServices;

public interface IBackgroundTaskQueue
{
    ValueTask QueueAsync<T>(T request) where T : IRequest;
     
    ValueTask<Func<CancellationToken, Task>> DequeueAsync(
        CancellationToken cancellationToken);
}

internal sealed class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly ISender _sender;

    private readonly Channel<Func<CancellationToken, Task>> _queue;

    public BackgroundTaskQueue(IHostedServiceOptions hostedServiceOptions, ISender sender)
    {
        var options = new BoundedChannelOptions(hostedServiceOptions.Capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        _queue = Channel.CreateBounded<Func<CancellationToken, Task>>(options);

        _sender = sender;
    }

    public async ValueTask QueueAsync<T>(T request) where T : IRequest
    {
        await _queue.Writer.WriteAsync((token) => {
            return _sender.Send(request, token);
        });
    }

    public async ValueTask<Func<CancellationToken, Task>> DequeueAsync(
        CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);
        return workItem;
    }
}