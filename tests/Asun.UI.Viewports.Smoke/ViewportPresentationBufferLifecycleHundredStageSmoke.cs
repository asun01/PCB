using Asun.UI.Viewports;

public static class ViewportPresentationBufferLifecycleHundredStageSmoke
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

            var first = buffers.Begin(
                new ViewportPresentationSubmissionToken(10 + i, 1),
                3);

            Check(
                ViewportPresentationBufferValidationRuntime.IsValid(buffers.Snapshot),
                $"initial buffer state {i + 1} should satisfy the validator.");

            Check(
                buffers.Snapshot.RenderingSlot == first.SlotIndex &&
                buffers.Snapshot.RenderingSequence == 1,
                $"initial rendering fence {i + 1} should be visible.");

            var doubleBeginRejected = false;
            try
            {
                buffers.Begin(
                    new ViewportPresentationSubmissionToken(10 + i, 2),
                    1);
            }
            catch (InvalidOperationException)
            {
                doubleBeginRejected = true;
            }

            Check(
                doubleBeginRejected,
                $"concurrent backbuffer acquisition {i + 1} should be rejected.");

            buffers.Commit(first, 3);

            Check(
                ViewportPresentationBufferValidationRuntime.IsValid(buffers.Snapshot),
                $"committed buffer state {i + 1} should remain valid.");

            Check(
                buffers.Snapshot.PresentedSlot == first.SlotIndex &&
                buffers.Snapshot.PresentedSequence == 1,
                $"presented fence {i + 1} should match the committed token.");

            var overBudgetRejected = false;
            var second = buffers.Begin(
                new ViewportPresentationSubmissionToken(20 + i, 2),
                2);

            try
            {
                buffers.Commit(second, 3);
            }
            catch (ArgumentOutOfRangeException)
            {
                overBudgetRejected = true;
                buffers.Discard(second);
            }

            Check(
                overBudgetRejected &&
                ViewportPresentationBufferValidationRuntime.IsValid(buffers.Snapshot),
                $"rendered-unit budget {i + 1} should reject impossible commits.");

            buffers.Reset();

            Check(
                ViewportPresentationBufferValidationRuntime.IsValid(buffers.Snapshot) &&
                buffers.Statistics.Acquired == 0 &&
                buffers.Statistics.Committed == 0,
                $"reset state {i + 1} should clear counters and slots.");

            var staleAfterResetRejected = false;
            try
            {
                buffers.Begin(
                    new ViewportPresentationSubmissionToken(20 + i, 1),
                    1);
            }
            catch (InvalidOperationException)
            {
                staleAfterResetRejected = true;
            }

            Check(
                staleAfterResetRejected,
                $"pre-reset submission fence {i + 1} should remain invalid.");

            var postReset = buffers.Begin(
                new ViewportPresentationSubmissionToken(30 + i, 7),
                1);
            buffers.Commit(postReset, 1);

            Check(
                ViewportPresentationBufferValidationRuntime.IsValid(buffers.Snapshot) &&
                buffers.Snapshot.PresentedSequence == 7,
                $"post-reset presentation fence {i + 1} should commit cleanly.");

            buffers.Dispose();

            Check(
                ViewportPresentationBufferValidationRuntime.IsValid(buffers.Snapshot) &&
                buffers.Snapshot.FirstState ==
                    ViewportPresentationBufferState.Disposed &&
                buffers.Snapshot.SecondState ==
                    ViewportPresentationBufferState.Disposed,
                $"disposed buffer state {i + 1} should remain structurally valid.");
        }

        assert(
            round == 100,
            $"Presentation buffer lifecycle smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
