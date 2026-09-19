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

        using var buffers = new ViewportPresentationBufferRuntime();

        var first = buffers.Begin(
            new ViewportPresentationSubmissionToken(10, 1),
            3);
        var initial = buffers.Snapshot;

        var doubleBeginRejected = false;
        try
        {
            buffers.Begin(
                new ViewportPresentationSubmissionToken(10, 2),
                1);
        }
        catch (InvalidOperationException)
        {
            doubleBeginRejected = true;
        }

        buffers.Commit(first, 3);
        var committed = buffers.Snapshot;

        var second = buffers.Begin(
            new ViewportPresentationSubmissionToken(20, 2),
            2);

        var overBudgetRejected = false;
        try
        {
            buffers.Commit(second, 3);
        }
        catch (ArgumentOutOfRangeException)
        {
            overBudgetRejected = true;
            buffers.Discard(second);
        }

        var afterOverBudget = buffers.Snapshot;

        buffers.Reset();
        var reset = buffers.Snapshot;

        var staleAfterResetRejected = false;
        try
        {
            buffers.Begin(
                new ViewportPresentationSubmissionToken(20, 1),
                1);
        }
        catch (InvalidOperationException)
        {
            staleAfterResetRejected = true;
        }

        var postReset = buffers.Begin(
            new ViewportPresentationSubmissionToken(30, 7),
            1);
        buffers.Commit(postReset, 1);
        var postResetSnapshot = buffers.Snapshot;

        buffers.Dispose();
        var disposed = buffers.Snapshot;

        for (var i = 0; i < 10; i++)
        {
            Check(
                ViewportPresentationBufferValidationRuntime.IsValid(initial),
                $"initial validator round {i + 1} should pass.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                doubleBeginRejected &&
                initial.RenderingSlot == first.SlotIndex &&
                initial.RenderingSequence == 1,
                $"initial rendering fence round {i + 1} should be coherent.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                ViewportPresentationBufferValidationRuntime.IsValid(committed) &&
                committed.PresentedSequence == 1,
                $"committed state round {i + 1} should preserve the published fence.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                overBudgetRejected,
                $"rendered-unit budget round {i + 1} should reject impossible commits.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                ViewportPresentationBufferValidationRuntime.IsValid(afterOverBudget) &&
                afterOverBudget.PresentedSequence == 1,
                $"post-rejection state round {i + 1} should preserve the previous presentation.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                ViewportPresentationBufferValidationRuntime.IsValid(reset) &&
                reset.PresentedSlot is null &&
                reset.RenderingSlot is null &&
                buffers.Statistics.Acquired == 0,
                $"reset state round {i + 1} should clear lifecycle counters.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                staleAfterResetRejected,
                $"pre-reset submission round {i + 1} should remain invalid.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                ViewportPresentationBufferValidationRuntime.IsValid(postResetSnapshot) &&
                postResetSnapshot.PresentedSequence == 7,
                $"post-reset submission round {i + 1} should commit cleanly.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                ViewportPresentationBufferValidationRuntime.IsValid(disposed) &&
                disposed.FirstState == ViewportPresentationBufferState.Disposed &&
                disposed.SecondState == ViewportPresentationBufferState.Disposed,
                $"disposed state round {i + 1} should remain structurally valid.");
        }

        assert(
            round == 100,
            $"Presentation buffer lifecycle smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
