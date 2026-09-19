using System.Drawing;
using Asun.UI.Viewports;

public static class ViewportRenderSurfaceValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        for (var i = 0; i < 10; i++)
        {
            using var surface = new ViewportRenderSurfaceRuntime();
            Check(
                ViewportRenderSurfaceValidationRuntime.IsValid(
                    surface.Snapshot),
                $"idle surface {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var surface = new ViewportRenderSurfaceRuntime();
            var transaction = surface.Begin(i + 1);
            Check(
                ViewportRenderSurfaceValidationRuntime.IsValid(
                    surface.Snapshot),
                $"rendering surface {i + 1} should validate.");
            surface.Discard(transaction);
        }

        for (var i = 0; i < 10; i++)
        {
            using var surface = new ViewportRenderSurfaceRuntime();
            var transaction = surface.Begin(i + 1);
            surface.Commit(transaction, 2, 2, new[]
            {
                new RectangleF(0, 0, 20, 20)
            });
            var snapshot = surface.Snapshot;
            Check(
                snapshot.State == ViewportRenderSurfaceState.Presented &&
                ViewportRenderSurfaceValidationRuntime.IsValid(snapshot),
                $"presented surface {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = new ViewportRenderSurfaceSnapshot(
                ViewportRenderSurfaceState.Presented,
                null, null, 1, null, null,
                1, 1, 1,
                Array.Empty<RectangleF>());

            Check(
                ViewportRenderSurfaceValidationRuntime.Validate(snapshot).Any(
                    item => item.Contains(
                        "Presented surface must expose",
                        StringComparison.Ordinal)),
                $"invalid presented snapshot {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = new ViewportRenderSurfaceSnapshot(
                ViewportRenderSurfaceState.Rendering,
                null, null, null, null, null,
                0, 0, 0,
                Array.Empty<RectangleF>());

            Check(
                ViewportRenderSurfaceValidationRuntime.Validate(snapshot).Any(
                    item => item.Contains(
                        "Rendering surface must expose",
                        StringComparison.Ordinal)),
                $"invalid rendering snapshot {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = new ViewportRenderSurfaceSnapshot(
                ViewportRenderSurfaceState.Idle,
                null, null, null, null, null,
                0, 3, 2,
                Array.Empty<RectangleF>());

            Check(
                ViewportRenderSurfaceValidationRuntime.Validate(snapshot).Any(
                    item => item.Contains(
                        "cannot exceed planned",
                        StringComparison.Ordinal)),
                $"invalid surface units {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = new ViewportRenderSurfaceSnapshot(
                ViewportRenderSurfaceState.Idle,
                null, null, null, null, null,
                0, 0, 0,
                new[]
                {
                    new RectangleF(float.NaN, 0, 10, 10)
                });

            Check(
                ViewportRenderSurfaceValidationRuntime.Validate(snapshot).Any(
                    item => item.Contains("region", StringComparison.OrdinalIgnoreCase)),
                $"non-finite region {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var surface = new ViewportRenderSurfaceRuntime();
            var transaction = surface.Begin(i + 1);
            surface.Discard(
                transaction,
                ViewportRenderDeliveryStatus.Cancelled);
            Check(
                surface.Snapshot.State ==
                    ViewportRenderSurfaceState.Discarded &&
                ViewportRenderSurfaceValidationRuntime.IsValid(
                    surface.Snapshot),
                $"discarded surface {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var surface = new ViewportRenderSurfaceRuntime();
            var transaction = surface.Begin(i + 1);
            surface.Commit(transaction, 1, 1);
            surface.Reset();
            Check(
                surface.Snapshot.State == ViewportRenderSurfaceState.Idle &&
                ViewportRenderSurfaceValidationRuntime.IsValid(
                    surface.Snapshot),
                $"reset surface {i + 1} should return to valid idle state.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var surface = new ViewportRenderSurfaceRuntime();
            var first = surface.Begin(i + 1);
            surface.Commit(first, 1, 1);

            var rejected = false;
            try
            {
                surface.Begin(i);
            }
            catch (InvalidOperationException)
            {
                rejected = true;
            }

            Check(
                rejected &&
                ViewportRenderSurfaceValidationRuntime.IsValid(
                    surface.Snapshot),
                $"stale surface generation {i + 1} should be rejected cleanly.");
        }

        assert(
            round == 100,
            $"Render surface validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
