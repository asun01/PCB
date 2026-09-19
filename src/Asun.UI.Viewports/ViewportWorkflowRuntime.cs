using System.Collections.Immutable;
using System.Numerics;

namespace Asun.UI.Viewports;

public enum ViewportWorkflowOperation
{
    Fit,
    ResizeViewport,
    Pan,
    PanClamped,
    Zoom,
    CenterOnImagePoint,
    AddRectangle,
    AddEllipse,
    Select,
    TranslateSelected,
    DuplicateSelected,
    DeleteSelected,
    MoveSelectedToFront,
    MoveSelectedToBack,
    Undo,
    Redo
}

public readonly record struct ViewportWorkflowCommand(
    ViewportWorkflowOperation Operation,
    Vector2 Vector,
    double Value,
    double SecondaryValue,
    Guid? RoiId)
{
    public static ViewportWorkflowCommand Fit() =>
        new(ViewportWorkflowOperation.Fit, default, 0, 0, 0, null);

    public static ViewportWorkflowCommand Resize(Vector2 size) =>
        new(ViewportWorkflowOperation.ResizeViewport, size, 0, 0, 0, null);

    public static ViewportWorkflowCommand Pan(Vector2 delta, bool clamp = true) =>
        new(
            clamp
                ? ViewportWorkflowOperation.PanClamped
                : ViewportWorkflowOperation.Pan,
            delta,
            0,
            0,
            null);

    public static ViewportWorkflowCommand Zoom(
        double factor,
        double minScale,
        double maxScale,
        Vector2 anchor) =>
        new(
            ViewportWorkflowOperation.Zoom,
            anchor,
            factor,
            minScale,
            null) with { SecondaryValue = maxScale };

    public static ViewportWorkflowCommand Center(Vector2 imagePoint) =>
        new(ViewportWorkflowOperation.CenterOnImagePoint, imagePoint, 0, 0, 0, null);

    public static ViewportWorkflowCommand AddRectangle(
        Vector2 center,
        Vector2 size) =>
        new(ViewportWorkflowOperation.AddRectangle, center, size.X, size.Y, 0, null);

    public static ViewportWorkflowCommand AddEllipse(
        Vector2 center,
        Vector2 size) =>
        new(ViewportWorkflowOperation.AddEllipse, center, size.X, size.Y, 0, null);

    public static ViewportWorkflowCommand Select(Guid? id) =>
        new(ViewportWorkflowOperation.Select, default, 0, 0, 0, id);

    public static ViewportWorkflowCommand Translate(Vector2 delta) =>
        new(ViewportWorkflowOperation.TranslateSelected, delta, 0, 0, 0, null);

    public static ViewportWorkflowCommand Duplicate(Vector2 offset) =>
        new(ViewportWorkflowOperation.DuplicateSelected, offset, 0, 0, 0, null);

    public static ViewportWorkflowCommand Simple(
        ViewportWorkflowOperation operation) =>
        new(operation, default, 0, 0, 0, null);
}

public readonly record struct ViewportWorkflowRecord(
    long Sequence,
    ViewportWorkflowCommand Command,
    bool Succeeded,
    int RoiCount,
    Guid? SelectedId,
    double Scale);

public sealed class ViewportWorkflowSnapshot
{
    internal ViewportWorkflowSnapshot(
        ViewportTransform transform,
        RoiDocumentSnapshot document)
    {
        Transform = transform;
        Document = document;
    }

    public ViewportTransform Transform { get; }

    public RoiDocumentSnapshot Document { get; }
}

/// <summary>
/// UI-neutral transactional workflow runtime that composes viewport navigation
/// and the multi-ROI document runtime into replayable command chains.
/// </summary>
public sealed class ViewportWorkflowRuntime
{
    private readonly object _sync = new();
    private readonly RoiViewportRuntime _runtime;
    private readonly List<ViewportWorkflowRecord> _journal = new();
    private long _sequence;

    public ViewportWorkflowRuntime(
        Vector2 imageSize,
        Vector2 viewportSize,
        RoiEditorMode mode = RoiEditorMode.Select)
    {
        _runtime = new RoiViewportRuntime(imageSize, viewportSize, mode);
    }

    public RoiViewportRuntime Runtime => _runtime;

    public IReadOnlyList<ViewportWorkflowRecord> Journal
    {
        get
        {
            lock (_sync)
                return _journal.ToImmutableArray();
        }
    }

    public ViewportWorkflowSnapshot Snapshot()
    {
        lock (_sync)
        {
            return new ViewportWorkflowSnapshot(
                _runtime.Transform,
                _runtime.Document.CreateSnapshot());
        }
    }

    public void Restore(ViewportWorkflowSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        lock (_sync)
        {
            _runtime.SetTransform(snapshot.Transform);
            _runtime.Document.RestoreSnapshot(snapshot.Document);
        }
    }

    public Guid? Execute(ViewportWorkflowCommand command)
    {
        lock (_sync)
        {
            var before = Snapshot();
            try
            {
                var selected = ExecuteCore(command);
                AppendRecord(command, true);
                return selected;
            }
            catch
            {
                Restore(before);
                AppendRecord(command, false);
                throw;
            }
        }
    }

    public void ExecuteBatch(
        IEnumerable<ViewportWorkflowCommand> commands,
        bool rollbackOnFailure = true)
    {
        ArgumentNullException.ThrowIfNull(commands);

        lock (_sync)
        {
            var batchStart = Snapshot();

            try
            {
                foreach (var command in commands)
                    Execute(command);
            }
            catch
            {
                if (rollbackOnFailure)
                    Restore(batchStart);

                throw;
            }
        }
    }

    public bool Replay(
        IEnumerable<ViewportWorkflowCommand> commands,
        bool rollbackOnFailure = true)
    {
        ArgumentNullException.ThrowIfNull(commands);

        try
        {
            ExecuteBatch(commands, rollbackOnFailure);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private Guid? ExecuteCore(ViewportWorkflowCommand command) =>
        command.Operation switch
        {
            ViewportWorkflowOperation.Fit =>
                ExecuteFit(),
            ViewportWorkflowOperation.ResizeViewport =>
                ExecuteResize(command.Vector),
            ViewportWorkflowOperation.Pan =>
                ExecutePan(command.Vector, false),
            ViewportWorkflowOperation.PanClamped =>
                ExecutePan(command.Vector, true),
            ViewportWorkflowOperation.Zoom =>
                ExecuteZoom(command),
            ViewportWorkflowOperation.CenterOnImagePoint =>
                ExecuteCenter(command.Vector),
            ViewportWorkflowOperation.AddRectangle =>
                ExecuteAddRectangle(command),
            ViewportWorkflowOperation.AddEllipse =>
                ExecuteAddEllipse(command),
            ViewportWorkflowOperation.Select =>
                ExecuteSelect(command.RoiId),
            ViewportWorkflowOperation.TranslateSelected =>
                ExecuteTranslate(command.Vector),
            ViewportWorkflowOperation.DuplicateSelected =>
                ExecuteDuplicate(command.Vector),
            ViewportWorkflowOperation.DeleteSelected =>
                ExecuteDelete(),
            ViewportWorkflowOperation.MoveSelectedToFront =>
                ExecuteMoveFront(),
            ViewportWorkflowOperation.MoveSelectedToBack =>
                ExecuteMoveBack(),
            ViewportWorkflowOperation.Undo =>
                ExecuteUndo(),
            ViewportWorkflowOperation.Redo =>
                ExecuteRedo(),
            _ => throw new ArgumentOutOfRangeException(
                nameof(command),
                command.Operation,
                "Unsupported viewport workflow operation.")
        };

    private Guid? ExecuteFit()
    {
        _runtime.FitToViewport();
        return _runtime.SelectedId;
    }

    private Guid? ExecuteResize(Vector2 size)
    {
        _runtime.ResizeViewport(size);
        return _runtime.SelectedId;
    }

    private Guid? ExecutePan(Vector2 delta, bool clamp)
    {
        _runtime.PanBy(delta, clamp);
        return _runtime.SelectedId;
    }

    private Guid? ExecuteZoom(ViewportWorkflowCommand command)
    {
        if (!double.IsFinite(command.SecondaryValue) ||
            command.SecondaryValue <= command.Value)
        {
            throw new ArgumentOutOfRangeException(
                nameof(command),
                "Zoom maximum scale must be greater than the minimum scale.");
        }

        _runtime.ZoomAt(
            command.Value,
            command.SecondaryValue,
            Math.Max(command.SecondaryValue, 1000d),
            command.Vector);

        return _runtime.SelectedId;
    }

    private Guid? ExecuteCenter(Vector2 point)
    {
        _runtime.CenterOnImagePoint(point);
        return _runtime.SelectedId;
    }

    private Guid ExecuteAddRectangle(ViewportWorkflowCommand command)
    {
        return _runtime.Document.Add(
            RoiGeometry.CreateRectangle(
                command.Vector,
                new Vector2(
                    (float)command.SecondaryValue,
                    (float)command.Value)));
    }

    private Guid ExecuteAddEllipse(ViewportWorkflowCommand command)
    {
        return _runtime.Document.Add(
            RoiGeometry.CreateEllipse(
                command.Vector,
                new Vector2(
                    (float)command.SecondaryValue,
                    (float)command.Value)));
    }

    private Guid? ExecuteSelect(Guid? roiId)
    {
        _runtime.Document.Select(roiId);
        return _runtime.SelectedId;
    }

    private Guid? ExecuteTranslate(Vector2 delta)
    {
        _runtime.Document.TranslateSelected(delta);
        return _runtime.SelectedId;
    }

    private Guid? ExecuteDuplicate(Vector2 offset)
    {
        return _runtime.Document.DuplicateSelected(offset);
    }

    private Guid? ExecuteDelete()
    {
        _runtime.Document.DeleteSelected();
        return _runtime.SelectedId;
    }

    private Guid? ExecuteMoveFront()
    {
        _runtime.Document.MoveSelectedToFront();
        return _runtime.SelectedId;
    }

    private Guid? ExecuteMoveBack()
    {
        _runtime.Document.MoveSelectedToBack();
        return _runtime.SelectedId;
    }

    private Guid? ExecuteUndo()
    {
        _runtime.Undo();
        return _runtime.SelectedId;
    }

    private Guid? ExecuteRedo()
    {
        _runtime.Redo();
        return _runtime.SelectedId;
    }

    private void AppendRecord(
        ViewportWorkflowCommand command,
        bool succeeded)
    {
        _journal.Add(
            new ViewportWorkflowRecord(
                ++_sequence,
                command,
                succeeded,
                _runtime.Document.Count,
                _runtime.SelectedId,
                _runtime.Transform.Scale));
    }
}
