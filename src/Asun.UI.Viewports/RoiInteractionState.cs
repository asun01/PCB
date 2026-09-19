using System.Numerics;

namespace Asun.UI.Viewports;

public enum RoiEditorMode
{
    Select,
    CreateRectangle,
    CreateEllipse
}

public enum RoiInteractionKind
{
    Idle,
    Creating,
    Moving,
    Resizing,
    Rotating
}

public enum RoiEditorEventKind
{
    PointerDown,
    PointerMove,
    PointerUp,
    Cancelled
}

public readonly record struct RoiEditorEvent(
    RoiEditorEventKind Kind,
    RoiInteractionKind Interaction,
    RoiHandleKind Handle,
    Vector2 Pointer,
    bool HasCommittedGeometry);

public sealed record RoiInteractionState
{
    private RoiInteractionState(
        RoiInteractionKind kind,
        RoiHandleKind handle,
        int handleIndex,
        Vector2 startPointer,
        RoiGeometry? startGeometry,
        RoiGeometry? previewGeometry)
    {
        Kind = kind;
        Handle = handle;
        HandleIndex = handleIndex;
        StartPointer = startPointer;
        StartGeometry = startGeometry;
        PreviewGeometry = previewGeometry;
    }

    public static RoiInteractionState Idle { get; } =
        new(
            RoiInteractionKind.Idle,
            RoiHandleKind.None,
            -1,
            default,
            null,
            null);

    public RoiInteractionKind Kind { get; }

    public RoiHandleKind Handle { get; }

    public int HandleIndex { get; }

    public Vector2 StartPointer { get; }

    public RoiGeometry? StartGeometry { get; }

    public RoiGeometry? PreviewGeometry { get; }

    public bool IsActive => Kind != RoiInteractionKind.Idle;

    public static RoiInteractionState Create(
        RoiInteractionKind kind,
        RoiHandleKind handle,
        int handleIndex,
        Vector2 startPointer,
        RoiGeometry? startGeometry,
        RoiGeometry? previewGeometry)
    {
        if (!float.IsFinite(startPointer.X) || !float.IsFinite(startPointer.Y))
            throw new ArgumentOutOfRangeException(nameof(startPointer));

        if (kind == RoiInteractionKind.Idle)
            return Idle;

        return new RoiInteractionState(
            kind,
            handle,
            handleIndex,
            startPointer,
            startGeometry,
            previewGeometry);
    }

    public RoiInteractionState WithPreview(RoiGeometry? previewGeometry) =>
        new(
            Kind,
            Handle,
            HandleIndex,
            StartPointer,
            StartGeometry,
            previewGeometry);

    public RoiInteractionState Reset() => Idle;
}
