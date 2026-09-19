namespace Asun.UI.Viewports;

public enum ViewportRenderDeliveryStatus
{
    Succeeded,
    Failed,
    Deferred,
    Cancelled
}

public sealed class ViewportRenderWorkUnavailableException : Exception
{
    public ViewportRenderWorkUnavailableException(
        ViewportRenderWorkItem workItem,
        int renderedUnits = 0)
        : this(
            new[] { workItem },
            renderedUnits)
    {
    }

    public ViewportRenderWorkUnavailableException(
        IReadOnlyList<ViewportRenderWorkItem> workItems,
        int renderedUnits = 0)
        : base($"{workItems.Count} render work item(s) are not currently available.")
    {
        ArgumentNullException.ThrowIfNull(workItems);

        if (workItems.Count == 0)
            throw new ArgumentException(
                "At least one deferred work item is required.",
                nameof(workItems));

        if (renderedUnits < 0)
            throw new ArgumentOutOfRangeException(nameof(renderedUnits));

        WorkItems = workItems.ToArray();
        RenderedUnits = renderedUnits;
    }

    public IReadOnlyList<ViewportRenderWorkItem> WorkItems { get; }

    public ViewportRenderWorkItem WorkItem => WorkItems[0];

    public int RenderedUnits { get; }
}

public readonly record struct ViewportRenderDeliveryResult(
    bool Succeeded,
    bool Cancelled,
    bool Deferred,
    long Generation,
    int RenderedUnits,
    int DeferredUnits,
    IReadOnlyList<ViewportRenderWorkItem> DeferredWorkItems,
    Exception? Error,
    int PlannedUnits,
    int RegionCount)
{
    public ViewportRenderDeliveryStatus Status =>
        Cancelled
            ? ViewportRenderDeliveryStatus.Cancelled
            : Deferred
                ? ViewportRenderDeliveryStatus.Deferred
                : Succeeded
                    ? ViewportRenderDeliveryStatus.Succeeded
                    : ViewportRenderDeliveryStatus.Failed;

    public ViewportRenderFrameState FrameState => new(
        Status,
        Generation,
        PlannedUnits,
        RenderedUnits,
        DeferredUnits,
        RegionCount,
        DeferredWorkItems,
        Error);
}

public static class ViewportRenderDeliveryRuntime
{
    public static async ValueTask<ViewportRenderDeliveryResult> TryDeliverAsync<TTile>(
        ViewportRenderPipelineFrame<TTile> frame,
        IViewportRenderSink<TTile> sink,
        ViewportRenderDeliveryTracker? tracker = null,
        CancellationToken cancellationToken = default,
        ViewportRenderSurfaceRuntime? surface = null,
        Func<bool>? presentationFence = null,
        Func<bool>? beginPresentationCommit = null)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(sink);

        var started = System.Diagnostics.Stopwatch.GetTimestamp();

        ViewportRenderDeliveryResult result;
        ViewportRenderSurfaceTransaction? transaction = null;

        try
        {
            if (surface is not null)
                transaction = surface.Begin(frame.Composite.Generation);

            var units = await ViewportRenderAdapterRuntime
                .RenderAsync(frame, sink, cancellationToken)
                .ConfigureAwait(false);

            result = new ViewportRenderDeliveryResult(
                true,
                false,
                false,
                frame.Composite.Generation,
                units,
                0,
                Array.Empty<ViewportRenderWorkItem>(),
                null,
                frame.Batch.ItemCount,
                frame.Batch.RegionCount);

            if (surface is not null)
            {
                try
                {
                    if (beginPresentationCommit is not null &&
                        !beginPresentationCommit())
                        throw new ViewportPresentationFenceRejectedException();

                    if (presentationFence is not null &&
                        !presentationFence())
                        throw new ViewportPresentationFenceRejectedException();

                    await sink.CommitFrameAsync(
                        new ViewportRenderCommitContext(
                            frame.Composite.Generation,
                            frame.Batch.ItemCount,
                            units,
                            frame.Batch.RegionCount,
                            frame.Batch.Regions,
                            frame.Batch.TileCount,
                            frame.Batch.RoiCount,
                            frame.Batch.OverlayCount,
                            frame.Batch.InvalidationCount,
                            frame.Batch.FullSurfaceCount),
                        cancellationToken).ConfigureAwait(false);

                    if (presentationFence is not null &&
                        !presentationFence())
                        throw new ViewportPresentationFenceRejectedException();

                    surface.Commit(
                        transaction!.Value,
                        frame.Batch.ItemCount,
                        units,
                        frame.Batch.Regions,
                        presentationFence);
                }
                catch (Exception exception)
                {
                    if (transaction is ViewportRenderSurfaceTransaction activeTransaction)
                    {
                        TryDiscardSurface(
                            surface,
                            activeTransaction,
                            exception is OperationCanceledException
                                ? ViewportRenderDeliveryStatus.Cancelled
                                : ViewportRenderDeliveryStatus.Failed);
                    }

                    await TryDiscardAsync(
                        sink,
                        new ViewportRenderDiscardContext(
                            frame.Composite.Generation,
                            exception is OperationCanceledException
                                ? ViewportRenderDeliveryStatus.Cancelled
                                : ViewportRenderDeliveryStatus.Failed,
                            frame.Batch.ItemCount,
                            units,
                            0,
                            exception)).ConfigureAwait(false);

                    result = new ViewportRenderDeliveryResult(
                        false,
                        exception is OperationCanceledException,
                        false,
                        frame.Composite.Generation,
                        units,
                        0,
                        Array.Empty<ViewportRenderWorkItem>(),
                        exception,
                        frame.Batch.ItemCount,
                        frame.Batch.RegionCount);
                }
            }
        }
        catch (OperationCanceledException)
        {
            if (surface is not null &&
                transaction is ViewportRenderSurfaceTransaction cancelledTransaction)
            {
                TryDiscardSurface(
                    surface,
                    cancelledTransaction,
                    ViewportRenderDeliveryStatus.Cancelled);
                await TryDiscardAsync(
                    sink,
                    new ViewportRenderDiscardContext(
                        frame.Composite.Generation,
                        ViewportRenderDeliveryStatus.Cancelled,
                        frame.Batch.ItemCount,
                        0,
                        0,
                        null)).ConfigureAwait(false);
            }

            result = new ViewportRenderDeliveryResult(
                false,
                true,
                false,
                frame.Composite.Generation,
                0,
                0,
                Array.Empty<ViewportRenderWorkItem>(),
                null,
                frame.Batch.ItemCount,
                frame.Batch.RegionCount);
        }
        catch (ViewportRenderWorkUnavailableException exception)
        {
            if (surface is not null &&
                transaction is ViewportRenderSurfaceTransaction deferredTransaction)
            {
                TryDiscardSurface(
                    surface,
                    deferredTransaction,
                    ViewportRenderDeliveryStatus.Deferred);
                await TryDiscardAsync(
                    sink,
                    new ViewportRenderDiscardContext(
                        frame.Composite.Generation,
                        ViewportRenderDeliveryStatus.Deferred,
                        frame.Batch.ItemCount,
                        exception.RenderedUnits,
                        exception.WorkItems.Count,
                        exception)).ConfigureAwait(false);
            }

            result = new ViewportRenderDeliveryResult(
                false,
                false,
                true,
                frame.Composite.Generation,
                exception.RenderedUnits,
                exception.WorkItems.Count,
                exception.WorkItems,
                exception,
                frame.Batch.ItemCount,
                frame.Batch.RegionCount);
        }
        catch (Exception exception)
        {
            if (surface is not null &&
                transaction is ViewportRenderSurfaceTransaction failedTransaction)
            {
                TryDiscardSurface(
                    surface,
                    failedTransaction,
                    ViewportRenderDeliveryStatus.Failed);
                await TryDiscardAsync(
                    sink,
                    new ViewportRenderDiscardContext(
                        frame.Composite.Generation,
                        ViewportRenderDeliveryStatus.Failed,
                        frame.Batch.ItemCount,
                        0,
                        0,
                        exception)).ConfigureAwait(false);
            }

            result = new ViewportRenderDeliveryResult(
                false,
                false,
                false,
                frame.Composite.Generation,
                0,
                0,
                Array.Empty<ViewportRenderWorkItem>(),
                exception,
                frame.Batch.ItemCount,
                frame.Batch.RegionCount);
        }

        tracker?.Record(
            result,
            System.Diagnostics.Stopwatch.GetElapsedTime(started));

        return result;
    }

    private static void TryDiscardSurface<TTile>(
        ViewportRenderSurfaceRuntime surface,
        ViewportRenderSurfaceTransaction transaction,
        ViewportRenderDeliveryStatus status)
    {
        try
        {
            surface.Discard(transaction, status);
        }
        catch
        {
            // Surface rollback is best effort; the original delivery result remains authoritative.
        }
    }

    private static async ValueTask TryDiscardAsync<TTile>(
        IViewportRenderSink<TTile> sink,
        ViewportRenderDiscardContext context)
    {
        try
        {
            await sink
                .DiscardFrameAsync(context)
                .ConfigureAwait(false);
        }
        catch
        {
            // The original delivery status/error is authoritative. Discard is best effort.
        }
    }
}
