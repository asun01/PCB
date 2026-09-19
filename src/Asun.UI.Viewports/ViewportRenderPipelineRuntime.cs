using System.Numerics;

namespace Asun.UI.Viewports;

public sealed class ViewportRenderPipelineFrame<TTile>
{
    internal ViewportRenderPipelineFrame(
        ViewportCompositeFrame<TTile> composite,
        ViewportRenderSubmission submission,
        ViewportRenderWorkPlan workPlan,
        ViewportRenderBatch batch,
        bool hasDeferredWork)
    {
        Composite = composite;
        Submission = submission;
        WorkPlan = workPlan;
        Batch = batch;
        HasDeferredWork = hasDeferredWork;
    }

    public ViewportCompositeFrame<TTile> Composite { get; }

    public ViewportRenderSubmission Submission { get; }

    public ViewportRenderWorkPlan WorkPlan { get; }

    public ViewportRenderBatch Batch { get; }

    public bool HasDeferredWork { get; }

    public bool Accepted => Submission.Sequence > 0;
}

public sealed class ViewportRenderPipelineRuntime<TTile> : IDisposable
{
    private readonly object _sync = new();
    private readonly ViewportCompositeRuntime<TTile> _composite;
    private readonly ViewportRenderSchedulerRuntime _scheduler;
    private readonly ViewportRenderBudget _budget;
    private readonly ViewportRenderReuseRuntime<TTile> _reuse = new();
    private ViewportRenderWorkPlan? _deferredWork;
    private ViewportCompositeFrame<TTile>? _deferredComposite;
    private long _deferredGeneration = -1;
    private int _disposed;

    public ViewportRenderPipelineRuntime(
        Vector2 imageSize,
        Vector2 viewportSize,
        Vector2 tileSize,
        int prefetchMarginTiles,
        int cacheCapacity,
        int maxConcurrency,
        ITileSource<TTile> tileSource,
        ViewportRenderBudget? budget = null,
        double framesPerSecond = 60,
        RoiEditorMode roiMode = RoiEditorMode.Select)
    {
        _composite = new ViewportCompositeRuntime<TTile>(
            imageSize,
            viewportSize,
            tileSize,
            prefetchMarginTiles,
            cacheCapacity,
            maxConcurrency,
            tileSource,
            roiMode);

        _scheduler = new ViewportRenderSchedulerRuntime(
            framesPerSecond);

        _budget = (budget ?? ViewportRenderBudget.Default)
            .Validate();
    }

    public ViewportCompositeRuntime<TTile> Composite => _composite;

    public ViewportRenderSchedulerRuntime Scheduler => _scheduler;

    public ViewportRenderBudget Budget => _budget;

    public ViewportRenderReuseRuntime<TTile> Reuse => _reuse;

    public bool TryReuse(
        long generation,
        out ViewportRenderPipelineFrame<TTile> frame)
    {
        ThrowIfDisposed();
        return _reuse.TryReuse(generation, out frame);
    }

    public void Invalidate(
        ViewportDirtyFlags flags,
        long generation)
    {
        ThrowIfDisposed();
        _scheduler.Submit(flags, generation);
    }

    public void SubmitPointer(Vector2 viewportPoint)
    {
        ThrowIfDisposed();
        _scheduler.SubmitPointer(viewportPoint);
    }

    public async ValueTask<ViewportRenderPipelineFrame<TTile>?> RefreshAsync(
        DateTimeOffset now,
        bool includePrefetch = false,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        var composite = await _composite
            .RefreshAsync(includePrefetch, cancellationToken)
            .ConfigureAwait(false);

        var dirtyFlags = _composite.ConsumeDirtyFlags();

        _scheduler.Submit(
            dirtyFlags,
            composite.Generation);

        if (!_scheduler.TryTakeFrame(now, out var submission, composite.Generation))
            return null;

        ViewportRenderWorkPlan prioritized;

        lock (_sync)
        {
            if (_deferredWork is not null &&
                _deferredGeneration == composite.Generation)
            {
                prioritized = _deferredWork;
            }
            else
            {
                var plan = ViewportRenderWorkRuntime.Plan(
                    composite,
                    submission.DirtyFlags);

                prioritized = ViewportRenderPriorityRuntime.Prioritize(
                    plan,
                    composite);
            }
        }

        var budgeted = ViewportRenderBudgetRuntime.Apply(
            prioritized,
            _budget);

        var batch = ViewportRenderBatchRuntime.Create(
            budgeted,
            composite.Tiles.Transform);

        var hasDeferredWork =
            budgeted.Items.Count < prioritized.Items.Count;

        lock (_sync)
        {
            if (hasDeferredWork)
            {
                var remaining = GetRemainingWork(
                    prioritized,
                    budgeted);

                _deferredWork = remaining;
                _deferredGeneration = composite.Generation;
            }
            else
            {
                _deferredWork = null;
                _deferredGeneration = -1;
            }
        }

        if (hasDeferredWork)
        {
            _scheduler.Submit(
                submission.DirtyFlags,
                composite.Generation);
        }

        var result = new ViewportRenderPipelineFrame<TTile>(
            composite,
            submission,
            budgeted,
            batch,
            hasDeferredWork);

        _reuse.Store(result);
        return result;
    }

    public bool TryTakePointer(
        out CoalescedPointer pointer)
    {
        ThrowIfDisposed();
        return _scheduler.TryTakePointer(out pointer);
    }

    public void RequeueFrame(ViewportRenderPipelineFrame<TTile> frame)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ThrowIfDisposed();

        var generation = frame.Composite.Generation;

        lock (_sync)
        {
            if (_composite.Generation != generation)
                return;

            var existing = _deferredWork;
            var items = existing is null
                ? frame.WorkPlan.Items.ToArray()
                : frame.WorkPlan.Items
                    .Concat(existing.Items)
                    .ToArray();

            _deferredWork = new ViewportRenderWorkPlan(
                items,
                frame.WorkPlan.ConsumedFlags |
                (existing?.ConsumedFlags ?? ViewportDirtyFlags.None),
                generation);
            _deferredComposite = frame.Composite;
            _deferredGeneration = generation;
        }
    }

    public ViewportRenderPipelineFrame<TTile> BuildFromFrame(
        ViewportCompositeFrame<TTile> composite,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(composite);
        ThrowIfDisposed();

        var dirtyFlags = _composite.ConsumeDirtyFlags();

        _scheduler.Submit(
            dirtyFlags,
            composite.Generation);

        if (!_scheduler.TryTakeFrame(now, out var submission, composite.Generation))
        {
            var emptyResult = new ViewportRenderPipelineFrame<TTile>(
                composite,
                default,
                ViewportRenderWorkRuntime.Plan(
                    composite,
                    ViewportDirtyFlags.None),
                ViewportRenderBatchRuntime.Empty(
                    composite.Generation),
                hasDeferredWork: false);

            return emptyResult;
        }

        ViewportRenderWorkPlan prioritized;

        lock (_sync)
        {
            if (_deferredWork is not null &&
                _deferredGeneration == composite.Generation)
            {
                prioritized = _deferredWork;
            }
            else
            {
                var plan = ViewportRenderWorkRuntime.Plan(
                    composite,
                    submission.DirtyFlags);

                prioritized = ViewportRenderPriorityRuntime.Prioritize(
                    plan,
                    composite);
            }
        }

        var budgeted = ViewportRenderBudgetRuntime.Apply(
            prioritized,
            _budget);

        var hasDeferredWork =
            budgeted.Items.Count < prioritized.Items.Count;

        lock (_sync)
        {
            if (hasDeferredWork)
            {
                _deferredWork = GetRemainingWork(
                    prioritized,
                    budgeted);
                _deferredGeneration = composite.Generation;
            }
            else
            {
                _deferredWork = null;
                _deferredGeneration = -1;
            }
        }

        if (hasDeferredWork)
        {
            _scheduler.Submit(
                submission.DirtyFlags,
                composite.Generation);
        }

        var result = new ViewportRenderPipelineFrame<TTile>(
            composite,
            submission,
            budgeted,
            ViewportRenderBatchRuntime.Create(
                budgeted,
                composite.Tiles.Transform),
            hasDeferredWork);

        _reuse.Store(result);
        return result;
    }

    public void Reset()
    {
        _scheduler.Reset();
        _reuse.Clear();

        lock (_sync)
        {
            _deferredWork = null;
            _deferredGeneration = -1;
        }
    }

    private static ViewportRenderWorkPlan GetRemainingWork(
        ViewportRenderWorkPlan prioritized,
        ViewportRenderWorkPlan budgeted)
    {
        if (budgeted.IsEmpty)
        {
            return prioritized;
        }

        var selected = budgeted.Items.ToHashSet();
        var remaining = prioritized.Items
            .Where(item => !selected.Contains(item))
            .ToArray();

        return new ViewportRenderWorkPlan(
            remaining,
            prioritized.ConsumedFlags,
            prioritized.Generation);
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        _composite.Dispose();
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
}
