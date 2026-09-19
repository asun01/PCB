using System.Text.Json;

namespace Asun.UI.Viewports;

public readonly record struct ViewportReplaySessionManifest(
    string SessionId,
    DateTimeOffset CreatedAtUtc,
    int InputEventCount,
    int EvidenceManifestCount,
    int AuditEventCount,
    string InputHash,
    string EvidenceHash,
    string AuditHash,
    string SessionHash)
{
    public bool IsEmpty =>
        InputEventCount == 0 &&
        EvidenceManifestCount == 0 &&
        AuditEventCount == 0;
}

public sealed class ViewportReplaySessionRuntime
{
    private readonly object _sync = new();
    private readonly List<ViewportInputEvent> _inputs = new();
    private readonly List<ViewportRenderEvidenceManifest> _evidence = new();
    private readonly List<ViewportPresentationAuditEvent> _audit = new();
    private readonly string _sessionId;
    private readonly DateTimeOffset _createdAtUtc;

    public ViewportReplaySessionRuntime(
        string? sessionId = null,
        DateTimeOffset? createdAtUtc = null)
    {
        _sessionId =
            string.IsNullOrWhiteSpace(sessionId)
                ? Guid.NewGuid().ToString("N")
                : sessionId;

        _createdAtUtc =
            createdAtUtc ??
            DateTimeOffset.UtcNow;
    }

    public string SessionId => _sessionId;

    public DateTimeOffset CreatedAtUtc => _createdAtUtc;

    public void RecordInput(ViewportInputEvent input)
    {
        lock (_sync)
            _inputs.Add(input);
    }

    public void RecordEvidence(
        ViewportRenderEvidenceManifest manifest)
    {
        lock (_sync)
            _evidence.Add(manifest);
    }

    public void RecordAudit(
        ViewportPresentationAuditEvent audit)
    {
        lock (_sync)
            _audit.Add(audit);
    }

    public ViewportReplaySessionManifest CreateManifest()
    {
        lock (_sync)
        {
            return ViewportReplaySessionBundleRuntime.CreateManifest(
                _sessionId,
                _createdAtUtc,
                _inputs,
                _evidence,
                _audit);
        }
    }

    public ViewportReplaySessionBundle SnapshotBundle()
    {
        lock (_sync)
        {
            return ViewportReplaySessionBundleRuntime.Capture(
                CreateManifest(),
                _inputs,
                _evidence,
                _audit);
        }
    }

    public IReadOnlyList<string> Validate()
    {
        lock (_sync)
            return ViewportReplaySessionBundleRuntime.Validate(
                SnapshotBundle());
    }

    public string ToJson()
    {
        lock (_sync)
        {
            return JsonSerializer.Serialize(
                SnapshotBundle(),
                new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }
    }

    public void Reset()
    {
        lock (_sync)
        {
            _inputs.Clear();
            _evidence.Clear();
            _audit.Clear();
        }
    }
}
