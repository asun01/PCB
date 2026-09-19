using System.Numerics;
using Asun.UI.Viewports;

public static class ViewportSceneDiffValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var runtime = new RoiViewportRuntime(
            new Vector2(1000, 1000),
            new Vector2(500, 400));

        var polygonId = runtime.Document.Add(
            RoiGeometry.CreatePolygon(new[]
            {
                new Vector2(100, 100),
                new Vector2(180, 100),
                new Vector2(200, 160),
                new Vector2(120, 190)
            }));

        runtime.Document.Select(polygonId);

        var first = ViewportSceneRuntime.Build(runtime.CreateSnapshot());

        runtime.Document.TranslateSelected(new Vector2(25, 15));

        var second = ViewportSceneRuntime.Build(runtime.CreateSnapshot());
        var moveDiff = ViewportSceneDiffRuntime.Diff(first, second);

        var bodyCommands = first.Commands
            .Where(command =>
                command.RoiId == polygonId &&
                command.Kind == ViewportSceneCommandKind.RoiBody)
            .OrderBy(command => command.HandleIndex)
            .ToArray();

        var movedBodyCommands = second.Commands
            .Where(command =>
                command.RoiId == polygonId &&
                command.Kind == ViewportSceneCommandKind.RoiBody)
            .OrderBy(command => command.HandleIndex)
            .ToArray();

        var selectionDiff = Array.Empty<ViewportSceneDiff>();
        runtime.Document.Select(null);
        var third = ViewportSceneRuntime.Build(runtime.CreateSnapshot());
        selectionDiff = ViewportSceneDiffRuntime.Diff(second, third);

        var identical = ViewportSceneDiffRuntime.Diff(third, third);
        var validation =
            ViewportSceneDiffValidationRuntime.Validate(first, second, moveDiff);
        var selectionValidation =
            ViewportSceneDiffValidationRuntime.Validate(second, third, selectionDiff);

        for (var i = 0; i < 10; i++)
            Check(
                bodyCommands.Length == 4,
                $"polygon body command count round {i + 1} should be four.");

        for (var i = 0; i < 10; i++)
            Check(
                bodyCommands.Select(command => command.HandleIndex)
                    .SequenceEqual(new[] { 0, 1, 2, 3 }),
                $"body segment identity round {i + 1} should be stable.");

        for (var i = 0; i < 10; i++)
            Check(
                movedBodyCommands.Select(command => command.HandleIndex)
                    .SequenceEqual(new[] { 0, 1, 2, 3 }),
                $"moved body segment identity round {i + 1} should remain stable.");

        for (var i = 0; i < 10; i++)
            Check(
                moveDiff.Count(diff =>
                    diff.Kind == ViewportSceneDiffKind.Changed) == 4,
                $"polygon move diff coverage round {i + 1} should include every edge.");

        for (var i = 0; i < 10; i++)
            Check(
                moveDiff.Select(diff => diff.Current?.HandleIndex)
                    .Where(index => index is not null)
                    .Select(index => index!.Value)
                    .OrderBy(index => index)
                    .SequenceEqual(new[] { 0, 1, 2, 3 }),
                $"move diff segment indexes round {i + 1} should be complete.");

        for (var i = 0; i < 10; i++)
            Check(
                moveDiff.All(diff =>
                    diff.Kind != ViewportSceneDiffKind.TransformChanged),
                $"ROI-only move transform isolation round {i + 1} should hold.");

        for (var i = 0; i < 10; i++)
            Check(
                selectionDiff.Any(diff =>
                    diff.Kind == ViewportSceneDiffKind.SelectionChanged &&
                    diff.RoiId == polygonId),
                $"selection diff round {i + 1} should identify the polygon.");

        for (var i = 0; i < 10; i++)
            Check(
                identical.Count == 0,
                $"identical scene diff round {i + 1} should be empty.");

        for (var i = 0; i < 10; i++)
            Check(
                validation.Count == 0 && selectionValidation.Count == 0,
                $"scene diff validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(
                first.Commands.All(command =>
                    ViewportSceneRuntime.GetCommandBounds(command).Width >= 0 &&
                    ViewportSceneRuntime.GetCommandBounds(command).Height >= 0),
                $"scene command bounds round {i + 1} should remain normalized.");

        assert(
            round == 100,
            $"Scene diff validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
