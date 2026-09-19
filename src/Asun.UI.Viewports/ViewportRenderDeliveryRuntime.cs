namespace Asun.UI.Viewports;

public readonly record struct ViewportRenderDeliveryResult(
    bool Succeeded,
    bool Cancelled,
    long Generation,
    int RenderedUnits,
    Exception? Error);

public static class ViewportRenderDeliveryRuntime
{
    public static async ValueTask<ViewportRenderDeliveryResult> TryDeliverAsync<TTile>(
        ViewportRenderPipelineFrame<TTile> frame,
        IViewportRenderSink<TTile> sink,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(sink);

        try
        {
            var units = await ViewportRenderAdapterRuntime
                .RenderAsync(frame, sink, cancellationToken)
                .ConfigureAwait(false);

            return new ViewportRenderDeliveryResult(
                true,
                false,
                frame.Composite.Generation,
                units,
                null);
        }
        catch (OperationCanceledException)
        {
            return new ViewportRenderDeliveryResult(
                false,
                true,
                frame.Composite.Generation,
                0,
                null);
        }
        catch (Exception exception)
        {
            return new ViewportRenderDeliveryResult(
                false,
                false,
                frame.Composite.Generation,
                0,
                exception);
        }
    }
}
