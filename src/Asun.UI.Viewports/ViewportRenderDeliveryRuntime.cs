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
    Exception? Error)
{
    public ViewportRenderDeliveryStatus Status =>
        Cancelled
            ? ViewportRenderDeliveryStatus.Cancelled
            : Deferred
                ? ViewportRenderDeliveryStatus.Deferred
                : Succeeded
                    ? ViewportRenderDeliveryStatus.Succeeded
                    : ViewportRenderDeliveryStatus.Failed;
}

public static class ViewportRenderDeliveryRuntime
{
    public static async ValueTask<ViewportRenderDeliveryResult> TryDeliverAsync<TTile>(
        ViewportRenderPipelineFrame<TTile> frame,
        IViewportRenderSink<TTile> sink,
        ViewportRenderDeliveryTracker? tracker = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(sink);

        var started = System.Diagnostics.Stopwatch.GetTimestamp();

        ViewportRenderDeliveryResult result;

        try
        {
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
                null);
        }
        catch (OperationCanceledException)
        {
            result = new ViewportRenderDeliveryResult(
                false,
                true,
                false,
                frame.Composite.Generation,
                0,
                0,
                Array.Empty<ViewportRenderWorkItem>(),
                null);
        }
        catch (ViewportRenderWorkUnavailableException exception)
        {
            result = new ViewportRenderDeliveryResult(
                false,
                false,
                true,
                frame.Composite.Generation,
                exception.RenderedUnits,
                exception.WorkItems.Count,
                exception.WorkItems,
                exception);
        }
        catch (Exception exception)
        {
            result = new ViewportRenderDeliveryResult(
                false,
                false,
                false,
                frame.Composite.Generation,
                0,
                0,
                Array.Empty<ViewportRenderWorkItem>(),
                exception);
        }

        tracker?.Record(
            result,
            System.Diagnostics.Stopwatch.GetElapsedTime(started));

        return result;
    }
}
