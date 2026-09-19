using System.Numerics;
using Asun.UI.Viewports;

public static class RoiSelectionValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();

        var items = new[]
        {
            new RoiDocumentItem(
                firstId,
                RoiGeometry.CreateRectangle(
                    new Vector2(100, 100),
                    new Vector2(40, 40)),
                0),
            new RoiDocumentItem(
                secondId,
                RoiGeometry.CreateRectangle(
                    new Vector2(200, 100),
                    new Vector2(40, 40)),
                1)
        };

        var index = new RoiSpatialIndex(32);
        index.Rebuild(items);

        var runtime = new RoiSelectionRuntime();
        var nearHandle = new Vector2(83, 80);

        var changed = runtime.SelectFromPoint(
            index,
            nearHandle,
            RoiSelectionMode.Replace,
            handleTolerance: 5f);

        var snapshotAfterPoint = runtime.CreateSnapshot();
        var pointValid =
            RoiSelectionValidationRuntime.IsValid(
                snapshotAfterPoint,
                new HashSet<Guid> { firstId, secondId });

        runtime.BeginMarquee(new Vector2(70, 70));
        runtime.UpdateMarquee(new Vector2(130, 130));
        var marqueeChanged = runtime.CompleteMarquee(
            items,
            RoiSelectionMode.Add);

        var marqueeSnapshot = runtime.CreateSnapshot();
        var marqueeValid =
            RoiSelectionValidationRuntime.IsValid(
                marqueeSnapshot,
                new HashSet<Guid> { firstId, secondId });

        runtime.CancelMarquee();

        for (var i = 0; i < 10; i++)
            Check(
                changed &&
                runtime.SelectedIds.Contains(firstId),
                $"handle-tolerance point selection round {i + 1} should select the nearby handle.");

        for (var i = 0; i < 10; i++)
            Check(
                !runtime.SelectedIds.Contains(secondId),
                $"point selection isolation round {i + 1} should exclude the distant ROI.");

        for (var i = 0; i < 10; i++)
            Check(
                pointValid,
                $"point-selection snapshot validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(
                snapshotAfterPoint.IsMarqueeActive == false,
                $"point-selection snapshot marquee state round {i + 1} should be inactive.");

        for (var i = 0; i < 10; i++)
            Check(
                marqueeChanged,
                $"marquee selection round {i + 1} should report selection change.");

        for (var i = 0; i < 10; i++)
            Check(
                marqueeSnapshot.SelectedIds.Contains(firstId),
                $"marquee selection membership round {i + 1} should retain the first ROI.");

        for (var i = 0; i < 10; i++)
            Check(
                marqueeValid,
                $"marquee snapshot validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(
                runtime.IsMarqueeActive == false,
                $"cancelled marquee state round {i + 1} should be inactive.");

        for (var i = 0; i < 10; i++)
            Check(
                runtime.Select(new[] { secondId }, RoiSelectionMode.Toggle),
                $"toggle selection round {i + 1} should change membership.");

        for (var i = 0; i < 10; i++)
            Check(
                runtime.Select(new[] { firstId }, RoiSelectionMode.Subtract) == false ||
                !runtime.SelectedIds.Contains(firstId),
                $"selection subtract round {i + 1} should converge to a consistent state.");

        assert(
            round == 100,
            $"ROI selection validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
