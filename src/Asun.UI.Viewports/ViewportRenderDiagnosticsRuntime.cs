namespace Asun.UI.Viewports;

public readonly record struct ViewportPresentationAuditEvent(
    long Sequence,
    string Stage,
    long Generation,
    long SubmissionSequence,
    ViewportRenderDeliveryStatus? DeliveryStatus,
    int RenderedUnits,
    int DeferredUnits,
    string EvidenceKey);

public sealed class ViewportPresentationAuditTrace
{
    private readonly object _sync = new();
    private readonly int _capacity;
    private readonly List<ViewportPresentationAuditEvent> _events = new();
    private long _sequence;
    private long _dropped;

    public ViewportPresentationAuditTrace(int capacity = 1024)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _capacity = capacity;
    }

    public int Capacity => _capacity;

    public long DroppedCount
    {
        get
        {
            lock (_sync)
                return _dropped;
        }
    }

    public int Count
    {
        get
        {
            lock (_sync)
                return _events.Count;
        }
    }

    public IReadOnlyList<ViewportPresentationAuditEvent> Snapshot()
    {
        lock (_sync)
            return _events.ToArray();
    }

    public void Record(
        string stage,
        long generation,
        long submissionSequence,
        ViewportRenderDeliveryStatus? deliveryStatus = null,
        int renderedUnits = 0,
        int deferredUnits = 0,
        string evidenceKey = "")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stage);

        if (generation < 0)
            throw new ArgumentOutOfRangeException(nameof(generation));

        if (submissionSequence < 0)
            throw new ArgumentOutOfRangeException(nameof(submissionSequence));

        if (renderedUnits < 0)
            throw new ArgumentOutOfRangeException(nameof(renderedUnits));

        if (deferredUnits < 0)
            throw new ArgumentOutOfRangeException(nameof(deferredUnits));

        lock (_sync)
        {
            if (_events.Count >= _capacity)
            {
                _events.RemoveAt(0);
                _dropped++;
            }

            _events.Add(
                new ViewportPresentationAuditEvent(
                    ++_sequence,
                    stage,
                    generation,
                    submissionSequence,
                    deliveryStatus,
                    renderedUnits,
                    deferredUnits,
                    evidenceKey ?? ""));
        }
    }

    public void Reset()
    {
        lock (_sync)
        {
            _events.Clear();
            _sequence = 0;
            _dropped = 0;
        }
    }

    public bool IsStrictlyOrdered()
    {
        lock (_sync)
        {
            long previous = 0;

            foreach (var item in _events)
            {
                if (item.Sequence <= previous)
                    return false;

                previous = item.Sequence;
            }

            return true;
        }
    }

    public IReadOnlyList<string> Validate()
    {
        lock (_sync)
        {
            var errors = new List<string>();
            long previousSequence = 0;

            foreach (var item in _events)
            {
                if (item.Sequence <= previousSequence)
                    errors.Add("Audit event sequence must increase strictly.");

                if (item.Generation < 0 ||
                    item.SubmissionSequence < 0 ||
                    item.RenderedUnits < 0 ||
                    item.DeferredUnits < 0)
                {
                    errors.Add("Audit event contains invalid counters.");
                }

                previousSequence = item.Sequence;
            }

            return errors;
        }
    }
}

public static class ViewportRenderDiagnosticsRuntime
{
    public static ViewportRenderEvidenceManifest BuildManifest<TTile>(
        ViewportRenderPipelineFrame<TTile> frame,
        ViewportRenderFrameState? frameState = null,
        ViewportRenderReplaySnapshot? replay = null)
    {
        ArgumentNullException.ThrowIfNull(frame);

        return new ViewportRenderEvidenceManifest(
            frame.Composite.Generation,
            frame.Submission.Sequence,
            frame.Submission.DirtyFlags,
            frame.Batch.ItemCount,
            frame.Batch.RegionCount,
            frame.Batch.TileCount,
            frame.Batch.RoiCount,
            frame.Batch.OverlayCount,
            frame.Batch.InvalidationCount,
            frame.Batch.FullSurfaceCount,
            frame.Batch.ItemCount,
            frameState?.RenderedUnits ?? 0,
            frameState?.DeferredUnits ?? (frame.HasDeferredWork ? 1 : 0),
            ViewportRenderEvidenceRuntime.ComputeBatchHash(frame.Batch),
            ViewportRenderEvidenceRuntime.ComputeCommandStreamHash(frame.CommandStream),
            ViewportRenderEvidenceRuntime.ComputePipelineFrameHash(frame),
            replay?.EvidenceHash ?? string.Empty);
    }

    public static IReadOnlyList<string> ValidateManifest(
        ViewportRenderEvidenceManifest manifest)
    {
        var errors = new List<string>();

        if (manifest.Generation < 0 ||
            manifest.SubmissionSequence < 0)
        {
            errors.Add("Manifest generation and submission sequence must be non-negative.");
        }

        if (manifest.BatchItemCount < 0 ||
            manifest.RegionCount < 0 ||
            manifest.TileCount < 0 ||
            manifest.RoiCount < 0 ||
            manifest.OverlayCount < 0 ||
            manifest.InvalidationCount < 0 ||
            manifest.FullSurfaceCount < 0 ||
            manifest.PlannedUnits < 0 ||
            manifest.RenderedUnits < 0 ||
            manifest.DeferredUnits < 0)
        {
            errors.Add("Manifest counters must be non-negative.");
        }

        if (manifest.RenderedUnits + manifest.DeferredUnits >
            manifest.PlannedUnits)
        {
            errors.Add("Manifest rendered plus deferred units exceed planned units.");
        }

        if (manifest.BatchHash.Length != 64 ||
            manifest.CommandHash.Length != 64 ||
            manifest.FrameHash.Length != 64 ||
            (manifest.ReplayHash.Length != 0 &&
             manifest.ReplayHash.Length != 64))
        {
            errors.Add("Manifest evidence hashes must be SHA-256 length.");
        }

        return errors;
    }
}
