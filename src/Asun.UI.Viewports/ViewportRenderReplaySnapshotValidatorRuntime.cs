namespace Asun.UI.Viewports;

public readonly record struct ViewportRenderReplayValidationResult(
    bool IsValid,
    IReadOnlyList<string> Errors)
{
    public static ViewportRenderReplayValidationResult Valid { get; } =
        new(true, Array.Empty<string>());
}

public static class ViewportRenderReplaySnapshotValidatorRuntime
{
    public static ViewportRenderReplayValidationResult Validate(
        ViewportRenderReplaySnapshot snapshot)
    {
        var errors = new List<string>();
        var operations = snapshot.Operations;

        var expectedSequence = 1;
        long? generation = null;
        var frameOpen = false;
        var beginCount = 0;
        var endCount = 0;

        foreach (var operation in operations)
        {
            if (operation.Sequence != expectedSequence)
            {
                errors.Add(
                    $"Replay operation sequence expected {expectedSequence} but was {operation.Sequence}.");
            }

            expectedSequence++;

            if (generation is null)
                generation = operation.Generation;
            else if (operation.Generation != generation.Value)
                errors.Add("Replay operations must remain on one generation.");

            switch (operation.Kind)
            {
                case ViewportRenderReplayOperationKind.Begin:
                    if (frameOpen)
                        errors.Add("Replay frame cannot begin twice without an end.");

                    frameOpen = true;
                    beginCount++;
                    break;

                case ViewportRenderReplayOperationKind.DrawTile:
                case ViewportRenderReplayOperationKind.DrawRoi:
                case ViewportRenderReplayOperationKind.DrawOverlay:
                case ViewportRenderReplayOperationKind.ClearInvalidatedRegion:
                    if (!frameOpen)
                        errors.Add("Replay draw operation occurred outside a frame.");
                    break;

                case ViewportRenderReplayOperationKind.End:
                    if (!frameOpen)
                        errors.Add("Replay frame ended without a matching begin.");

                    frameOpen = false;
                    endCount++;
                    break;

                case ViewportRenderReplayOperationKind.Commit:
                    if (frameOpen)
                        errors.Add("Replay commit occurred before the frame ended.");

                    if (operation.DeliveryStatus !=
                        ViewportRenderDeliveryStatus.Succeeded)
                    {
                        errors.Add("Replay commit must use Succeeded status.");
                    }

                    break;

                case ViewportRenderReplayOperationKind.Discard:
                    if (frameOpen)
                        errors.Add("Replay discard occurred before the frame ended.");

                    if (operation.DeliveryStatus is not
                        (ViewportRenderDeliveryStatus.Cancelled or
                         ViewportRenderDeliveryStatus.Deferred or
                         ViewportRenderDeliveryStatus.Failed))
                    {
                        errors.Add("Replay discard must carry a non-success delivery status.");
                    }

                    break;

                default:
                    errors.Add("Replay operation contains an unknown kind.");
                    break;
            }
        }

        if (frameOpen)
            errors.Add("Replay sequence ended with an open frame.");

        if (beginCount != endCount)
            errors.Add("Replay begin/end counts must balance.");

        if (snapshot.OperationCount != operations.Count)
            errors.Add("Replay snapshot operation count does not match operations.");

        if (snapshot.BeginCount != beginCount ||
            snapshot.EndCount != endCount)
        {
            errors.Add("Replay snapshot lifecycle counters do not match operations.");
        }

        if (snapshot.TileCount != operations.Count(
                operation => operation.Kind ==
                    ViewportRenderReplayOperationKind.DrawTile))
        {
            errors.Add("Replay tile count does not match operations.");
        }

        if (snapshot.RoiCount != operations.Count(
                operation => operation.Kind ==
                    ViewportRenderReplayOperationKind.DrawRoi))
        {
            errors.Add("Replay ROI count does not match operations.");
        }

        if (snapshot.CommitCount != operations.Count(
                operation => operation.Kind ==
                    ViewportRenderReplayOperationKind.Commit))
        {
            errors.Add("Replay commit count does not match operations.");
        }

        if (snapshot.DiscardCount != operations.Count(
                operation => operation.Kind ==
                    ViewportRenderReplayOperationKind.Discard))
        {
            errors.Add("Replay discard count does not match operations.");
        }

        var renderedUnits = operations.Sum(
            operation => operation.RenderedUnits);

        if (snapshot.RenderedUnits != renderedUnits)
            errors.Add("Replay rendered unit total does not match operations.");

        return errors.Count == 0
            ? ViewportRenderReplayValidationResult.Valid
            : new ViewportRenderReplayValidationResult(false, errors);
    }
}
