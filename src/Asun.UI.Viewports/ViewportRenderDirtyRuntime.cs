namespace Asun.UI.Viewports;

[Flags]
public enum ViewportDirtyFlags
{
    None = 0,
    Image = 1,
    Transform = 2,
    Roi = 4,
    Overlay = 8,
    Selection = 16,
    All = Image | Transform | Roi | Overlay | Selection
}

public sealed class ViewportRenderDirtyRuntime
{
    private int _flags;

    public ViewportDirtyFlags Flags => (ViewportDirtyFlags)Volatile.Read(ref _flags);

    public bool IsDirty => Flags != ViewportDirtyFlags.None;

    public void Mark(ViewportDirtyFlags flags) => Interlocked.Or(ref _flags, (int)flags);

    public ViewportDirtyFlags Consume() =>
        (ViewportDirtyFlags)Interlocked.Exchange(ref _flags, 0);

    public void Clear() => Interlocked.Exchange(ref _flags, 0);
}
