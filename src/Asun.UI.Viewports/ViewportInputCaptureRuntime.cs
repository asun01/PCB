namespace Asun.UI.Viewports;

public enum ViewportInputOwner
{
    None,
    Pan,
    Roi,
    Marquee,
    Overlay
}

public sealed class ViewportInputCaptureRuntime
{
    private readonly object _sync = new();
    private ViewportInputOwner _owner;

    public ViewportInputOwner Owner { get { lock (_sync) return _owner; } }

    public bool TryCapture(ViewportInputOwner owner)
    {
        if (owner == ViewportInputOwner.None) throw new ArgumentOutOfRangeException(nameof(owner));
        lock (_sync)
        {
            if (_owner != ViewportInputOwner.None) return false;
            _owner = owner;
            return true;
        }
    }

    public bool Release(ViewportInputOwner owner)
    {
        lock (_sync)
        {
            if (_owner != owner) return false;
            _owner = ViewportInputOwner.None;
            return true;
        }
    }

    public void ForceRelease() { lock (_sync) _owner = ViewportInputOwner.None; }
}
