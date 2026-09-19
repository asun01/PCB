using System.Drawing;

namespace Asun.UI.Viewports;

public enum ViewportPresentationBufferState
{
    Available,
    Rendering,
    Presented,
    Disposed
}

public readonly record struct ViewportPresentationBufferTransaction(
    int SlotIndex,
    ViewportPresentationSubmissionToken Submission);

public readonly record struct ViewportPresentationBufferSnapshot(
    ViewportPresentationBufferState FirstState,
    ViewportPresentationBufferState SecondState,
    int? RenderingSlot,
    int? PresentedSlot,
    long? RenderingGeneration,
    long? PresentedGeneration,
    long? RenderingSequence,
    long? PresentedSequence,
    int LastPlannedUnits,
    int LastRenderedUnits,
    IReadOnlyList<RectangleF> PresentedRegions);

public readonly record struct ViewportPresentationBufferStatistics(
    long Acquired,
    long Committed,
    long Discarded,
    long Rejected,
    long RegionCommits);

public sealed class ViewportPresentationBufferRuntime : IDisposable
{
    private sealed class Slot
    {
        public ViewportPresentationBufferState State =
            ViewportPresentationBufferState.Available;

        public long? Generation;

        public long? Sequence;

        public int PlannedUnits;

        public int RenderedUnits;

        public IReadOnlyList<RectangleF> Regions =
            Array.Empty<RectangleF>();
    }

    private readonly object _sync = new();
    private readonly Slot[] _slots = { new(), new() };
    private int? _renderingSlot;
    private int? _presentedSlot;
    private long _acquired;
    private long _committed;
    private long _discarded;
    private long _rejected;
    private long _regionCommits;
    private long _lastSubmissionSequence;
    private int _disposed;

    public ViewportPresentationBufferSnapshot Snapshot
    {
        get
        {
            lock (_sync)
            {
                var first = _slots[0];
                var second = _slots[1];

                Slot? rendering = _renderingSlot is int renderingIndex
                    ? _slots[renderingIndex]
                    : null;

                Slot? presented = _presentedSlot is int presentedIndex
                    ? _slots[presentedIndex]
                    : null;

                return new(
                    first.State,
                    second.State,
                    _renderingSlot,
                    _presentedSlot,
                    rendering?.Generation,
                    presented?.Generation,
                    rendering?.Sequence,
                    presented?.Sequence,
                    presented?.PlannedUnits ?? 0,
                    presented?.RenderedUnits ?? 0,
                    presented?.Regions.ToArray() ??
                    Array.Empty<RectangleF>());
            }
        }
    }

    public ViewportPresentationBufferStatistics Statistics
    {
        get
        {
            lock (_sync)
            {
                return new(
                    _acquired,
                    _committed,
                    _discarded,
                    _rejected,
                    _regionCommits);
            }
        }
    }

    public ViewportPresentationBufferTransaction Begin(
        ViewportPresentationSubmissionToken submission,
        int plannedUnits,
        IReadOnlyList<RectangleF>? regions = null)
    {
        if (plannedUnits < 0)
            throw new ArgumentOutOfRangeException(nameof(plannedUnits));

        lock (_sync)
        {
            ThrowIfDisposed();

            if (_renderingSlot is not null)
                throw new InvalidOperationException(
                    "A presentation buffer is already rendering.");

            if (submission.Sequence <= _lastSubmissionSequence)
            {
                _rejected++;
                throw new InvalidOperationException(
                    "An old presentation fence cannot acquire a backbuffer.");
            }

            if (_presentedSlot is int currentPresented)
            {
                var presentedSlot = _slots[currentPresented];

                if (presentedSlot.Generation is long presentedGeneration &&
                    submission.Generation < presentedGeneration)
                {
                    _rejected++;
                    throw new InvalidOperationException(
                        "A stale presentation cannot acquire a backbuffer.");
                }

                if (presentedSlot.Sequence is long presentedSequence &&
                    submission.Generation == presentedSlot.Generation &&
                    submission.Sequence <= presentedSequence)
                {
                    _rejected++;
                    throw new InvalidOperationException(
                        "An older presentation sequence cannot acquire a backbuffer.");
                }
            }

            var slotIndex = _presentedSlot is 0 ? 1 : 0;

            if (_slots[slotIndex].State !=
                ViewportPresentationBufferState.Available)
            {
                _rejected++;
                throw new InvalidOperationException(
                    "No presentation backbuffer is currently available.");
            }

            var slot = _slots[slotIndex];
            slot.State = ViewportPresentationBufferState.Rendering;
            slot.Generation = submission.Generation;
            slot.Sequence = submission.Sequence;
            slot.PlannedUnits = plannedUnits;
            slot.RenderedUnits = 0;
            slot.Regions = regions is null
                ? Array.Empty<RectangleF>()
                : regions.ToArray();

            _lastSubmissionSequence = submission.Sequence;
            _renderingSlot = slotIndex;
            _acquired++;

            return new ViewportPresentationBufferTransaction(
                slotIndex,
                submission);
        }
    }

    public void Commit(
        ViewportPresentationBufferTransaction transaction,
        int renderedUnits,
        IReadOnlyList<RectangleF>? regions = null,
        Func<bool>? presentationFence = null)
    {
        if (renderedUnits < 0)
            throw new ArgumentOutOfRangeException(nameof(renderedUnits));

        lock (_sync)
        {
            ThrowIfDisposed();

            if (_renderingSlot != transaction.SlotIndex ||
                transaction.SlotIndex is < 0 or > 1)
            {
                throw new InvalidOperationException(
                    "Only the active presentation backbuffer can be committed.");
            }

            var slot = _slots[transaction.SlotIndex];

            if (slot.State != ViewportPresentationBufferState.Rendering ||
                slot.Generation != transaction.Submission.Generation ||
                slot.Sequence != transaction.Submission.Sequence)
            {
                throw new InvalidOperationException(
                    "The presentation backbuffer transaction is stale.");
            }

            if (presentationFence is not null &&
                !presentationFence())
                throw new ViewportPresentationFenceRejectedException();

            var committedRegions = regions is null
                ? slot.Regions
                : regions.ToArray();

            if (_presentedSlot is int previousPresented)
            {
                _slots[previousPresented].State =
                    ViewportPresentationBufferState.Available;
            }

            slot.State = ViewportPresentationBufferState.Presented;
            slot.RenderedUnits = renderedUnits;
            slot.Regions = committedRegions;

            _presentedSlot = transaction.SlotIndex;
            _renderingSlot = null;
            _committed++;
            _regionCommits += committedRegions.Count;
        }
    }

    public void Discard(
        ViewportPresentationBufferTransaction transaction)
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            if (_renderingSlot != transaction.SlotIndex)
                return;

            var slot = _slots[transaction.SlotIndex];

            if (slot.State != ViewportPresentationBufferState.Rendering ||
                slot.Generation != transaction.Submission.Generation ||
                slot.Sequence != transaction.Submission.Sequence)
            {
                return;
            }

            slot.State = ViewportPresentationBufferState.Available;
            slot.Generation = null;
            slot.Sequence = null;
            slot.PlannedUnits = 0;
            slot.RenderedUnits = 0;
            slot.Regions = Array.Empty<RectangleF>();

            _renderingSlot = null;
            _discarded++;
        }
    }

    public void Reset()
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            foreach (var slot in _slots)
            {
                slot.State = ViewportPresentationBufferState.Available;
                slot.Generation = null;
                slot.Sequence = null;
                slot.PlannedUnits = 0;
                slot.RenderedUnits = 0;
                slot.Regions = Array.Empty<RectangleF>();
            }

            _renderingSlot = null;
            _presentedSlot = null;
            _acquired = 0;
            _committed = 0;
            _discarded = 0;
            _rejected = 0;
            _regionCommits = 0;
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        lock (_sync)
        {
            foreach (var slot in _slots)
                slot.State = ViewportPresentationBufferState.Disposed;

            _renderingSlot = null;
            _presentedSlot = null;
        }
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
}
