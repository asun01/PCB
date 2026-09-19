using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportCompositeSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        for (var i = 0; i < 500; i++)
        {
            using var runtime = new ViewportCompositeRuntime<string>(
                imageSize: new Vector2(
                    2000f + (i % 17) * 31f,
                    1500f + (i % 13) * 23f),
                viewportSize: new Vector2(
                    500f + (i % 5) * 20f,
                    400f + (i % 7) * 15f),
                tileSize: new Vector2(100, 100),
                prefetchMarginTiles: 1,
                cacheCapacity: 64,
                maxConcurrency: 4,
                tileSource: new LocalTileSource());

            var center = new Vector2(
                250f + (i % 17) * 13f,
                220f + (i % 11) * 9f);

            runtime.AddRoi(
                RoiGeometry.CreateRectangle(
                    center,
                    new Vector2(
                        80f + (i % 5) * 6f,
                        60f + (i % 7) * 4f)));

            runtime.DuplicateSelected(
                new Vector2(120f + i % 9, 90f + i % 7));

            runtime.PanBy(
                new Vector2(
                    8f - i % 5,
                    -4f + i % 3));

            runtime.ZoomAt(
                1.01 + (i % 3) * 0.01,
                0.01,
                20,
                runtime.Transform.ViewportCenter);

            var cached = runtime.CreateCachedFrame();

            assert(
                cached.Roi.Document.Items.Count == 2 &&
                cached.RoiCommands.Count > 0 &&
                cached.SceneCommands.Count > 0,
                $"Composite chain {i + 1} should build ROI and scene render commands.");

            assert(
                cached.Tiles.Transform == cached.Roi.Transform,
                $"Composite chain {i + 1} should share one viewport transform.");

            var frame = await runtime.RefreshAsync(
                includePrefetch: i % 2 == 0);

            var metrics = ViewportRenderFrameRuntime.Capture(frame);

            assert(
                metrics.RequestedTiles > 0 &&
                metrics.LoadedTiles > 0 &&
                metrics.VisibleTiles > 0 &&
                metrics.RoiCommands > 0 &&
                metrics.SceneCommands > 0 &&
                metrics.SceneDiffs >= 0 &&
                metrics.Complete,
                $"Composite chain {i + 1} should finish the image-to-render frame.");

            assert(
                frame.Tiles.Transform == frame.Roi.Transform &&
                frame.Generation >= 3,
                $"Composite chain {i + 1} should preserve generation and transform consistency.");

            var consumed = runtime.ConsumeDirtyFlags();
            assert(
                consumed != ViewportDirtyFlags.None &&
                !runtime.DirtyRuntime.IsDirty,
                $"Composite chain {i + 1} should expose and consume render dirtiness.");

            runtime.PanBy(new Vector2(5, 2));
            var refreshed = await runtime.RefreshAsync();
            assert(
                refreshed.Tiles.Transform == refreshed.Roi.Transform &&
                refreshed.SceneDiff.Count > 0 &&
                refreshed.IsReady,
                $"Composite chain {i + 1} should refresh after navigation.");
        }
    }

    private sealed class LocalTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            RectangleF imageRectangle,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(
                $"tile:{request.Index.X},{request.Index.Y}:{imageRectangle.Width:0.###}x{imageRectangle.Height:0.###}");
    }
}
