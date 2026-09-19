using System.Drawing;

namespace Asun.UI.Viewports;

public static class ViewportRenderAdapterRuntime
{
    public static async ValueTask<int> RenderAsync<TTile>(
        ViewportRenderPipelineFrame<TTile> frame,
        IViewportRenderSink<TTile> sink,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(sink);

        var transform = frame.Composite.Tiles.Transform;
        var viewportBounds = new RectangleF(
            0,
            0,
            transform.ViewportSize.X,
            transform.ViewportSize.Y);

        var context = new ViewportRenderFrameContext(
            frame.Composite.Generation,
            viewportBounds,
            frame.Composite.DirtyFlags,
            frame.WorkPlan.Items.Count);

        await sink
            .BeginFrameAsync(context, cancellationToken)
            .ConfigureAwait(false);

        try
        {
            var visibility = ViewportTileRoiVisibilityRuntime.Build(
            frame.Composite);

        var visibleRoiIds = visibility.VisibleRoiIds;

        var roiCommandsById = frame.Composite.RoiCommands
            .Where(command => visibleRoiIds.Contains(command.RoiId))
            .GroupBy(command => command.RoiId)
            .ToDictionary(group => group.Key, group => group.ToArray());

        var renderedRoiIds = new HashSet<Guid>();
        var renderedUnits = 0;

        foreach (var work in frame.WorkPlan.Items)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (work.Kind == ViewportRenderWorkKind.FullSurface)
            {
                await sink
                    .ClearInvalidatedRegionAsync(
                        new ViewportRenderInvalidationContext(
                            viewportBounds,
                            frame.Composite.Generation),
                        cancellationToken)
                    .ConfigureAwait(false);

                renderedUnits++;
                continue;
            }

            if (work.IsInvalidation)
            {
                var invalidationBounds = ViewportRenderRegionRuntime.ClipToViewport(
                    work.Bounds,
                    transform);

                if (!invalidationBounds.IsEmpty)
                {
                    await sink
                        .ClearInvalidatedRegionAsync(
                            new ViewportRenderInvalidationContext(
                                invalidationBounds,
                                frame.Composite.Generation),
                            cancellationToken)
                        .ConfigureAwait(false);

                    renderedUnits++;
                }

                continue;
            }

            if (work.Kind == ViewportRenderWorkKind.Overlay)
            {
                await sink
                    .DrawOverlayAsync(
                        new ViewportRenderOverlayContext(
                            work.Bounds,
                            frame.Composite.DirtyFlags,
                            frame.Composite.Generation),
                        cancellationToken)
                    .ConfigureAwait(false);

                renderedUnits++;
                continue;
            }

            if (work.Kind == ViewportRenderWorkKind.Tile &&
                work.Tile is TileIndex tileIndex &&
                frame.Composite.Tiles.TryGetTile(tileIndex, out var tile))
            {
                var request = frame.Composite.Tiles.Requests
                    .FirstOrDefault(item => item.Index == tileIndex);

                var imageRectangle = TileRequestPlanner.GetRequestRectangle(
                    transform.ImageSize,
                    frame.Composite.Tiles.TileSize,
                    request);

                await sink
                    .DrawTileAsync(
                        new ViewportRenderTileContext<TTile>(
                            tileIndex,
                            request,
                            tile,
                            transform.ImageToViewportRectangle(imageRectangle),
                            frame.Composite.Generation),
                        cancellationToken)
                    .ConfigureAwait(false);

                renderedUnits++;
                continue;
            }

            if (work.Kind == ViewportRenderWorkKind.Tile &&
                work.Tile is TileIndex unavailableTile)
            {
                throw new ViewportRenderWorkUnavailableException(work);
            }

            if (work.Kind != ViewportRenderWorkKind.Roi ||
                work.RoiId == Guid.Empty ||
                !renderedRoiIds.Add(work.RoiId) ||
                !roiCommandsById.TryGetValue(work.RoiId, out var commands))
            {
                continue;
            }

            foreach (var command in commands)
            {
                var clipped = ViewportRenderRegionRuntime.ClipToViewport(
                    command.Bounds,
                    transform);

                if (clipped.IsEmpty)
                    continue;

                await sink
                    .DrawRoiAsync(
                        new ViewportRenderRoiContext(
                            command,
                            clipped,
                            frame.Composite.Generation),
                        cancellationToken)
                    .ConfigureAwait(false);
            }
        }
        finally
        {
            await sink
                .EndFrameAsync(context, CancellationToken.None)
                .ConfigureAwait(false);
        }

        return renderedUnits + renderedRoiIds.Count;
    }
}
