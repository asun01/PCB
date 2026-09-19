using System.Drawing;
using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportReplayVerificationHundredStageSmoke
{
    private static readonly Guid StableRoiId =
        Guid.Parse("11111111-2222-3333-4444-555555555555");

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

        static ViewportCompositeInputRuntime<string> CreateInput()
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
                    new Vector2(80, 60)),
                StableRoiId);

            return new ViewportCompositeInputRuntime<string>(composite);
        }

        static ViewportReplaySessionBundle CreateBundle(
            IReadOnlyList<ViewportInputEvent> inputs)
        {
            var manifest = ViewportReplaySessionBundleRuntime.CreateManifest(
                "verification-test",
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

        for (var i = 0; i < 10; i++)
        {
            var expectedInput = CreateInput();
            var actualInput = CreateInput();

            using (expectedInput.Composite)
            using (actualInput.Composite)
            {
                var expected =
                    ViewportReplayExecutionStateRuntime.Execute(
                        expectedInput,
                        events);

                var actual =
                    ViewportReplayExecutionStateRuntime.Execute(
                        actualInput,
                        events);

                var verification =
                    ViewportReplayVerificationRuntime.Verify(
                        expected,
                        actual);

                Check(
                    verification.IsValid &&
                    ViewportReplayVerificationRuntime.IsValid(
                        verification),
                    $"equal replay {i + 1} should pass the verification gate.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var expectedInput = CreateInput();
            var actualInput = CreateInput();

            using (expectedInput.Composite)
            using (actualInput.Composite)
            {
                var expected =
                    ViewportReplayExecutionStateRuntime.Execute(
                        expectedInput,
                        events);

                var changedEvents = events
                    .Select(item =>
                        item with
                        {
                            Position =
                                item.Position +
                                (item.Sequence == 5
                                    ? new Vector2(i + 1, 0)
                                    : Vector2.Zero)
                        })
                    .ToArray();

                var actual =
                    ViewportReplayExecutionStateRuntime.Execute(
                        actualInput,
                        changedEvents);

                var verification =
                    ViewportReplayVerificationRuntime.Verify(
                        expected,
                        actual);

                Check(
                    !verification.IsValid &&
                    !verification.InputMatches &&
                    !verification.ResultMatches &&
                    verification.Differences.Count > 0,
                    $"input mutation {i + 1} should fail the verification gate.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var expectedInput = CreateInput();
            var actualInput = CreateInput();

            using (expectedInput.Composite)
            using (actualInput.Composite)
            {
                var expected =
                    ViewportReplayExecutionStateRuntime.Execute(
                        expectedInput,
                        events);

                var actual =
                    ViewportReplayExecutionStateRuntime.Execute(
                        actualInput,
                        events);

                actualInput.Composite.PanBy(
                    new Vector2(i + 1, 0));

                actual =
                    actual with
                    {
                        FinalState =
                            ViewportReplayStateFingerprintRuntime.Capture(
                                actualInput.Composite)
                    };

                var verification =
                    ViewportReplayVerificationRuntime.Verify(
                        expected,
                        actual);

                Check(
                    !verification.IsValid &&
                    verification.InputMatches &&
                    verification.ResultMatches &&
                    !verification.FinalStateMatches &&
                    verification.Differences.Any(
                        item => item.StartsWith(
                            "FinalState.Transform.",
                            StringComparison.Ordinal)),
                    $"final-state mutation {i + 1} should be isolated by the gate.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var actual =
                    ViewportReplayExecutionStateRuntime.Execute(
                        input,
                        events);

                var bundle = CreateBundle(events);

                var verification =
                    ViewportReplayVerificationRuntime.VerifyInputAgainstBundle(
                        bundle,
                        actual);

                Check(
                    verification.IsValid &&
                    verification.InputMatches &&
                    verification.Differences.Count == 0,
                    $"bundle input gate {i + 1} should accept matching input.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var actual =
                    ViewportReplayExecutionStateRuntime.Execute(
                        input,
                        events);

                var bundle = CreateBundle(
                    events.Select(
                        item => item with
                        {
                            Sequence =
                                item.Sequence +
                                (item.Sequence == 1 ? 10 : 0)
                        }).ToArray());

                var verificationRejected = false;

                try
                {
                    ViewportReplayVerificationRuntime.VerifyInputAgainstBundle(
                        bundle,
                        actual);
                }
                catch (InvalidOperationException)
                {
                    verificationRejected = true;
                }

                Check(
                    verificationRejected,
                    $"invalid bundle gate {i + 1} should reject malformed input ordering.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var input = CreateInput();

            using (input.Composite)
            {
                var actual =
                    ViewportReplayExecutionStateRuntime.Execute(
                        input,
                        events);

                var bundle = CreateBundle(
                    events.Select(
                        item => item with
                        {
                            Position =
                                item.Sequence == 2
                                    ? item.Position +
                                      new Vector2(i + 1, i + 2)
                                    : item.Position
                        }).ToArray());

                var verification =
                    ViewportReplayVerificationRuntime.VerifyInputAgainstBundle(
                        bundle,
                        actual);

                Check(
                    !verification.IsValid &&
                    !verification.InputMatches &&
                    verification.Differences.Count == 1,
                    $"bundle input mismatch {i + 1} should produce one focused difference.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var expectedInput = CreateInput();
            var actualInput = CreateInput();

            using (expectedInput.Composite)
            using (actualInput.Composite)
            {
                var expected =
                    ViewportReplayExecutionStateRuntime.Execute(
                        expectedInput,
                        Array.Empty<ViewportInputEvent>());

                var actual =
                    ViewportReplayExecutionStateRuntime.Execute(
                        actualInput,
                        Array.Empty<ViewportInputEvent>());

                var verification =
                    ViewportReplayVerificationRuntime.Verify(
                        expected,
                        actual);

                Check(
                    verification.IsValid &&
                    !expected.StateChanged &&
                    !actual.StateChanged,
                    $"empty replay gate {i + 1} should remain valid and unchanged.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var first = CreateInput();
            var second = CreateInput();

            using (first.Composite)
            using (second.Composite)
            {
                var expected =
                    ViewportReplayExecutionStateRuntime.Execute(
                        first,
                        events);

                var actual =
                    ViewportReplayExecutionStateRuntime.Execute(
                        second,
                        events);

                var comparison =
                    ViewportReplayStateFingerprintRuntime.Compare(
                        expected.FinalState,
                        actual.FinalState);

                Check(
                    comparison.IsEquivalent &&
                    comparison.Differences.Count == 0 &&
                    ViewportReplayVerificationRuntime
                        .Verify(expected, actual)
                        .IsValid,
                    $"structured state comparison {i + 1} should agree with verification.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var expectedInput = CreateInput();
            var actualInput = CreateInput();

            using (expectedInput.Composite)
            using (actualInput.Composite)
            {
                var expected =
                    ViewportReplayExecutionStateRuntime.Execute(
                        expectedInput,
                        events);

                var actual =
                    ViewportReplayExecutionStateRuntime.Execute(
                        actualInput,
                        events);

                actualInput.Composite.SelectRoi(null);

                actual =
                    actual with
                    {
                        FinalState =
                            ViewportReplayStateFingerprintRuntime.Capture(
                                actualInput.Composite)
                    };

                var verification =
                    ViewportReplayVerificationRuntime.Verify(
                        expected,
                        actual);

                Check(
                    !verification.IsValid &&
                    verification.InputMatches &&
                    verification.ResultMatches &&
                    verification.Differences.Any(
                        item => item.Contains(
                            "Roi.SelectedId",
                            StringComparison.Ordinal)),
                    $"selection mismatch {i + 1} should be reported structurally.");
            }
        }

        for (var i = 0; i < 10; i++)
        {
            var expectedInput = CreateInput();
            var actualInput = CreateInput();

            using (expectedInput.Composite)
            using (actualInput.Composite)
            {
                var expected =
                    ViewportReplayExecutionStateRuntime.Execute(
                        expectedInput,
                        events);

                var actual =
                    ViewportReplayExecutionStateRuntime.Execute(
                        actualInput,
                        events);

                var verification =
                    ViewportReplayVerificationRuntime.Verify(
                        expected,
                        actual);

                Check(
                    verification.InputMatches &&
                    verification.ResultMatches &&
                    verification.FinalStateMatches &&
                    verification.Differences.Count == 0 &&
                    verification.IsValid,
                    $"repeated clean gate {i + 1} should be stable.");
            }
        }

        assert(
            round == 100,
            $"Replay verification smoke should execute exactly 100 numbered rounds; actual {round}.");
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
