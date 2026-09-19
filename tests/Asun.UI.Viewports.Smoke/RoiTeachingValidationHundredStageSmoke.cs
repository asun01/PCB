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
                $"teaching step identity round {i + 1} should remain unique.");

        for (var i = 0; i < 10; i++)
            Check(
                RoiTeachingValidationRuntime.HasCurrentStepWhenActive(
                    guide.Snapshot,
                    steps),
                $"initial teaching state round {i + 1} should expose a current step.");

        var press = new RoiEditorEvent(
            RoiEditorEventKind.PointerDown,
            RoiInteractionKind.Creating,
            RoiHandleKind.Body,
            default,
            false);

        for (var i = 0; i < 10; i++)
        {
            Check(
                guide.Observe(press),
                $"expected teaching action round {i + 1} should advance.");
            guide.Reset();
            guide.Start(restart: true);
        }

        guide.Observe(press);

        var release = new RoiEditorEvent(
            RoiEditorEventKind.PointerUp,
            RoiInteractionKind.Creating,
            RoiHandleKind.Body,
            default,
            true);

        for (var i = 0; i < 10; i++)
        {
            Check(
                guide.Observe(release) ||
                !guide.Snapshot.IsActive,
                $"completion observation round {i + 1} should reach a terminal state.");
            guide.Reset();
            guide.Start(restart: true);
            guide.Observe(press);
        }

        guide.Observe(release);
        var completed = guide.Snapshot;

        for (var i = 0; i < 10; i++)
        {
            guide.Start();
            Check(
                !guide.Snapshot.IsActive &&
                guide.Snapshot.CurrentStep is null,
                $"completed non-repeatable start round {i + 1} should remain terminal.");
        }

        guide.Start(restart: true);

        for (var i = 0; i < 10; i++)
            Check(
                guide.Snapshot.IsActive &&
                RoiTeachingValidationRuntime.HasCurrentStepWhenActive(
                    guide.Snapshot,
                    steps),
                $"explicit restart round {i + 1} should reopen from the first step.");

        Check(
            completed.IsCompleted &&
            !completed.IsActive,
            "completed guide should expose terminal completion.");

        Check(
            round == 100,
            $"ROI teaching validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}

