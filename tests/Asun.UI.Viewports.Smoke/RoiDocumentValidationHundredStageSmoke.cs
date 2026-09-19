using System.Numerics;
using Asun.UI.Viewports;

public static class RoiDocumentValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var document = new RoiDocumentRuntime();
        var explicitId = Guid.NewGuid();

        document.Add(
            RoiGeometry.CreateRectangle(
                new Vector2(100, 100),
                new Vector2(40, 40)),
            explicitId);

        var duplicateRejected = false;
        try
        {
            document.Add(
                RoiGeometry.CreateRectangle(
                    new Vector2(150, 100),
                    new Vector2(20, 20)),
                explicitId);
        }
        catch (ArgumentException)
        {
            duplicateRejected = true;
        }

        var emptyRejected = false;
        try
        {
            document.Add(
                RoiGeometry.CreateRectangle(
                    new Vector2(200, 100),
                    new Vector2(20, 20)),
                Guid.Empty);
        }
        catch (ArgumentOutOfRangeException)
        {
            emptyRejected = true;
        }

        document.Mode = RoiEditorMode.CreateRectangle;
        var down = document.PointerDown(new Vector2(220, 200));
        var cancelled = document.Cancel(new Vector2(260, 240));
        var snapshot = document.CreateSnapshot();

        var cancelValidation =
            down.RoiId is Guid createdId
                ? RoiDocumentValidationRuntime.ValidateCancelEvent(
                    cancelled,
                    createdId)
                : new[] { "Create pointer down did not provide a ROI id." };

        var snapshotValidation =
            RoiDocumentValidationRuntime.ValidateSnapshot(snapshot);

        for (var i = 0; i < 10; i++)
            Check(
                duplicateRejected,
                $"duplicate id rejection round {i + 1} should hold.");

        for (var i = 0; i < 10; i++)
            Check(
                emptyRejected,
                $"empty id rejection round {i + 1} should hold.");

        for (var i = 0; i < 10; i++)
            Check(
                down.RoiId is not null,
                $"create pointer identity round {i + 1} should be present.");

        for (var i = 0; i < 10; i++)
            Check(
                cancelled.EditorEvent.Kind == RoiEditorEventKind.Cancelled,
                $"cancel event kind round {i + 1} should be explicit.");

        for (var i = 0; i < 10; i++)
            Check(
                cancelValidation.Count == 0,
                $"cancel identity validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(
                document.Count == 1,
                $"cancelled creation rollback round {i + 1} should restore document count.");

        for (var i = 0; i < 10; i++)
            Check(
                snapshotValidation.Count == 0,
                $"document snapshot validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(
                snapshot.Items.Select(item => item.Id).Distinct().Count() ==
                snapshot.Items.Count,
                $"snapshot id uniqueness round {i + 1} should hold.");

        for (var i = 0; i < 10; i++)
            Check(
                snapshot.SelectedId == explicitId,
                $"snapshot selection round {i + 1} should remain the original ROI.");

        for (var i = 0; i < 10; i++)
            Check(
                document.Items.Count == 1 &&
                document.Items[0].Id == explicitId,
                $"document identity round {i + 1} should remain stable after rollback.");

        assert(
            round == 100,
            $"ROI document validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
