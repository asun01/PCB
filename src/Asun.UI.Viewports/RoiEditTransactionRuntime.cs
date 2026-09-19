namespace Asun.UI.Viewports;

public sealed class RoiEditTransactionRuntime
{
    private readonly object _sync = new();
    private RoiDocumentSnapshot? _before;
    private bool _active;

    public bool IsActive { get { lock (_sync) return _active; } }

    public void Begin(RoiDocumentSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        lock (_sync) { _before = snapshot; _active = true; }
    }

    public RoiDocumentSnapshot? Complete(RoiDocumentSnapshot after)
    {
        ArgumentNullException.ThrowIfNull(after);
        lock (_sync)
        {
            if (!_active) return null;
            _active = false;
            var before = _before;
            _before = null;
            return before;
        }
    }

    public RoiDocumentSnapshot? Cancel()
    {
        lock (_sync)
        {
            if (!_active) return null;
            _active = false;
            var before = _before;
            _before = null;
            return before;
        }
    }
}
