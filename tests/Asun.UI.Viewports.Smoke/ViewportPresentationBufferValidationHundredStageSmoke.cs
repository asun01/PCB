using System.Drawing;
using Asun.UI.Viewports;

public static class ViewportPresentationBufferValidationHundredStageSmoke
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
            using var buffers = new ViewportPresentationBufferRuntime();
            Check(
                ViewportPresentationBufferValidationRuntime.IsValid(
                    buffers.Snapshot),
                $"initial buffers {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var buffers = new ViewportPresentationBufferRuntime();
            var token = new ViewportPresentationSubmissionToken(i + 1, i + 1);
            var tx = buffers.Begin(token, 2, new[]
            {
                new RectangleF(0, 0, 20, 20)
            });
            Check(
                ViewportPresentationBufferValidationRuntime.IsValid(
                    buffers.Snapshot) &&
                buffers.Snapshot.RenderingSlot is not null,
                $"rendering buffer {i + 1} should validate.");
            buffers.Discard(tx);
        }

        for (var i = 0; i < 10; i++)
        {
            using var buffers = new ViewportPresentationBufferRuntime();
            var token = new ViewportPresentationSubmissionToken(i + 1, i + 1);
            var tx = buffers.Begin(token, 2);
            buffers.Commit(tx, 2);
            Check(
                buffers.Snapshot.PresentedSlot is not null &&
                ViewportPresentationBufferValidationRuntime.IsValid(
                    buffers.Snapshot),
                $"presented buffer {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = new ViewportPresentationBufferSnapshot(
                ViewportPresentationBufferState.Rendering,
                ViewportPresentationBufferState.Available,
                null,
                null,
                1, 1,
                null, null,
                2, 1,
                Array.Empty<RectangleF>());

            Check(
                ViewportPresentationBufferValidationRuntime.Validate(snapshot).Any(
                    item => item.Contains("requires an owning slot index", StringComparison.Ordinal)),
                $"orphan rendering metadata {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = new ViewportPresentationBufferSnapshot(
                ViewportPresentationBufferState.Presented,
                ViewportPresentationBufferState.Available,
                null,
                null,
                null, null,
                1, 1,
                2, 1,
                Array.Empty<RectangleF>());

            Check(
                ViewportPresentationBufferValidationRuntime.Validate(snapshot).Any(
                    item => item.Contains("requires a presented slot", StringComparison.Ordinal)),
                $"orphan presented metadata {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = new ViewportPresentationBufferSnapshot(
                ViewportPresentationBufferState.Available,
                ViewportPresentationBufferState.Available,
                0,
                0,
                null, null,
                null, null,
                0, 0,
                Array.Empty<RectangleF>());

            Check(
                ViewportPresentationBufferValidationRuntime.Validate(snapshot).Any(
                    item => item.Contains("cannot be identical", StringComparison.Ordinal)),
                $"same rendering/presented slot {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            var snapshot = new ViewportPresentationBufferSnapshot(
                ViewportPresentationBufferState.Available,
                ViewportPresentationBufferState.Available,
                null, null, null, null, null, null,
                1, 2, Array.Empty<RectangleF>());

            Check(
                ViewportPresentationBufferValidationRuntime.Validate(snapshot).Any(
                    item => item.Contains("cannot exceed planned", StringComparison.Ordinal)),
                $"buffer unit mismatch {i + 1} should fail.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var buffers = new ViewportPresentationBufferRuntime();
            var tx1 = buffers.Begin(new ViewportPresentationSubmissionToken(1, 1), 1);
            buffers.Commit(tx1, 1);
            var tx2 = buffers.Begin(new ViewportPresentationSubmissionToken(2, 2), 2);
            buffers.Commit(tx2, 1);
            Check(
                buffers.Snapshot.PresentedGeneration == 2 &&
                buffers.Snapshot.PresentedSequence == 2 &&
                ViewportPresentationBufferValidationRuntime.IsValid(
                    buffers.Snapshot),
                $"buffer generation progression {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var buffers = new ViewportPresentationBufferRuntime();
            var token = new ViewportPresentationSubmissionToken(1, 1);
            var tx = buffers.Begin(token, 1);
            buffers.Discard(tx);
            Check(
                buffers.Snapshot.RenderingSlot is null &&
                ViewportPresentationBufferValidationRuntime.IsValid(
                    buffers.Snapshot),
                $"discard cleanup {i + 1} should validate.");
        }

        for (var i = 0; i < 10; i++)
        {
            using var buffers = new ViewportPresentationBufferRuntime();
            var token = new ViewportPresentationSubmissionToken(i + 1, i + 1);
            var tx = buffers.Begin(token, 1);
            buffers.Commit(tx, 1);
            buffers.Reset();
            Check(
                buffers.Snapshot.PresentedSlot is null &&
                ViewportPresentationBufferValidationRuntime.IsValid(
                    buffers.Snapshot),
                $"buffer reset {i + 1} should return to a valid empty state.");
        }

        assert(
            round == 100,
            $"Presentation buffer validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
