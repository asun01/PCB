namespace Asun.UI.Viewports;

public readonly record struct ViewportPresentationExecutionResult<TTile>(
    bool Executed,
    bool Presented,
    bool Superseded,
    ViewportPresentationPacket<TTile>? Packet,
    ViewportRenderDeliveryResult Delivery)
{
    public bool Cancelled =>
        Executed &&
        Delivery.Status == ViewportRenderDeliveryStatus.Cancelled;

    public long Generation =>
        Packet?.Token.Generation ?? Delivery.Generation;
}

public sealed class ViewportPresentationExecutionRuntime<TTile>
{
    private readonly ViewportPresentationQueueRuntime<TTile> _queue;
    private readonly ViewportPresentationBufferRuntime _buffers;
    private readonly ViewportRenderSurfaceRuntime _surface;
    private readonly ViewportRenderDeliveryTracker _delivery;

    public ViewportPresentationExecutionRuntime(
        ViewportPresentationQueueRuntime<TTile> queue,
        ViewportPresentationBufferRuntime buffers,
        ViewportRenderSurfaceRuntime surface,
        ViewportRenderDeliveryTracker delivery)
    {
        _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        _buffers = buffers ?? throw new ArgumentNullException(nameof(buffers));
        _surface = surface ?? throw new ArgumentNullException(nameof(surface));
        _delivery = delivery ?? throw new ArgumentNullException(nameof(delivery));
    }

    public ViewportPresentationQueueRuntime<TTile> Queue => _queue;

    public ViewportPresentationBufferRuntime Buffers => _buffers;

    public ViewportRenderSurfaceRuntime Surface => _surface;

    public ViewportRenderDeliveryTracker Delivery => _delivery;

    public async ValueTask<ViewportPresentationExecutionResult<TTile>> ExecuteNextAsync(
        IViewportRenderSink<TTile> sink,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sink);

        if (!_queue.TryTakeLatest(out var packet))
        {
            return default;
        }

        ViewportPresentationBufferTransaction? bufferTransaction = null;

        try
        {
            var frame = packet.Frame;

            bufferTransaction = _buffers.Begin(
                packet.Token,
                frame.Batch.ItemCount,
                frame.CommandStream.Regions);

            var delivery = await ViewportRenderDeliveryRuntime
                .TryDeliverAsync(
                    frame,
                    sink,
                    _delivery,
                    cancellationToken,
                    _surface)
                .ConfigureAwait(false);

            if (delivery.Succeeded)
            {
                _buffers.Commit(
                    bufferTransaction.Value,
                    delivery.RenderedUnits,
                    frame.CommandStream.Regions);

                if (_queue.TryAcknowledgePresented(packet.Token))
                {
                    return new(
                        true,
                        true,
                        false,
                        packet,
                        delivery);
                }

                _queue.TryCancel(packet.Token);

                return new(
                    true,
                    false,
                    true,
                    packet,
                    delivery);
            }

            if (bufferTransaction is ViewportPresentationBufferTransaction activeBuffer)
                _buffers.Discard(activeBuffer);

            _queue.TryCancel(packet.Token);

            return new(
                true,
                false,
                false,
                packet,
                delivery);
        }
        catch
        {
            if (bufferTransaction is ViewportPresentationBufferTransaction activeBuffer)
                _buffers.Discard(activeBuffer);

            _queue.TryCancel(packet.Token);
            throw;
        }
    }

    public async ValueTask<ViewportPresentationExecutionResult<TTile>> WaitAndExecuteAsync(
        IViewportRenderSink<TTile> sink,
        CancellationToken cancellationToken = default)
    {
        await _queue
            .WaitForActivityAsync(cancellationToken)
            .ConfigureAwait(false);

        return await ExecuteNextAsync(
            sink,
            cancellationToken)
            .ConfigureAwait(false);
    }
}
