using System.Drawing;
using Asun.UI.Viewports;

public static class ViewportPresentationBufferSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        using var buffers = new ViewportPresentationBufferRuntime();

        var regionsA = new[]
        {
            new RectangleF(0, 0, 20, 20),
            new RectangleF(20, 0, 10, 10)
        };

        var tokenA = new ViewportPresentationSubmissionToken(10, 1);
        var first = buffers.Begin(tokenA, 3, regionsA);

        assert(
            first.SlotIndex == 0 &&
            buffers.Snapshot.FirstState ==
            ViewportPresentationBufferState.Rendering &&
            buffers.Snapshot.SecondState ==
            ViewportPresentationBufferState.Available &&
            buffers.Snapshot.RenderingGeneration == 10 &&
            buffers.Snapshot.RenderingSequence == 1,
            "First presentation should acquire the first backbuffer with the submission fence.");

        var secondBeginRejected = false;

        try
        {
            buffers.Begin(
                new ViewportPresentationSubmissionToken(10, 2),
                1);
        }
        catch (InvalidOperationException)
        {
            secondBeginRejected = true;
        }

        assert(
            secondBeginRejected,
            "Double buffering must not allow two render transactions to mutate presentation slots concurrently.");

        buffers.Commit(first, 2, regionsA);

        var firstSnapshot = buffers.Snapshot;

        assert(
            firstSnapshot.PresentedSlot == 0 &&
            firstSnapshot.PresentedGeneration == 10 &&
            firstSnapshot.PresentedSequence == 1 &&
            firstSnapshot.LastPlannedUnits == 3 &&
            firstSnapshot.LastRenderedUnits == 2 &&
            firstSnapshot.PresentedRegions.Count == 2 &&
            firstSnapshot.FirstState ==
            ViewportPresentationBufferState.Presented &&
            firstSnapshot.SecondState ==
            ViewportPresentationBufferState.Available,
            "Committed frame should become the presented buffer with exact incremental regions.");

        var tokenB = new ViewportPresentationSubmissionToken(11, 2);
        var second = buffers.Begin(tokenB, 4, new[]
        {
            new RectangleF(5, 5, 8, 8)
        });

        assert(
            second.SlotIndex == 1 &&
            buffers.Snapshot.FirstState ==
            ViewportPresentationBufferState.Presented &&
            buffers.Snapshot.SecondState ==
            ViewportPresentationBufferState.Rendering,
            "A newer generation should render into the alternate backbuffer while preserving the presented slot.");

        buffers.Commit(
            second,
            4,
            new[]
            {
                new RectangleF(5, 5, 8, 8)
            });

        var secondSnapshot = buffers.Snapshot;

        assert(
            secondSnapshot.PresentedSlot == 1 &&
            secondSnapshot.PresentedGeneration == 11 &&
            secondSnapshot.PresentedSequence == 2 &&
            secondSnapshot.FirstState ==
            ViewportPresentationBufferState.Available &&
            secondSnapshot.SecondState ==
            ViewportPresentationBufferState.Presented,
            "Commit should atomically swap the presented slot and release the previous buffer.");

        var staleRejected = false;

        try
        {
            buffers.Begin(
                new ViewportPresentationSubmissionToken(11, 1),
                1);
        }
        catch (InvalidOperationException)
        {
            staleRejected = true;
        }

        assert(
            staleRejected,
            "An older submission sequence must not acquire a backbuffer over the current presented frame.");

        var tokenC = new ViewportPresentationSubmissionToken(12, 3);
        var third = buffers.Begin(
            tokenC,
            1,
            new[] { new RectangleF(40, 40, 5, 5) });

        buffers.Discard(third);

        var afterDiscard = buffers.Snapshot;

        assert(
            afterDiscard.PresentedSlot == 1 &&
            afterDiscard.PresentedGeneration == 11 &&
            afterDiscard.PresentedSequence == 2 &&
            afterDiscard.FirstState ==
            ViewportPresentationBufferState.Available &&
            afterDiscard.SecondState ==
            ViewportPresentationBufferState.Presented &&
            buffers.Statistics.Discarded == 1,
            "Discarded work must return only the rendering buffer to Available and preserve the last presented frame.");

        buffers.Reset();

        var resetSnapshot = buffers.Snapshot;

        assert(
            resetSnapshot.FirstState == ViewportPresentationBufferState.Available &&
            resetSnapshot.SecondState == ViewportPresentationBufferState.Available &&
            resetSnapshot.PresentedSlot is null &&
            resetSnapshot.RenderingSlot is null &&
            buffers.Statistics.Acquired == 0 &&
            buffers.Statistics.Committed == 0 &&
            buffers.Statistics.RegionCommits == 0,
            "Backbuffer reset should clear presentation state and counters.");

        var postReset = buffers.Begin(
            new ViewportPresentationSubmissionToken(20, 7),
            1);

        buffers.Commit(postReset, 1);

        assert(
            buffers.Snapshot.PresentedGeneration == 20 &&
            buffers.Snapshot.PresentedSequence == 7,
            "A reset backbuffer should accept a new presentation fence without stale state.");

        buffers.Dispose();

        assert(
            buffers.Snapshot.FirstState ==
            ViewportPresentationBufferState.Disposed &&
            buffers.Snapshot.SecondState ==
            ViewportPresentationBufferState.Disposed,
            "Disposed backbuffers should expose an explicit disposed state.");
    }
}
