namespace Asun.UI.Viewports;

public sealed class ViewportRenderEvidenceStore
{
    private readonly object _sync = new();
    private readonly int _capacity;
    private readonly List<ViewportRenderEvidenceManifest> _entries = new();

    public ViewportRenderEvidenceStore(int capacity = 256)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _capacity = capacity;
    }

    public int Capacity => _capacity;

    public int Count
    {
        get
        {
            lock (_sync)
                return _entries.Count;
        }
    }

    public ViewportRenderEvidenceManifest? Latest
    {
        get
        {
            lock (_sync)
                return _entries.Count == 0
                    ? null
                    : _entries[^1];
        }
    }

    public void Add(ViewportRenderEvidenceManifest manifest)
    {
        lock (_sync)
        {
            if (_entries.Count != 0)
            {
                var last = _entries[^1];

                if (manifest.Generation < last.Generation ||
                    (manifest.Generation == last.Generation &&
                     manifest.SubmissionSequence < last.SubmissionSequence))
                {
                    throw new InvalidOperationException(
                        "Evidence manifests must be monotonic by generation and submission sequence.");
                }
            }

            if (_entries.Count >= _capacity)
                _entries.RemoveAt(0);

            _entries.Add(manifest);
        }
    }

    public bool TryGetByStableKey(
        string stableKey,
        out ViewportRenderEvidenceManifest manifest)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stableKey);

        lock (_sync)
        {
            foreach (var item in _entries)
            {
                if (string.Equals(
                    item.StableKey,
                    stableKey,
                    StringComparison.Ordinal))
                {
                    manifest = item;
                    return true;
                }
            }

            manifest = default;
            return false;
        }
    }

    public IReadOnlyList<ViewportRenderEvidenceManifest> Snapshot()
    {
        lock (_sync)
            return _entries.ToArray();
    }

    public IReadOnlyList<string> Validate()
    {
        lock (_sync)
        {
            var errors = new List<string>();
            long previousGeneration = -1;
            long previousSequence = -1;

            foreach (var manifest in _entries)
            {
                if (manifest.Generation < previousGeneration ||
                    (manifest.Generation == previousGeneration &&
                     manifest.SubmissionSequence < previousSequence))
                {
                    errors.Add(
                        "Evidence history must remain monotonic.");
                }

                errors.AddRange(
                    ViewportRenderDiagnosticsRuntime.ValidateManifest(
                        manifest));

                previousGeneration = manifest.Generation;
                previousSequence = manifest.SubmissionSequence;
            }

            return errors;
        }
    }

    public void Clear()
    {
        lock (_sync)
            _entries.Clear();
    }
}
