using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportInputSubmissionValidationHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool, string> assert)
    {
        using var input = new ViewportInputSubmissionRuntime();

        var round = 0;
        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        input.Submit(
            ViewportInputEventKind.PointerDown,
            new Vector2(1, 1));

        input.Submit(
            ViewportInputEventKind.PointerMove,
            new Vector2(2, 2));

        input.Submit(
            ViewportInputEventKind.PointerMove,
            new Vector2(3, 3));

        var active = input.Snapshot();

        for (var i = 0; i < 10; i++)
            Check(
                ViewportInputSubmissionValidationRuntime.IsValid(active),
                $"active submission round {i + 1} should satisfy accounting.");

        var drained = await input.WaitAndDrainAsync();

        for (var i = 0; i < 10; i++)
            Check(
                drained.Count == 2 &&
                drained[0].Kind == ViewportInputEventKind.PointerDown &&
                drained[1].Position == new Vector2(3, 3),
                $"move coalescing round {i + 1} should preserve FIFO and latest move.");

        input.Complete();

        var completed = input.Snapshot();

        for (var i = 0; i < 10; i++)
            Check(
                completed.IsCompleted &&
                ViewportInputSubmissionValidationRuntime.IsValid(completed),
                $"completed state round {i + 1} should remain valid.");

        input.ResetLifecycle();

        for (var i = 0; i < 10; i++)
            Check(
                !input.IsCompleted &&
                !input.IsCancelled &&
                ViewportInputSubmissionValidationRuntime.IsValid(input.Snapshot()),
                $"reset lifecycle round {i + 1} should reopen the submission runtime.");

        input.Cancel();

        for (var i = 0; i < 10; i++)
            Check(
                input.IsCancelled &&
                input.IsCompleted &&
                input.PendingCount == 0 &&
                ViewportInputSubmissionValidationRuntime.IsValid(input.Snapshot()),
                $"cancel lifecycle round {i + 1} should become terminal.");

        input.ResetLifecycle();

        Check(
            input.TrySubmit(
                ViewportInputEventKind.PointerUp,
                new Vector2(4, 4)),
            "post-reset submission should be accepted.");

        Check(
            round == 100,
            $"Input submission validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
