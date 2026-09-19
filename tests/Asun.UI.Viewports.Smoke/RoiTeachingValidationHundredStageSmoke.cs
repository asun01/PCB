using Asun.UI.Viewports;

public static class RoiTeachingValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var guide = new RoiTeachingGuide(new[]
        {
            new RoiTeachingStep(
                "press",
                "Press",
                RoiTeachingAction.PointerDown,
                RoiInteractionKind.Creating,
                repeatable: false),
            new RoiTeachingStep(
                "release",
                "Release",
                RoiTeachingAction.PointerUp,
                RoiInteractionKind.Creating,
                repeatable: false)
        });

        guide.Start(restart: true);

        var round = 0;
        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var steps = guide.Steps;

        for (var i = 0; i < 10; i++)
            Check(
                RoiTeachingValidationRuntime.HasUniqueIds(steps),
                $"teaching identity round {i + 1} should remain unique.");

        for (var i = 0; i < 10; i++)
            Check(
                RoiTeachingValidationRuntime.HasCurrentStepWhenActive(
                    guide.Snapshot,
                    steps),
                $"initial active state round {i + 1} should expose a step.");

        for (var i = 0; i < 10; i++)
        {
            guide.Reset();
            guide.Start(restart: true);
            Check(
                guide.Observe(new RoiEditorEvent(
                    RoiEditorEventKind.PointerDown,
                    RoiInteractionKind.Creating,
                    RoiHandleKind.Body,
                    default,
                    false)),
                $"pointer-down advance round {i + 1} should succeed.");
        }

        for (var i = 0; i < 10; i++)
        {
            Check(
                guide.Snapshot.CurrentStep?.Action ==
                    RoiTeachingAction.PointerUp,
                $"second teaching step round {i + 1} should be PointerUp.");
        }

        for (var i = 0; i < 10; i++)
        {
            guide.Reset();
            guide.Start(restart: true);
            guide.Observe(new RoiEditorEvent(
                RoiEditorEventKind.PointerDown,
                RoiInteractionKind.Creating,
                RoiHandleKind.Body,
                default,
                false));
            Check(
                guide.Observe(new RoiEditorEvent(
                    RoiEditorEventKind.PointerUp,
                    RoiInteractionKind.Creating,
                    RoiHandleKind.Body,
                    default,
                    true)),
                $"completion round {i + 1} should advance to terminal.");
        }

        guide.Reset();
        guide.Start(restart: true);
        guide.Observe(new RoiEditorEvent(
            RoiEditorEventKind.PointerDown,
            RoiInteractionKind.Creating,
            RoiHandleKind.Body,
            default,
            false));
        guide.Observe(new RoiEditorEvent(
            RoiEditorEventKind.PointerUp,
            RoiInteractionKind.Creating,
            RoiHandleKind.Body,
            default,
            true));

        var completed = guide.Snapshot;

        for (var i = 0; i < 10; i++)
        {
            guide.Start();
            Check(
                completed.IsCompleted &&
                !guide.Snapshot.IsActive &&
                guide.Snapshot.CurrentStep is null,
                $"completed non-repeatable start round {i + 1} should remain terminal.");
        }

        for (var i = 0; i < 10; i++)
        {
            guide.Start(restart: true);
            Check(
                guide.Snapshot.IsActive &&
                RoiTeachingValidationRuntime.HasCurrentStepWhenActive(
                    guide.Snapshot,
                    steps),
                $"explicit restart round {i + 1} should reopen at the first step.");
            guide.Stop();
        }

        for (var i = 0; i < 10; i++)
            Check(
                RoiTeachingValidationRuntime.HasCurrentStepWhenActive(
                    guide.Snapshot,
                    steps),
                $"stopped teaching round {i + 1} should not expose an invalid active index.");

        for (var i = 0; i < 10; i++)
            Check(
                completed.CurrentStepIndex == steps.Count,
                $"terminal index round {i + 1} should equal the step count.");

        for (var i = 0; i < 10; i++)
            Check(
                completed.CurrentStep is null &&
                completed.IsCompleted,
                $"terminal snapshot round {i + 1} should expose completed state.");

        assert(
            round == 100,
            $"ROI teaching validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
