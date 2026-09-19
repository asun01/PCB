using System.Text;
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
            var inputHash = Hash(
                string.Join(
                    "\n",
                    _inputs.Select(item =>
                        $"{item.Sequence}|{item.Kind}|{item.Position.X:R}|{item.Position.Y:R}|{item.WheelDelta}|{item.Button}")));

            var evidenceHash = Hash(
                string.Join(
                    "\n",
                    _evidence.Select(item =>
                        $"{item.Generation}|{item.SubmissionSequence}|{item.FrameHash}|{item.BatchHash}|{item.CommandHash}|{item.ReplayHash}")));

            var auditHash = Hash(
                string.Join(
                    "\n",
                    _audit.Select(item =>
                        $"{item.Sequence}|{item.Stage}|{item.Generation}|{item.SubmissionSequence}|{item.DeliveryStatus}|{item.RenderedUnits}|{item.DeferredUnits}|{item.EvidenceKey}")));

            var sessionHash = Hash(
                $"{_sessionId}|{_createdAtUtc:O}|{inputHash}|{evidenceHash}|{auditHash}");

            return new ViewportReplaySessionManifest(
                _sessionId,
                _createdAtUtc,
                _inputs.Count,
                _evidence.Count,
                _audit.Count,
                inputHash,
                evidenceHash,
                auditHash,
                sessionHash);
        }
    }

    public string ToJson()
    {
        lock (_sync)
        {
            var payload = new
            {
                Manifest = CreateManifest(),
                Inputs = _inputs.ToArray(),
                Evidence = _evidence.ToArray(),
                Audit = _audit.ToArray()
            };

            return JsonSerializer.Serialize(
                payload,
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

    private static string Hash(string value) =>
        Convert.ToHexString(
            System.Security.Cryptography.SHA256.HashData(
                Encoding.UTF8.GetBytes(value)));
}
