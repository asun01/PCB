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
                $"active accounting round {i + 1} should be valid.");

        var drained = await input.WaitAndDrainAsync();

        for (var i = 0; i < 10; i++)
            Check(
                drained.Count == 2 &&
                drained[0].Kind == ViewportInputEventKind.PointerDown &&
                drained[1].Position == new Vector2(3, 3),
                $"coalescing round {i + 1} should preserve the latest move.");

        input.Complete();
        var completed = input.Snapshot();

        for (var i = 0; i < 10; i++)
            Check(
                completed.IsCompleted &&
                ViewportInputSubmissionValidationRuntime.IsTerminal(completed),
                $"completion round {i + 1} should be terminal.");

        input.ResetLifecycle();
        var reset = input.Snapshot();

        for (var i = 0; i < 10; i++)
            Check(
                !reset.IsCompleted &&
                !reset.IsCancelled &&
                ViewportInputSubmissionValidationRuntime.IsValid(reset),
                $"reset round {i + 1} should reopen the runtime.");

        input.Cancel();
        var cancelled = input.Snapshot();

        for (var i = 0; i < 10; i++)
            Check(
                cancelled.IsCancelled &&
                cancelled.IsCompleted &&
                cancelled.Pending == 0 &&
                ViewportInputSubmissionValidationRuntime.IsTerminal(cancelled),
                $"cancel round {i + 1} should be terminal.");

        input.ResetLifecycle();

        for (var i = 0; i < 10; i++)
            Check(
                input.TrySubmit(
                    ViewportInputEventKind.PointerUp,
                    new Vector2(4, 4)) &&
                input.PendingCount == 1,
                $"post-reset submission round {i + 1} should be accepted.");

        input.Drain();

        for (var i = 0; i < 10; i++)
            Check(
                input.PendingCount == 0 &&
                ViewportInputSubmissionValidationRuntime.IsValid(input.Snapshot()),
                $"post-drain round {i + 1} should close the queue cleanly.");

        assert(
            round == 100,
            $"Input submission validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
