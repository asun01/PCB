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

    public IReadOnlyList<string> Validate()
    {
        lock (_sync)
        {
            var errors = new List<string>();
            long previousInputSequence = 0;
            long previousEvidenceGeneration = -1;
            long previousEvidenceSequence = -1;
            long previousAuditSequence = 0;

            foreach (var input in _inputs)
            {
                if (input.Sequence <= previousInputSequence)
                    errors.Add("Input event sequence must increase strictly.");

                previousInputSequence = input.Sequence;
            }

            foreach (var evidence in _evidence)
            {
                if (evidence.Generation < previousEvidenceGeneration ||
                    (evidence.Generation == previousEvidenceGeneration &&
                     evidence.SubmissionSequence < previousEvidenceSequence))
                {
                    errors.Add("Evidence sequence must remain monotonic.");
                }

                errors.AddRange(
                    ViewportRenderDiagnosticsRuntime.ValidateManifest(
                        evidence));

                previousEvidenceGeneration = evidence.Generation;
                previousEvidenceSequence = evidence.SubmissionSequence;
            }

            foreach (var audit in _audit)
            {
                if (audit.Sequence <= previousAuditSequence)
                    errors.Add("Audit event sequence must increase strictly.");

                if (audit.Generation < 0 ||
                    audit.SubmissionSequence < 0 ||
                    audit.RenderedUnits < 0 ||
                    audit.DeferredUnits < 0)
                {
                    errors.Add("Audit event contains invalid counters.");
                }

                previousAuditSequence = audit.Sequence;
            }

            var manifest = CreateManifest();

            if (manifest.InputEventCount != _inputs.Count ||
                manifest.EvidenceManifestCount != _evidence.Count ||
                manifest.AuditEventCount != _audit.Count)
            {
                errors.Add("Replay session manifest counters do not match retained evidence.");
            }

            if (!IsSha256(manifest.InputHash) ||
                !IsSha256(manifest.EvidenceHash) ||
                !IsSha256(manifest.AuditHash) ||
                !IsSha256(manifest.SessionHash))
            {
                errors.Add("Replay session manifest hashes must be SHA-256 values.");
            }

            return errors;
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

    private static bool IsSha256(string value) =>
        value.Length == 64 &&
        value.All(character =>
            (character >= '0' && character <= '9') ||
            (character >= 'A' && character <= 'F'));

    private static string Hash(string value) =>
        Convert.ToHexString(
            System.Security.Cryptography.SHA256.HashData(
                Encoding.UTF8.GetBytes(value)));
}
