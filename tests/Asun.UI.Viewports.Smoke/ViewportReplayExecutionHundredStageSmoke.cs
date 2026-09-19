using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplayExecutionHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var events = new[]
        {
            new ViewportInputEvent(1, ViewportInputEventKind.PointerMove, new Vector2(80, 90), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(2, ViewportInputEventKind.PointerDown, new Vector2(80, 90), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(3, ViewportInputEventKind.PointerMove, new Vector2(120, 130), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(4, ViewportInputEventKind.PointerUp, new Vector2(120, 130), 0, ViewportMouseButton.Left),
            new ViewportInputEvent(5, ViewportInputEventKind.Wheel, new Vector2(140, 140), 120, ViewportMouseButton.Left),
            new ViewportInputEvent(6, ViewportInputEventKind.PointerDown, new Vector2(140, 140), 0, ViewportMouseButton.Middle),
            new ViewportInputEvent(7, ViewportInputEventKind.PointerMove, new Vector2(150, 145), 0, ViewportMouseButton.Middle),
            new ViewportInputEvent(8, ViewportInputEventKind.PointerUp, new Vector2(150, 145), 0, ViewportMouseButton.Middle)
        };

        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        static (ViewportCompositeRuntime<string> Composite, ViewportCompositeInputRuntime<string> Input)
            CreateRuntime()
        {
            var composite = new ViewportCompositeRuntime<string>(
                new Vector2(1600, 1200),
                new Vector2(500, 400),
                new Vector2(100, 100),
                1,
                64,
                4,
                new LocalTileSource());

            composite.AddRoi(
                RoiGeometry.CreateRectangle(
                    new Vector2(180, 160),
                    new Vector2(80, 60)));

            return (
                composite,
                new ViewportCompositeInputRuntime<string>(composite));
        }

        for (var i = 0; i < 10; i++)
        {
            var first = CreateRuntime();
            var second = CreateRuntime();

            using (first.Composite)
            using (second.Composite)
            {
                var firstReport = ViewportReplayExecutionRuntime.Execute(
                    first.Input,
                    events);

                var secondReport = ViewportReplayExecutionRuntime.Execute(
                    second.Input,
                    events);

                Check(
                    ViewportReplayExecutionRuntime.HasSameInput(firstReport, secondReport) &&
                    ViewportReplayExecutionRuntime.HasSameResult(firstReport, secondReport) &&
                    firstReport.Count == events.Length &&
                    secondReport.Count == events.Length,
                    $"deterministic execution {i + 1} should preserve input/result hashes and event counts.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var runtime = CreateRuntime();

            using (runtime.Composite)
            {
                var report = ViewportReplayExecutionRuntime.Execute(
                    runtime.Input,
                    events);

                Check(
                    report.InitialGeneration < report.FinalGeneration &&
                    report.TransformChanges > 0 &&
                    report.DirtyEvents > 0 &&
                    report.InputHash.Length == 64 &&
                    report.ResultHash.Length == 64,
                    $"execution metrics {i + 1} should expose deterministic generation/hash data.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var duplicateSequence = events[..2]
                .Select((item, index) =>
                    index == 1
                        ? item with { Sequence = 1 }
                        : item)
                .ToArray();

            var rejected = false;
            var runtime = CreateRuntime();

            using (runtime.Composite)
            {
                try
                {
                    ViewportReplayExecutionRuntime.Execute(
                        runtime.Input,
                        duplicateSequence);
                }
                catch (InvalidOperationException)
                {
                    rejected = true;
                }

                Check(
                    rejected,
                    $"sequence guard {i + 1} should reject duplicate event sequences.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var runtime = CreateRuntime();

            using (runtime.Composite)
            {
                var report = ViewportReplayExecutionRuntime.Execute(
                    runtime.Input,
                    Array.Empty<ViewportInputEvent>());

                Check(
                    report.IsEmpty &&
                    report.Count == 0 &&
                    report.InitialGeneration == report.FinalGeneration &&
                    report.InputHash.Length == 64 &&
                    report.ResultHash.Length == 64,
                    $"empty execution {i + 1} should remain side-effect free.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var runtime = CreateRuntime();
            var bundle = CreateBundle(events);

            using (runtime.Composite)
            {
                var report = ViewportReplayExecutionRuntime.Execute(
                    runtime.Input,
                    bundle);

                Check(
                    report.Count == events.Length &&
                    report.InputHash.Length == 64 &&
                    ViewportReplaySessionBundleRuntime.Validate(bundle).Count == 0,
                    $"bundle execution {i + 1} should accept a valid bundle.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var runtime = CreateRuntime();

            using (runtime.Composite)
            {
                var bundle = CreateBundle(events);
                var report = ViewportReplayExecutionRuntime.Execute(
                    runtime.Input,
                    bundle);

                Check(
                    report.TransformChanges <= report.Count &&
                    report.DocumentChanges <= report.Count &&
                    report.SelectionChanges <= report.Count &&
                    report.DirtyEvents <= report.Count,
                    $"execution counters {i + 1} should stay bounded by event count.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var runtime = CreateRuntime();

            using (runtime.Composite)
            {
                var report = ViewportReplayExecutionRuntime.Execute(
                    runtime.Input,
                    events);

                Check(
                    report.Results.All(result =>
                        float.IsFinite(result.ViewportPoint.X) &&
                        float.IsFinite(result.ViewportPoint.Y) &&
                        float.IsFinite(result.ImagePoint.X) &&
                        float.IsFinite(result.ImagePoint.Y)),
                    $"replay result coordinates {i + 1} should remain finite.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var reordered = events.Reverse().ToArray();
            var rejected = false;
            var runtime = CreateRuntime();

            using (runtime.Composite)
            {
                try
                {
                    ViewportReplayExecutionRuntime.Execute(
                        runtime.Input,
                        reordered);
                }
                catch (InvalidOperationException)
                {
                    rejected = true;
                }

                Check(
                    rejected,
                    $"ordering guard {i + 1} should reject reversed replay input.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var first = CreateRuntime();
            var second = CreateRuntime();

            using (first.Composite)
            using (second.Composite)
            {
                var firstReport = ViewportReplayExecutionRuntime.Execute(
                    first.Input,
                    events);

                var secondReport = ViewportReplayExecutionRuntime.Execute(
                    second.Input,
                    events);

                Check(
                    firstReport.ResultHash == secondReport.ResultHash &&
                    firstReport.InputHash == secondReport.InputHash &&
                    firstReport.FinalGeneration == secondReport.FinalGeneration,
                    $"cross-runtime result hash {i + 1} should be stable.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var invalid = new[]
            {
                events[0] with
                {
                    Position = new Vector2(float.NaN, 0)
                },
                events[1] with
                {
                    Sequence = 2
                }
            };

            var rejected = false;
            var runtime = CreateRuntime();

            using (runtime.Composite)
            {
                try
                {
                    ViewportReplayExecutionRuntime.Execute(
                        runtime.Input,
                        invalid);
                }
                catch (ArgumentOutOfRangeException)
                {
                    rejected = true;
                }

                Check(
                    rejected,
                    $"finite-coordinate guard {i + 1} should reject invalid points.");
            }
        }

        Check(
            round == 100,
            $"Replay execution smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private static ViewportReplaySessionBundle CreateBundle(
        IReadOnlyList<ViewportInputEvent> inputs)
    {
        var manifest = ViewportReplaySessionBundleRuntime.CreateManifest(
            "replay-execution-test",
            DateTimeOffset.UnixEpoch,
            inputs,
            Array.Empty<ViewportRenderEvidenceManifest>(),
            Array.Empty<ViewportPresentationAuditEvent>());

        return ViewportReplaySessionBundleRuntime.Capture(
            manifest,
            inputs,
            Array.Empty<ViewportRenderEvidenceManifest>(),
            Array.Empty<ViewportPresentationAuditEvent>());
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
