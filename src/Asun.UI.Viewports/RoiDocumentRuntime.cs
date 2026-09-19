using System.Collections.Immutable;
using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct RoiDocumentItem(
    Guid Id,
    RoiGeometry Geometry,
    int ZIndex);

public readonly record struct RoiDocumentHit(
    Guid Id,
    RoiHitResult Hit);

public readonly record struct RoiDocumentEvent(
    RoiEditorEvent EditorEvent,
    Guid? RoiId,
    bool SelectionChanged,
    bool DocumentChanged);

public sealed class RoiDocumentSnapshot
{
    internal RoiDocumentSnapshot(
        IEnumerable<RoiDocumentItem> items,
        Guid? selectedId,
        RoiEditorMode mode)
    {
        Items = items.ToImmutableArray();
        SelectedId = selectedId;
        Mode = mode;
    }

    public IReadOnlyList<RoiDocumentItem> Items { get; }

    public Guid? SelectedId { get; }

    public RoiEditorMode Mode { get; }
}

/// <summary>
/// Complete multi-ROI editor/session runtime. It adds selection, z-order hit testing,
/// create/duplicate/delete, transactional pointer editing, and snapshot-based undo/redo
/// on top of the framework-neutral single-ROI editor.
/// </summary>
public sealed class RoiDocumentRuntime
{
    private sealed class Entry
    {
        public Entry(Guid id, RoiEditorRuntime editor)
        {
            Id = id;
            Editor = editor;
        }

        public Guid Id { get; }

        public RoiEditorRuntime Editor { get; }
    }

    private readonly object _sync = new();
    private readonly List<Entry> _entries = new();
    private readonly Stack<RoiDocumentSnapshot> _undo = new();
    private readonly Stack<RoiDocumentSnapshot> _redo = new();

    private Guid? _selectedId;
    private RoiEditorMode _mode = RoiEditorMode.Select;
    private RoiDocumentSnapshot? _pointerTransactionStart;
    private bool _pointerTransactionMutated;

    public RoiEditorMode Mode
    {
        get
        {
            lock (_sync)
                return _mode;
        }
        set
        {
            lock (_sync)
            {
                _mode = value;

                foreach (var entry in _entries)
                    entry.Editor.Mode = RoiEditorMode.Select;
            }
        }
    }

    public Guid? SelectedId
    {
        get
        {
            lock (_sync)
                return _selectedId;
        }
    }

    public int Count
    {
        get
        {
            lock (_sync)
                return _entries.Count;
        }
    }

    public bool CanUndo
    {
        get
        {
            lock (_sync)
                return _undo.Count > 0;
        }
    }

    public bool CanRedo
    {
        get
        {
            lock (_sync)
                return _redo.Count > 0;
        }
    }

    public IReadOnlyList<RoiDocumentItem> Items
    {
        get
        {
            lock (_sync)
                return BuildItemsUnsafe();
        }
    }

    public RoiDocumentSnapshot Snapshot
    {
        get
        {
            lock (_sync)
                return CreateSnapshotUnsafe();
        }
    }

    public Guid Add(
        RoiGeometry geometry,
        Guid? id = null)
    {
        ArgumentNullException.ThrowIfNull(geometry);

        lock (_sync)
        {
            var before = CreateSnapshotUnsafe();
            var entry = new Entry(id ?? Guid.NewGuid(), new RoiEditorRuntime());
            entry.Editor.SetGeometry(geometry, commit: true);
            _entries.Add(entry);
            _selectedId = entry.Id;
            PushUndoUnsafe(before);
            return entry.Id;
        }
    }

    public Guid? DuplicateSelected(Vector2 offset = default)
    {
        ValidateFinite(offset);

        lock (_sync)
        {
            var selected = FindEntryUnsafe(_selectedId);
            if (selected is null)
                return null;

            var before = CreateSnapshotUnsafe();
            var duplicate = new Entry(Guid.NewGuid(), new RoiEditorRuntime());
            duplicate.Editor.SetGeometry(
                selected.Editor.Geometry!.Translate(offset),
                commit: true);

            _entries.Add(duplicate);
            _selectedId = duplicate.Id;
            PushUndoUnsafe(before);
            return duplicate.Id;
        }
    }

    public bool DeleteSelected()
    {
        lock (_sync)
        {
            if (_selectedId is null)
                return false;

            var index = FindEntryIndexUnsafe(_selectedId.Value);
            if (index < 0)
                return false;

            var before = CreateSnapshotUnsafe();
            _entries.RemoveAt(index);

            if (_entries.Count == 0)
            {
                _selectedId = null;
            }
            else
            {
                var newIndex = Math.Min(index, _entries.Count - 1);
                _selectedId = _entries[newIndex].Id;
            }

            PushUndoUnsafe(before);
            return true;
        }
    }

    public bool Select(Guid? id)
    {
        lock (_sync)
        {
            if (id is null)
            {
                var changed = _selectedId is not null;
                _selectedId = null;
                return changed;
            }

            if (FindEntryUnsafe(id) is null)
                return false;

            var wasChanged = _selectedId != id;
            _selectedId = id;
            return wasChanged;
        }
    }

    public bool MoveSelectedToFront() =>
        ReorderSelectedUnsafe(toFront: true);

    public bool MoveSelectedToBack() =>
        ReorderSelectedUnsafe(toFront: false);

    public RoiDocumentHit HitTest(
        Vector2 point,
        float handleTolerance = 8f,
        float bodyTolerance = 0f)
    {
        ValidateFinite(point);

        lock (_sync)
        {
            for (var i = _entries.Count - 1; i >= 0; i--)
            {
                var entry = _entries[i];
                var geometry = entry.Editor.Geometry;
                if (geometry is null)
                    continue;

                var hit = RoiHitTester.HitTest(
                    geometry,
                    point,
                    handleTolerance,
                    bodyTolerance);

                if (hit.Hit)
                    return new RoiDocumentHit(entry.Id, hit);
            }

            return new RoiDocumentHit(Guid.Empty, RoiHitResult.None);
        }
    }

    public RoiDocumentEvent PointerDown(
        Vector2 pointer,
        float handleTolerance = 8f,
        float bodyTolerance = 0f)
    {
        ValidateFinite(pointer);

        lock (_sync)
        {
            _pointerTransactionStart = CreateSnapshotUnsafe();
            _pointerTransactionMutated = false;

            if (_mode is RoiEditorMode.CreateRectangle or RoiEditorMode.CreateEllipse)
            {
                var entry = CreateEntryUnsafe();
                _entries.Add(entry);
                _selectedId = entry.Id;

                entry.Editor.Mode = _mode;
                var editorEvent = entry.Editor.PointerDown(
                    pointer,
                    handleTolerance);

                _pointerTransactionMutated = true;
                return new RoiDocumentEvent(
                    editorEvent,
                    entry.Id,
                    true,
                    true);
            }

            var documentHit = HitTestUnsafe(
                pointer,
                handleTolerance,
                bodyTolerance);

            if (!documentHit.Hit.Hit)
            {
                var changed = _selectedId is not null;
                _selectedId = null;

                return new RoiDocumentEvent(
                    new RoiEditorEvent(
                        RoiEditorEventKind.PointerDown,
                        RoiInteractionKind.Idle,
                        RoiHandleKind.None,
                        pointer,
                        false),
                    null,
                    changed,
                    changed);
            }

            var entryForHit = FindEntryUnsafe(documentHit.Id)!;
            var selectionChanged = _selectedId != entryForHit.Id;
            _selectedId = entryForHit.Id;

            var editorEventForHit = entryForHit.Editor.PointerDown(
                pointer,
                handleTolerance);

            _pointerTransactionMutated =
                editorEventForHit.Interaction != RoiInteractionKind.Idle;

            return new RoiDocumentEvent(
                editorEventForHit,
                entryForHit.Id,
                selectionChanged,
                _pointerTransactionMutated || selectionChanged);
        }
    }

    public RoiDocumentEvent PointerMove(
        Vector2 pointer,
        float handleTolerance = 8f)
    {
        ValidateFinite(pointer);

        lock (_sync)
        {
            var entry = FindEntryUnsafe(_selectedId);
            if (entry is null)
            {
                return new RoiDocumentEvent(
                    new RoiEditorEvent(
                        RoiEditorEventKind.PointerMove,
                        RoiInteractionKind.Idle,
                        RoiHandleKind.None,
                        pointer,
                        false),
                    null,
                    false,
                    false);
            }

            var editorEvent = entry.Editor.PointerMove(
                pointer,
                handleTolerance);

            return new RoiDocumentEvent(
                editorEvent,
                entry.Id,
                false,
                editorEvent.Interaction != RoiInteractionKind.Idle ||
                entry.Editor.Geometry != entry.Editor.CommittedGeometry);
        }
    }

    public RoiDocumentEvent PointerUp(Vector2 pointer)
    {
        ValidateFinite(pointer);

        lock (_sync)
        {
            var entry = FindEntryUnsafe(_selectedId);
            if (entry is null)
                return new RoiDocumentEvent(
                    new RoiEditorEvent(
                        RoiEditorEventKind.PointerUp,
                        RoiInteractionKind.Idle,
                        RoiHandleKind.None,
                        pointer,
                        false),
                    null,
                    false,
                    false);

            var editorEvent = entry.Editor.PointerUp(pointer);
            var changed = _pointerTransactionMutated &&
                          HasDocumentChangedUnsafe(_pointerTransactionStart);

            if (changed && _pointerTransactionStart is not null)
                PushUndoUnsafe(_pointerTransactionStart);

            ClearPointerTransactionUnsafe();

            return new RoiDocumentEvent(
                editorEvent,
                entry.Id,
                false,
                changed);
        }
    }

    public RoiDocumentEvent Cancel(Vector2 pointer)
    {
        ValidateFinite(pointer);

        lock (_sync)
        {
            var entry = FindEntryUnsafe(_selectedId);
            if (entry is null)
            {
                ClearPointerTransactionUnsafe();
                return new RoiDocumentEvent(
                    new RoiEditorEvent(
                        RoiEditorEventKind.Cancelled,
                        RoiInteractionKind.Idle,
                        RoiHandleKind.None,
                        pointer,
                        false),
                    null,
                    false,
                    false);
            }

            var editorEvent = entry.Editor.Cancel(pointer);

            if (_pointerTransactionStart is not null)
            {
                RestoreSnapshotUnsafe(_pointerTransactionStart);
            }

            ClearPointerTransactionUnsafe();

            return new RoiDocumentEvent(
                editorEvent,
                _selectedId,
                false,
                false);
        }
    }

    public bool Undo()
    {
        lock (_sync)
        {
            if (_undo.Count == 0)
                return false;

            var current = CreateSnapshotUnsafe();
            var previous = _undo.Pop();
            _redo.Push(current);
            RestoreSnapshotUnsafe(previous);
            ClearPointerTransactionUnsafe();
            return true;
        }
    }

    public bool Redo()
    {
        lock (_sync)
        {
            if (_redo.Count == 0)
                return false;

            var current = CreateSnapshotUnsafe();
            var next = _redo.Pop();
            _undo.Push(current);
            RestoreSnapshotUnsafe(next);
            ClearPointerTransactionUnsafe();
            return true;
        }
    }

    public RoiDocumentSnapshot CreateSnapshot()
    {
        lock (_sync)
            return CreateSnapshotUnsafe();
    }

    public void RestoreSnapshot(RoiDocumentSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        lock (_sync)
        {
            RestoreSnapshotUnsafe(snapshot);
            ClearPointerTransactionUnsafe();
        }
    }

    private Entry CreateEntryUnsafe()
    {
        var entry = new Entry(Guid.NewGuid(), new RoiEditorRuntime());
        entry.Editor.Mode = RoiEditorMode.Select;
        return entry;
    }

    private RoiDocumentHit HitTestUnsafe(
        Vector2 point,
        float handleTolerance,
        float bodyTolerance)
    {
        for (var i = _entries.Count - 1; i >= 0; i--)
        {
            var entry = _entries[i];
            var geometry = entry.Editor.Geometry;
            if (geometry is null)
                continue;

            var hit = RoiHitTester.HitTest(
                geometry,
                point,
                handleTolerance,
                bodyTolerance);

            if (hit.Hit)
                return new RoiDocumentHit(entry.Id, hit);
        }

        return new RoiDocumentHit(Guid.Empty, RoiHitResult.None);
    }

    private bool ReorderSelectedUnsafe(bool toFront)
    {
        if (_selectedId is null)
            return false;

        var index = FindEntryIndexUnsafe(_selectedId.Value);
        if (index < 0)
            return false;

        var target = toFront ? _entries.Count - 1 : 0;
        if (index == target)
            return false;

        var before = CreateSnapshotUnsafe();
        var entry = _entries[index];
        _entries.RemoveAt(index);

        if (toFront)
            _entries.Add(entry);
        else
            _entries.Insert(0, entry);

        PushUndoUnsafe(before);
        return true;
    }

    private List<RoiDocumentItem> BuildItemsUnsafe()
    {
        var items = new List<RoiDocumentItem>(_entries.Count);
        for (var i = 0; i < _entries.Count; i++)
        {
            var geometry = _entries[i].Editor.Geometry;
            if (geometry is not null)
                items.Add(new RoiDocumentItem(
                    _entries[i].Id,
                    geometry,
                    i));
        }

        return items;
    }

    private RoiDocumentSnapshot CreateSnapshotUnsafe() =>
        new(
            BuildItemsUnsafe(),
            _selectedId,
            _mode);

    private bool HasDocumentChangedUnsafe(RoiDocumentSnapshot? before)
    {
        if (before is null)
            return false;

        var current = CreateSnapshotUnsafe();
        if (current.Mode != before.Mode ||
            current.SelectedId != before.SelectedId ||
            current.Items.Count != before.Items.Count)
        {
            return true;
        }

        for (var i = 0; i < current.Items.Count; i++)
        {
            var a = current.Items[i];
            var b = before.Items[i];

            if (a.Id != b.Id ||
                a.ZIndex != b.ZIndex ||
                !a.Geometry.Equals(b.Geometry))
            {
                return true;
            }
        }

        return false;
    }

    private void PushUndoUnsafe(RoiDocumentSnapshot snapshot)
    {
        _undo.Push(snapshot);
        _redo.Clear();
    }

    private void RestoreSnapshotUnsafe(RoiDocumentSnapshot snapshot)
    {
        _entries.Clear();

        foreach (var item in snapshot.Items.OrderBy(item => item.ZIndex))
        {
            var entry = new Entry(item.Id, new RoiEditorRuntime());
            entry.Editor.SetGeometry(item.Geometry, commit: true);
            entry.Editor.Mode = RoiEditorMode.Select;
            _entries.Add(entry);
        }

        _selectedId = snapshot.SelectedId;
        _mode = snapshot.Mode;
    }

    private Entry? FindEntryUnsafe(Guid? id)
    {
        if (id is null)
            return null;

        return _entries.FirstOrDefault(entry => entry.Id == id.Value);
    }

    private int FindEntryIndexUnsafe(Guid id)
    {
        for (var i = 0; i < _entries.Count; i++)
        {
            if (_entries[i].Id == id)
                return i;
        }

        return -1;
    }

    private void ClearPointerTransactionUnsafe()
    {
        _pointerTransactionStart = null;
        _pointerTransactionMutated = false;
    }

    private static void ValidateFinite(Vector2 point)
    {
        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            throw new ArgumentOutOfRangeException(nameof(point));
    }
}
