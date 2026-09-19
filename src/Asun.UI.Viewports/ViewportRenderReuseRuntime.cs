namespace Asun.UI.Viewports;

public sealed class ViewportRenderReuseRuntime<TTile>
{
    private readonly object _sync = new();
    private ViewportRenderPipelineFrame<TTile>? _latest;

    public bool TryReuse(
        long generation,
        out ViewportRenderPipelineFrame<TTile> frame)
    {
        lock (_sync)
        {
            if (_latest is not null &&
                !_latest.HasDeferredWork &&
                _latest.Composite.Generation == generation)
            {
                frame = _latest;
                return true;
            }

            frame = default!;
            return false;
        }
    }

    public void Store(ViewportRenderPipelineFrame<TTile> frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        if (frame.HasDeferredWork)
            return;

        lock (_sync)
        {
            if (_latest is not null &&
                frame.Composite.Generation < _latest.Composite.Generation)
                return;

            _latest = frame;
        }
    }

    public void Clear()
    {
        lock (_sync)
            _latest = null;
    }

    public long? LatestGeneration
    {
        get
        {
            lock (_sync)
                return _latest?.Composite.Generation;
        }
    }
}
