using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportInputEdgeSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        using var input = new ViewportInputSubmissionRuntime();

        var sequence1 = input.Submit(
            ViewportInputEventKind.PointerDown,
            new Vector2(1, 1));

        var sequence2 = input.Submit(
            ViewportInputEventKind.PointerUp,
            new Vector2(2, 2));

        assert(
            sequence2 > sequence1 &&
            input.SubmittedCount == 2 &&
            input.PendingCount == 2,
            "Input submission should allocate strictly increasing event sequences.");

        var moveInput = new ViewportInputSubmissionRuntime();
        var move1 = moveInput.Submit(
            ViewportInputEventKind.PointerMove,
            new Vector2(10, 10));

        var move2 = moveInput.Submit(
            ViewportInputEventKind.PointerMove,
            new Vector2(20, 20));

        var moveEvents = moveInput.Drain();

        assert(
            move2 > move1 &&
            moveInput.CoalescedCount == 1 &&
            moveEvents.Count == 1 &&
            moveEvents[0].Position == new Vector2(20, 20),
            "Consecutive pointer moves should coalesce to the latest event.");

        var replaceInput = new ViewportInputSubmissionRuntime();
        replaceInput.Submit(
            ViewportInputEventKind.PointerMove,
            new Vector2(5, 5));

        var replaced = replaceInput.TryReplaceLatestMove(
            new Vector2(15, 25));

        var replacedEvents = replaceInput.Drain();

        assert(
            replaced &&
            replacedEvents.Count == 1 &&
            replacedEvents[0].Position == new Vector2(15, 25) &&
            replaceInput.CoalescedCount == 1,
            "Explicit ReplaceLatestMove should mutate only the latest pending pointer event.");

        using var drainInput = new ViewportInputSubmissionRuntime();
        for (var index = 0; index < 5; index++)
        {
            drainInput.Submit(
                ViewportInputEventKind.Wheel,
                new Vector2(index, index),
                wheelDelta: 120);
        }

        var firstDrain = drainInput.Drain(2);
        var secondDrain = drainInput.Drain();

        assert(
            firstDrain.Count == 2 &&
            secondDrain.Count == 3 &&
            firstDrain[0].Sequence < secondDrain[0].Sequence,
            "Bounded Drain should preserve FIFO order across multiple batches.");

        using var waitInput = new ViewportInputSubmissionRuntime();
        waitInput.Submit(
            ViewportInputEventKind.PointerDown,
            new Vector2(3, 4));

        var waited = await waitInput.WaitAndDrainAsync();

        assert(
            waited.Count == 1 &&
            waited[0].Position == new Vector2(3, 4),
            "WaitAndDrainAsync should return already-queued input without an extra wakeup.");

        using var completeInput = new ViewportInputSubmissionRuntime();
        completeInput.Submit(
            ViewportInputEventKind.PointerDown,
            new Vector2(1, 2));
        completeInput.Complete(cancelPending: false);

        var retainedAfterComplete = await completeInput.WaitAndDrainAsync();

        assert(
            completeInput.IsCompleted &&
            retainedAfterComplete.Count == 1,
            "Completion without cancelPending should retain queued input for final draining.");

        using var cancelledCompletion = new ViewportInputSubmissionRuntime();
        cancelledCompletion.Submit(
            ViewportInputEventKind.PointerDown,
            new Vector2(1, 2));
        cancelledCompletion.Complete(cancelPending: true);

        assert(
            cancelledCompletion.IsCompleted &&
            cancelledCompletion.PendingCount == 0 &&
            (await cancelledCompletion.WaitAndDrainAsync()).Count == 0,
            "Completion with cancelPending should discard queued input deterministically.");

        cancelledCompletion.ResetLifecycle();
        assert(
            !cancelledCompletion.IsCompleted &&
            !cancelledCompletion.IsCancelled &&
            cancelledCompletion.TrySubmit(
                ViewportInputEventKind.PointerUp,
                new Vector2(4, 5)),
            "ResetLifecycle should reopen a completed input submission runtime.");

        using var cancelInput = new ViewportInputSubmissionRuntime();
        cancelInput.Submit(
            ViewportInputEventKind.PointerDown,
            new Vector2(1, 1));
        cancelInput.Cancel();

        assert(
            cancelInput.IsCompleted &&
            cancelInput.IsCancelled &&
            cancelInput.PendingCount == 0 &&
            !cancelInput.TrySubmit(
                ViewportInputEventKind.PointerMove,
                new Vector2(9, 9)),
            "Cancellation should clear pending input and reject subsequent submissions.");

        var oldestInput = new ViewportInputSubmissionRuntime();
        var dropOldest = new ViewportInputBackpressureRuntime(
            2,
            ViewportInputDropPolicy.DropOldest);

        dropOldest.TrySubmit(
            oldestInput,
            ViewportInputEventKind.Wheel,
            new Vector2(1, 1));
        dropOldest.TrySubmit(
            oldestInput,
            ViewportInputEventKind.Wheel,
            new Vector2(2, 2));
        dropOldest.TrySubmit(
            oldestInput,
            ViewportInputEventKind.Wheel,
            new Vector2(3, 3));

        var oldestEvents = oldestInput.Drain();
        var oldestSnapshot = dropOldest.Capture(oldestInput);

        assert(
            oldestEvents.Count == 2 &&
            oldestEvents[0].Position == new Vector2(2, 2) &&
            oldestEvents[1].Position == new Vector2(3, 3) &&
            oldestSnapshot.Dropped == 1,
            "DropOldest backpressure should evict the earliest event at saturation.");

        var newestInput = new ViewportInputSubmissionRuntime();
        var dropNewest = new ViewportInputBackpressureRuntime(
            2,
            ViewportInputDropPolicy.DropNewest);

        dropNewest.TrySubmit(
            newestInput,
            ViewportInputEventKind.Wheel,
            new Vector2(1, 1));
        dropNewest.TrySubmit(
            newestInput,
            ViewportInputEventKind.Wheel,
            new Vector2(2, 2));
        var acceptedNewest = dropNewest.TrySubmit(
            newestInput,
            ViewportInputEventKind.Wheel,
            new Vector2(3, 3));

        var newestEvents = newestInput.Drain();
        var newestSnapshot = dropNewest.Capture(newestInput);

        assert(
            !acceptedNewest &&
            newestEvents.Count == 2 &&
            newestEvents[^1].Position == new Vector2(2, 2) &&
            newestSnapshot.Dropped == 1,
            "DropNewest backpressure should preserve existing queued events when saturated.");

        oldestInput.Dispose();
        newestInput.Dispose();
    }
}
