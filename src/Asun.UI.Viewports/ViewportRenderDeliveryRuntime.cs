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
        ViewportRenderWorkItem workItem)
        : base($"Render work is not currently available: {workItem.Kind}.")
    {
        WorkItem = workItem;
    }

    public ViewportRenderWorkItem WorkItem { get; }
}

public readonly record struct ViewportRenderDeliveryResult(
    bool Succeeded,
    bool Cancelled,
    long Generation,
    int RenderedUnits,
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
                null);
        }
        catch (ViewportRenderWorkUnavailableException)
        {
            result = new ViewportRenderDeliveryResult(
                false,
                false,
                true,
                frame.Composite.Generation,
                0,
                1,
                null);
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
                exception);
        }

        tracker?.Record(
            result,
            System.Diagnostics.Stopwatch.GetElapsedTime(started));

        return result;
    }
}
