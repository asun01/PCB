using System.Drawing;

namespace Asun.UI.Viewports;

/// <summary>
/// Centralized structural invariant checks for the framework-neutral viewport
/// runtime. These checks validate consistency only; they do not define
/// measurement/domain authority or external vendor behavior.
/// </summary>
public static class ViewportInvariantRuntime
{
    public static IReadOnlyList<string> ValidateWorkPlan(
        ViewportRenderWorkPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);

        var errors = new List<string>();

        foreach (var item in plan.Items)
        {
            if (item.Generation != plan.Generation)
                errors.Add("Render work generation does not match the containing plan.");

            if (!float.IsFinite(item.Bounds.X) ||
                !float.IsFinite(item.Bounds.Y) ||
                !float.IsFinite(item.Bounds.Width) ||
                !float.IsFinite(item.Bounds.Height))
            {
                errors.Add("Render work contains non-finite bounds.");
            }
        }

        return errors;
    }

    public static IReadOnlyList<string> ValidateBatch(
        ViewportRenderBatch batch)
    {
        ArgumentNullException.ThrowIfNull(batch);

        var errors = new List<string>();

        foreach (var item in batch.Items)
        {
            if (item.Generation != batch.Generation)
                errors.Add("Render batch item generation does not match the batch.");
        }

        foreach (var region in batch.Regions)
        {
            if (!IsFinite(region) || region.IsEmpty)
                errors.Add("Render batch contains an invalid region.");
        }

        if (batch.InvalidationCount < 0 ||
            batch.TileCount < 0 ||
            batch.RoiCount < 0 ||
            batch.OverlayCount < 0 ||
            batch.FullSurfaceCount < 0)
        {
            errors.Add("Render batch counters cannot be negative.");
        }

        return errors;
    }

    public static IReadOnlyList<string> ValidateCommandStream(
        ViewportRenderCommandStream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var errors = new List<string>();

        for (var index = 0; index < stream.Commands.Count; index++)
        {
            var command = stream.Commands[index];
            var expectedSequence = index + 1;

            if (command.Sequence != expectedSequence)
                errors.Add("Render command sequence is not contiguous.");

            if (command.WorkItem.Generation != stream.Generation)
                errors.Add("Render command work generation does not match the stream.");

            if (command.WorkItem.Bounds != command.Bounds)
                errors.Add("Render command bounds do not match its work item.");

            if (!IsFinite(command.Bounds))
                errors.Add("Render command contains non-finite bounds.");
        }

        if (stream.CommandCount != stream.Commands.Count)
            errors.Add("Render command count is inconsistent.");

        return errors;
    }

    public static IReadOnlyList<string> ValidateFrameState(
        ViewportRenderFrameState state)
    {
        var errors = new List<string>();

        if (state.PlannedUnits < 0 ||
            state.RenderedUnits < 0 ||
            state.DeferredUnits < 0 ||
            state.RegionCount < 0)
        {
            errors.Add("Frame-state counters cannot be negative.");
        }

        if (state.RenderedUnits + state.DeferredUnits > state.PlannedUnits)
            errors.Add("Rendered plus deferred units cannot exceed planned units.");

        if (!state.HasDeferredWork &&
            state.DeferredUnits != 0 &&
            state.DeferredWorkItems.Count == 0)
        {
            errors.Add("Frame-state deferred flag is inconsistent.");
        }

        if (state.IsComplete &&
            (state.DeferredUnits != 0 ||
             state.DeferredWorkItems.Count != 0 ||
             state.Error is not null))
        {
            errors.Add("A complete frame cannot carry deferred work or an error.");
        }

        return errors;
    }

    public static IReadOnlyList<string> ValidateDeliveryStatistics(
        ViewportRenderDeliveryStatistics statistics)
    {
        var errors = new List<string>();

        if (statistics.Attempts < 0 ||
            statistics.Succeeded < 0 ||
            statistics.Partial < 0 ||
            statistics.Failed < 0 ||
            statistics.Deferred < 0 ||
            statistics.Cancelled < 0 ||
            statistics.RenderedUnits < 0)
        {
            errors.Add("Delivery statistics cannot be negative.");
        }

        var classified =
            statistics.Succeeded +
            statistics.Failed +
            statistics.Deferred +
            statistics.Cancelled;

        if (classified > statistics.Attempts)
            errors.Add("Delivery classifications exceed total attempts.");

        if (statistics.Partial > statistics.Deferred)
            errors.Add("Partial delivery count cannot exceed deferred attempts.");

        return errors;
    }

    public static IReadOnlyList<string> ValidateQueueStatistics(
        ViewportPresentationQueueStatistics statistics)
    {
        var errors = new List<string>();

        if (statistics.Pending < 0 ||
            statistics.LatestSubmissionSequence < 0)
        {
            errors.Add("Queue counters cannot be negative.");
        }

        if (statistics.CommitInProgress &&
            (statistics.InFlightSequence is null ||
             statistics.CommittingSequence is null))
        {
            errors.Add("Commit window requires an in-flight and committing sequence.");
        }

        if (statistics.CommitInProgress &&
            statistics.InFlightSequence != statistics.CommittingSequence)
        {
            errors.Add("Commit window must refer to the active in-flight sequence.");
        }

        if (!statistics.CommitInProgress &&
            statistics.CommittingSequence is not null)
        {
            errors.Add("A closed commit window cannot expose a committing sequence.");
        }

        if (statistics.PresentedSequence is long presented &&
            presented > statistics.LatestSubmissionSequence)
        {
            errors.Add("Presented sequence cannot exceed the latest submission sequence.");
        }

        return errors;
    }

    public static IReadOnlyList<string> ValidateBufferSnapshot(
        ViewportPresentationBufferSnapshot snapshot)
    {
        var errors = new List<string>();

        if (snapshot.RenderingSlot is int rendering &&
            rendering is < 0 or > 1)
        {
            errors.Add("Rendering slot index is outside the double-buffer domain.");
        }

        if (snapshot.PresentedSlot is int presented &&
            presented is < 0 or > 1)
        {
            errors.Add("Presented slot index is outside the double-buffer domain.");
        }

        if (snapshot.RenderingSlot is not null &&
            snapshot.FirstState != ViewportPresentationBufferState.Rendering &&
            snapshot.SecondState != ViewportPresentationBufferState.Rendering)
        {
            errors.Add("Rendering slot and slot states disagree.");
        }

        if (snapshot.PresentedSlot is not null &&
            snapshot.FirstState != ViewportPresentationBufferState.Presented &&
            snapshot.SecondState != ViewportPresentationBufferState.Presented)
        {
            errors.Add("Presented slot and slot states disagree.");
        }

        if (snapshot.LastPlannedUnits < 0 ||
            snapshot.LastRenderedUnits < 0)
        {
            errors.Add("Backbuffer render counters cannot be negative.");
        }

        return errors;
    }

    public static IReadOnlyList<string> ValidateSurfaceSnapshot(
        ViewportRenderSurfaceSnapshot snapshot)
    {
        var errors = new List<string>();

        if (snapshot.PresentationSequence < 0 ||
            snapshot.LastRenderedUnits < 0 ||
            snapshot.LastPlannedUnits < 0)
        {
            errors.Add("Surface counters cannot be negative.");
        }

        if (snapshot.State == ViewportRenderSurfaceState.Rendering &&
            snapshot.RenderingGeneration is null)
        {
            errors.Add("Rendering surface state requires a rendering generation.");
        }

        if (snapshot.State == ViewportRenderSurfaceState.Presented &&
            snapshot.PresentedGeneration is null)
        {
            errors.Add("Presented surface state requires a presented generation.");
        }

        if (snapshot.PresentedRegionCount != snapshot.PresentedRegions.Count)
            errors.Add("Surface region count is inconsistent.");

        return errors;
    }

    public static IReadOnlyList<string> ValidateReplaySnapshot(
        ViewportRenderReplaySnapshot snapshot)
    {
        var errors = new List<string>();

        if (snapshot.OperationCount != snapshot.Operations.Count)
            errors.Add("Replay operation count is inconsistent.");

        foreach (var operation in snapshot.Operations)
        {
            if (operation.Sequence <= 0 ||
                operation.Generation < 0 ||
                operation.RenderedUnits < 0)
            {
                errors.Add("Replay operation contains invalid counters.");
            }
        }

        if (snapshot.TileCount < 0 ||
            snapshot.RoiCount < 0 ||
            snapshot.OverlayCount < 0 ||
            snapshot.InvalidationCount < 0 ||
            snapshot.CommitCount < 0 ||
            snapshot.DiscardCount < 0)
        {
            errors.Add("Replay operation counters cannot be negative.");
        }

        return errors;
    }

    public static IReadOnlyList<string> ValidateInputSnapshot(
        ViewportInputSubmissionSnapshot snapshot)
    {
        var errors = new List<string>();

        if (snapshot.Pending < 0 ||
            snapshot.Submitted < 0 ||
            snapshot.Coalesced < 0)
        {
            errors.Add("Input submission counters cannot be negative.");
        }

        if (snapshot.Pending > snapshot.Submitted)
            errors.Add("Pending input cannot exceed submitted input.");

        return errors;
    }

    public static IReadOnlyList<string> ValidateBackpressureSnapshot(
        ViewportInputBackpressureSnapshot snapshot)
    {
        var errors = new List<string>();

        if (snapshot.Capacity <= 0 ||
            snapshot.Pending < 0 ||
            snapshot.Dropped < 0 ||
            snapshot.Coalesced < 0)
        {
            errors.Add("Backpressure counters contain invalid values.");
        }

        if (snapshot.Pending > snapshot.Capacity)
            errors.Add("Backpressure pending count exceeds capacity.");

        return errors;
    }

    public static IReadOnlyList<string> ValidateVisibleRegion(
        ViewportVisibleRegion region)
    {
        var errors = new List<string>();

        if (!IsFinite(region.ImageRectangle) ||
            region.ImageRectangle.IsEmpty)
        {
            errors.Add("Visible image rectangle is invalid.");
        }

        if (region.Tiles.Count != region.Tiles.Distinct().Count())
            errors.Add("Visible tile list contains duplicate indices.");

        return errors;
    }

    public static IReadOnlyList<string> ValidateTileFrame<TTile>(
        ViewportTileFrame<TTile> frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var errors = new List<string>();

        if (frame.Requests.Count != frame.RequestedCount)
            errors.Add("Tile frame requested count is inconsistent.");

        foreach (var pair in frame.LoadedTiles)
        {
            if (!frame.Requests.Any(request => request.Index.Equals(pair.Key)))
                errors.Add("Loaded tile does not belong to the request set.");
        }

        if (frame.Requests.Select(request => request.Index).Distinct().Count() !=
            frame.Requests.Count)
        {
            errors.Add("Tile frame requests contain duplicate indices.");
        }

        return errors;
    }

    public static IReadOnlyList<string> ValidateSceneSnapshot(
        ViewportSceneSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var errors = new List<string>();

        foreach (var command in snapshot.Commands)
        {
            if (command.RoiId == Guid.Empty)
                errors.Add("Scene command must identify its ROI.");

            if (!float.IsFinite(command.Start.X) ||
                !float.IsFinite(command.Start.Y) ||
                !float.IsFinite(command.End.X) ||
                !float.IsFinite(command.End.Y))
            {
                errors.Add("Scene command contains non-finite geometry.");
            }
        }

        return errors;
    }

    public static IReadOnlyList<string> ValidateSceneDiff(
        IReadOnlyList<ViewportSceneDiff> diffs)
    {
        ArgumentNullException.ThrowIfNull(diffs);

        var errors = new List<string>();

        foreach (var diff in diffs)
        {
            if (diff.Kind == ViewportSceneDiffKind.Added &&
                (diff.Current is null || diff.Previous is not null))
            {
                errors.Add("Added scene diff must contain only the current command.");
            }

            if (diff.Kind == ViewportSceneDiffKind.Removed &&
                (diff.Previous is null || diff.Current is not null))
            {
                errors.Add("Removed scene diff must contain only the previous command.");
            }

            if (diff.Kind == ViewportSceneDiffKind.TransformChanged &&
                diff.RoiId != Guid.Empty)
            {
                errors.Add("Transform diff must not be associated with an ROI.");
            }
        }

        return errors;
    }

    public static IReadOnlyList<string> ValidateWorkflow(
        ViewportWorkflowValidation validation)
    {
        var errors = new List<string>();

        if (validation.CommandCount < 0 ||
            validation.ErrorCount < 0)
        {
            errors.Add("Workflow validation counters cannot be negative.");
        }

        if (validation.ErrorCount != validation.Errors.Count)
            errors.Add("Workflow validation error count is inconsistent.");

        if (validation.IsValid && validation.ErrorCount != 0)
            errors.Add("A valid workflow cannot contain validation errors.");

        return errors;
    }

    public static IReadOnlyList<string> ValidateWorkflowJournal(
        IReadOnlyList<ViewportWorkflowRecord> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        var errors = new List<string>();
        long previousSequence = 0;

        foreach (var record in records)
        {
            if (record.Sequence <= previousSequence)
                errors.Add("Workflow journal sequence is not strictly increasing.");

            previousSequence = record.Sequence;

            if (record.RoiCount < 0)
                errors.Add("Workflow journal ROI count cannot be negative.");

            if (!double.IsFinite(record.Scale) || record.Scale <= 0)
                errors.Add("Workflow journal scale is invalid.");
        }

        return errors;
    }

    public static IReadOnlyList<string> ValidateZoomProfile(
        ViewportZoomProfile profile)
    {
        try
        {
            ViewportZoomProfileRuntime.Validate(profile);
            return Array.Empty<string>();
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return new[] { exception.Message };
        }
    }

    public static IReadOnlyList<string> ValidatePlanMetrics(
        ViewportRenderPlanMetrics metrics)
    {
        var errors = new List<string>();

        if (metrics.Total < 0 ||
            metrics.Tile < 0 ||
            metrics.SelectedRoi < 0 ||
            metrics.Roi < 0 ||
            metrics.Overlay < 0 ||
            metrics.FullSurface < 0 ||
            metrics.Regions < 0)
        {
            errors.Add("Render plan metrics cannot be negative.");
        }

        if (metrics.SelectedRoi > metrics.Roi ||
            metrics.Tile > metrics.Total ||
            metrics.Roi > metrics.Total ||
            metrics.Overlay > metrics.Total ||
            metrics.FullSurface > metrics.Total)
        {
            errors.Add("Render plan metrics exceed their containing totals.");
        }

        return errors;
    }

    public static IReadOnlyList<string> ValidateFrameMetrics(
        ViewportRenderFrameMetrics metrics)
    {
        var errors = new List<string>();

        if (metrics.RequestedTiles < 0 ||
            metrics.LoadedTiles < 0 ||
            metrics.VisibleTiles < 0 ||
            metrics.RoiCommands < 0 ||
            metrics.SceneCommands < 0 ||
            metrics.SceneDiffs < 0 ||
            metrics.Generation < 0)
        {
            errors.Add("Render frame metrics cannot be negative.");
        }

        if (metrics.LoadedTiles > metrics.RequestedTiles)
            errors.Add("Loaded tiles cannot exceed requested tiles.");

        if (metrics.VisibleTiles > metrics.RequestedTiles)
            errors.Add("Visible tiles cannot exceed requested tiles.");

        return errors;
    }

    private static bool IsFinite(RectangleF value) =>
        float.IsFinite(value.X) &&
        float.IsFinite(value.Y) &&
        float.IsFinite(value.Width) &&
        float.IsFinite(value.Height);
}
