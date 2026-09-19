using System.Numerics;

namespace Asun.UI.Viewports;

/// <summary>
/// Complete single-ROI editor runtime: pointer input, hit testing, shape creation,
/// move/resize/rotate editing, commit/cancel, and immutable snapshots.
/// It is deliberately independent of WPF, DevExpress, HALCON, and camera SDKs.
/// </summary>
public sealed class RoiEditorRuntime
{
    private readonly object _sync = new();

    private RoiGeometry? _geometry;
    private RoiGeometry? _committedGeometry;
    private RoiHitResult _hover = RoiHitResult.None;
    private RoiInteractionState _interaction = RoiInteractionState.Idle;
    private RoiEditorMode _mode = RoiEditorMode.Select;

    public RoiGeometry? Geometry
    {
        get
        {
            lock (_sync)
                return _geometry;
        }
    }

    public RoiGeometry? CommittedGeometry
    {
        get
        {
            lock (_sync)
                return _committedGeometry;
        }
    }

    public RoiHitResult Hover
    {
        get
        {
            lock (_sync)
                return _hover;
        }
    }

    public RoiInteractionState Interaction
    {
        get
        {
            lock (_sync)
                return _interaction;
        }
    }

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
            }
        }
    }

    public void SetGeometry(RoiGeometry? geometry, bool commit = true)
    {
        lock (_sync)
        {
            _geometry = geometry;
            _committedGeometry = commit ? geometry : _committedGeometry;
            _interaction = RoiInteractionState.Idle;
            _hover = RoiHitResult.None;
        }
    }

    public void Clear()
    {
        lock (_sync)
        {
            _geometry = null;
            _committedGeometry = null;
            _interaction = RoiInteractionState.Idle;
            _hover = RoiHitResult.None;
        }
    }

    public RoiEditorEvent PointerDown(
        Vector2 pointer,
        float handleTolerance = 8f)
    {
        ValidatePoint(pointer);

        lock (_sync)
        {
            if (_mode == RoiEditorMode.Select && _geometry is not null)
            {
                var hit = RoiHitTester.HitTest(
                    _geometry,
                    pointer,
                    handleTolerance);

                _hover = hit;

                if (hit.Hit)
                {
                    var interaction = hit.Handle switch
                    {
                        RoiHandleKind.Body => RoiInteractionKind.Moving,
                        RoiHandleKind.Rotation => RoiInteractionKind.Rotating,
                        _ when RoiHitTester.IsResizeHandle(hit.Handle) =>
                            RoiInteractionKind.Resizing,
                        RoiHandleKind.Vertex => RoiInteractionKind.Resizing,
                        _ => RoiInteractionKind.Idle
                    };

                    if (interaction != RoiInteractionKind.Idle)
                    {
                        _interaction = RoiInteractionState.Create(
                            interaction,
                            hit.Handle,
                            hit.Index,
                            pointer,
                            _geometry,
                            _geometry);

                        return new RoiEditorEvent(
                            RoiEditorEventKind.PointerDown,
                            interaction,
                            hit.Handle,
                            pointer,
                            false);
                    }
                }
            }

            if (_mode is RoiEditorMode.CreateRectangle or RoiEditorMode.CreateEllipse)
            {
                var geometry = CreateGeometryFromMode(pointer, pointer);

                _geometry = geometry;
                _interaction = RoiInteractionState.Create(
                    RoiInteractionKind.Creating,
                    RoiHandleKind.Body,
                    -1,
                    pointer,
                    null,
                    geometry);

                return new RoiEditorEvent(
                    RoiEditorEventKind.PointerDown,
                    RoiInteractionKind.Creating,
                    RoiHandleKind.Body,
                    pointer,
                    false);
            }

            return new RoiEditorEvent(
                RoiEditorEventKind.PointerDown,
                RoiInteractionKind.Idle,
                RoiHandleKind.None,
                pointer,
                false);
        }
    }

    public RoiEditorEvent PointerMove(
        Vector2 pointer,
        float handleTolerance = 8f)
    {
        ValidatePoint(pointer);

        lock (_sync)
        {
            if (!_interaction.IsActive)
            {
                if (_geometry is not null)
                {
                    _hover = RoiHitTester.HitTest(
                        _geometry,
                        pointer,
                        handleTolerance);
                }
                else
                {
                    _hover = RoiHitResult.None;
                }

                return new RoiEditorEvent(
                    RoiEditorEventKind.PointerMove,
                    RoiInteractionKind.Idle,
                    _hover.Handle,
                    pointer,
                    false);
            }

            var start = _interaction.StartGeometry;
            if (_interaction.Kind == RoiInteractionKind.Creating)
            {
                _geometry = CreateGeometryFromMode(
                    _interaction.StartPointer,
                    pointer);

                _interaction = _interaction.WithPreview(_geometry);
            }
            else if (start is not null)
            {
                _geometry = ApplyEdit(start, _interaction, pointer);
                _interaction = _interaction.WithPreview(_geometry);
            }

            return new RoiEditorEvent(
                RoiEditorEventKind.PointerMove,
                _interaction.Kind,
                _interaction.Handle,
                pointer,
                false);
        }
    }

    public RoiEditorEvent PointerUp(Vector2 pointer)
    {
        ValidatePoint(pointer);

        lock (_sync)
        {
            if (!_interaction.IsActive)
            {
                return new RoiEditorEvent(
                    RoiEditorEventKind.PointerUp,
                    RoiInteractionKind.Idle,
                    RoiHandleKind.None,
                    pointer,
                    false);
            }

            if (_geometry is not null)
                _committedGeometry = _geometry;

            var completed = _interaction.Kind;
            var handle = _interaction.Handle;

            _interaction = RoiInteractionState.Idle;
            _hover = _geometry is null
                ? RoiHitResult.None
                : RoiHitTester.HitTest(_geometry, pointer);

            return new RoiEditorEvent(
                RoiEditorEventKind.PointerUp,
                completed,
                handle,
                pointer,
                _geometry is not null);
        }
    }

    public RoiEditorEvent Cancel(Vector2 pointer)
    {
        ValidatePoint(pointer);

        lock (_sync)
        {
            if (!_interaction.IsActive)
            {
                return new RoiEditorEvent(
                    RoiEditorEventKind.Cancelled,
                    RoiInteractionKind.Idle,
                    RoiHandleKind.None,
                    pointer,
                    false);
            }

            _geometry = _committedGeometry;
            _interaction = RoiInteractionState.Idle;
            _hover = _geometry is null
                ? RoiHitResult.None
                : RoiHitTester.HitTest(_geometry, pointer);

            return new RoiEditorEvent(
                RoiEditorEventKind.Cancelled,
                RoiInteractionKind.Idle,
                RoiHandleKind.None,
                pointer,
                false);
        }
    }

    public RoiEditorSnapshot CreateSnapshot()
    {
        lock (_sync)
        {
            return new RoiEditorSnapshot(
                _geometry,
                _committedGeometry,
                _hover,
                _interaction,
                _mode);
        }
    }

    private RoiGeometry CreateGeometryFromMode(
        Vector2 start,
        Vector2 current)
    {
        var left = MathF.Min(start.X, current.X);
        var top = MathF.Min(start.Y, current.Y);
        var right = MathF.Max(start.X, current.X);
        var bottom = MathF.Max(start.Y, current.Y);
        var width = MathF.Max(right - left, 1e-4f);
        var height = MathF.Max(bottom - top, 1e-4f);
        var center = new Vector2(
            (left + right) / 2f,
            (top + bottom) / 2f);
        var size = new Vector2(width, height);

        return _mode == RoiEditorMode.CreateEllipse
            ? RoiGeometry.CreateEllipse(center, size)
            : RoiGeometry.CreateRectangle(center, size);
    }

    private static RoiGeometry ApplyEdit(
        RoiGeometry start,
        RoiInteractionState interaction,
        Vector2 pointer)
    {
        return interaction.Kind switch
        {
            RoiInteractionKind.Moving =>
                start.Translate(pointer - interaction.StartPointer),

            RoiInteractionKind.Rotating =>
                ApplyRotation(start, pointer),

            RoiInteractionKind.Resizing when start.IsPolygon =>
                start.WithVertex(interaction.HandleIndex, pointer),

            RoiInteractionKind.Resizing =>
                ApplyResize(start, interaction.Handle, pointer),

            _ => start
        };
    }

    private static RoiGeometry ApplyRotation(
        RoiGeometry start,
        Vector2 pointer)
    {
        var delta = pointer - start.Center;
        var angle = MathF.Atan2(delta.Y, delta.X) + MathF.PI / 2f;
        return start.WithRotation(angle);
    }

    private static RoiGeometry ApplyResize(
        RoiGeometry start,
        RoiHandleKind handle,
        Vector2 pointer)
    {
        var local = start.ToLocal(pointer);
        var half = start.Size / 2f;
        var direction = RoiHitTester.GetHandleDirection(handle);

        if (direction == Vector2.Zero)
            return start;

        var x = half.X;
        var y = half.Y;

        if (direction.X < 0f)
            x = MathF.Max(MathF.Abs(local.X) + half.X, 1e-4f) / 2f;
        else if (direction.X > 0f)
            x = MathF.Max(local.X + half.X, 1e-4f) / 2f;

        if (direction.Y < 0f)
            y = MathF.Max(MathF.Abs(local.Y) + half.Y, 1e-4f) / 2f;
        else if (direction.Y > 0f)
            y = MathF.Max(local.Y + half.Y, 1e-4f) / 2f;

        var newHalf = new Vector2(x, y);

        if (direction.X < 0f)
            newHalf.X = MathF.Max(MathF.Abs(local.X), 1e-4f) / 2f + half.X / 2f;
        if (direction.X > 0f)
            newHalf.X = MathF.Max(local.X, 1e-4f) / 2f + half.X / 2f;
        if (direction.Y < 0f)
            newHalf.Y = MathF.Max(MathF.Abs(local.Y), 1e-4f) / 2f + half.Y / 2f;
        if (direction.Y > 0f)
            newHalf.Y = MathF.Max(local.Y, 1e-4f) / 2f + half.Y / 2f;

        var fixedLocal = new Vector2(
            direction.X < 0f ? half.X : direction.X > 0f ? -half.X : 0f,
            direction.Y < 0f ? half.Y : direction.Y > 0f ? -half.Y : 0f);

        if (direction.X == 0f)
            fixedLocal.X = 0f;

        if (direction.Y == 0f)
            fixedLocal.Y = 0f;

        var movingLocal = local;
        var centerLocal = (fixedLocal + movingLocal) / 2f;
        var size = new Vector2(
            MathF.Max(MathF.Abs(movingLocal.X - fixedLocal.X), 1e-4f),
            MathF.Max(MathF.Abs(movingLocal.Y - fixedLocal.Y), 1e-4f));

        if (direction.X == 0f)
        {
            centerLocal.X = 0f;
            size.X = start.Size.X;
        }

        if (direction.Y == 0f)
        {
            centerLocal.Y = 0f;
            size.Y = start.Size.Y;
        }

        return RoiGeometry.CreateRectangle(
            start.ToWorld(centerLocal),
            size).WithRotation(start.RotationRadians)
            .WithRotation(start.RotationRadians);
    }

    private static void ValidatePoint(Vector2 point)
    {
        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            throw new ArgumentOutOfRangeException(nameof(point));
    }
}

public readonly record struct RoiEditorSnapshot(
    RoiGeometry? Geometry,
    RoiGeometry? CommittedGeometry,
    RoiHitResult Hover,
    RoiInteractionState Interaction,
    RoiEditorMode Mode);
