using Asun.UI.Viewports;

public static class ViewportRenderDeliveryValidationHundredStageSmoke
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
            var result = new ViewportRenderDeliveryResult(
                true, false, false, i, 2, 0,
                Array.Empty<ViewportRenderWorkItem>(),
                null, 2, 1);

            Check(
                ViewportRenderDeliveryValidationRuntime.IsValid(result),
                $"successful delivery {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var result = new ViewportRenderDeliveryResult(
                true, false, false, i, 3, 0,
                Array.Empty<ViewportRenderWorkItem>(),
                null, 3, 1);

            Check(
                ViewportRenderDeliveryValidationRuntime.Validate(result).Count == 0,
                $"successful unit accounting {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var result = new ViewportRenderDeliveryResult(
                false, false, true, i, 1, 1,
                new[]
                {
                    new ViewportRenderWorkItem(
                        ViewportRenderWorkKind.Tile,
                        new System.Drawing.RectangleF(0, 0, 10, 10),
                        Guid.Empty,
                        new TileIndex(0, 0),
                        i)
                },
                new ViewportRenderWorkUnavailableException(
                    new ViewportRenderWorkItem(
                        ViewportRenderWorkKind.Tile,
                        new System.Drawing.RectangleF(0, 0, 10, 10),
                        Guid.Empty,
                        new TileIndex(0, 0),
                        i),
                    1),
                2,
                1);

            Check(
                ViewportRenderDeliveryValidationRuntime.IsValid(result),
                $"deferred delivery {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var result = new ViewportRenderDeliveryResult(
                false, true, false, i, 0, 0,
                Array.Empty<ViewportRenderWorkItem>(),
                null, 2, 1);

            Check(
                ViewportRenderDeliveryValidationRuntime.IsValid(result),
                $"cancelled delivery {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var result = new ViewportRenderDeliveryResult(
                false, false, false, i, 0, 0,
                Array.Empty<ViewportRenderWorkItem>(),
                new InvalidOperationException("failure"),
                2, 1);

            Check(
                ViewportRenderDeliveryValidationRuntime.IsValid(result),
                $"failed delivery {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var result = new ViewportRenderDeliveryResult(
                true, false, false, i, 3, 0,
                Array.Empty<ViewportRenderWorkItem>(),
                null, 2, 1);

            Check(
                ViewportRenderDeliveryValidationRuntime.Validate(result).Any(
                    item => item.Contains("exceed planned", StringComparison.Ordinal)),
                $"rendered-over-planned mutation {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var result = new ViewportRenderDeliveryResult(
                false, false, true, i, 1, 0,
                new[]
                {
                    new ViewportRenderWorkItem(
                        ViewportRenderWorkKind.Tile,
                        new System.Drawing.RectangleF(0, 0, 10, 10),
                        Guid.Empty,
                        new TileIndex(0, 0),
                        i)
                },
                null, 2, 1);

            Check(
                ViewportRenderDeliveryValidationRuntime.Validate(result).Any(
                    item => item.Contains("Deferred unit count", StringComparison.Ordinal)),
                $"deferred count mismatch {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var result = new ViewportRenderDeliveryResult(
                true, true, false, i, 0, 0,
                Array.Empty<ViewportRenderWorkItem>(),
                null, 0, 0);

            Check(
                ViewportRenderDeliveryValidationRuntime.Validate(result).Any(
                    item => item.Contains("successful delivery", StringComparison.Ordinal)),
                $"success-and-cancelled mutation {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var result = new ViewportRenderDeliveryResult(
                false, false, false, i, 0, 0,
                Array.Empty<ViewportRenderWorkItem>(),
                null, 0, 0);

            Check(
                ViewportRenderDeliveryValidationRuntime.Validate(result).Any(
                    item => item.Contains("failed delivery", StringComparison.Ordinal)),
                $"failed-without-error mutation {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var pipeline = new ViewportRenderPipelineRuntime<string>(
                new System.Numerics.Vector2(800, 600),
                new System.Numerics.Vector2(400, 300),
                new System.Numerics.Vector2(100, 100),
                1, 16, 2, new LocalTileSource());
            using var surface = new ViewportRenderSurfaceRuntime();
            var tracker = new ViewportRenderDeliveryTracker();
            var sink = new ViewportRenderReplaySink<string>();

            pipeline.Invalidate(
                ViewportDirtyFlags.All,
                pipeline.Composite.Generation);

            var first =
                pipeline.RefreshAsync(
                    DateTimeOffset.UtcNow.AddSeconds(1))
                    .GetAwaiter()
                    .GetResult();

            var interaction =
                new ViewportCompositeInputRuntime<string>(
                    pipeline.Composite);

            interaction.Apply(
                new ViewportInputEvent(
                    1,
                    ViewportInputEventKind.Wheel,
                    new System.Numerics.Vector2(100, 100),
                    120,
                    ViewportMouseButton.Left));

            pipeline.Invalidate(
                ViewportDirtyFlags.All,
                pipeline.Composite.Generation);

            var second =
                pipeline.RefreshAsync(
                    DateTimeOffset.UtcNow.AddSeconds(1))
                    .GetAwaiter()
                    .GetResult();

            var newer =
                ViewportRenderDeliveryRuntime.TryDeliverAsync(
                    second!,
                    sink,
                    tracker,
                    default,
                    surface)
                .GetAwaiter()
                .GetResult();

            var older =
                ViewportRenderDeliveryRuntime.TryDeliverAsync(
                    first!,
                    sink,
                    tracker,
                    default,
                    surface)
                .GetAwaiter()
                .GetResult();

            Check(
                newer.Generation >= older.Generation &&
                tracker.Statistics.LastGeneration == newer.Generation,
                $"delivery tracker generation monotonicity {i + 1} should hold.");
        }

        assert(
            round == 100,
            $"Render delivery validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
