namespace Asun.Platform.PcbEvidenceEnvelopeIntegration;

public sealed record PcbProductionEvidenceEnvelope(
    string AssemblyFingerprint,
    Guid ProductionSessionId,
    string ExecutionSnapshotFingerprint,
    string ReplaySnapshotFingerprint,
    string EvidenceResolutionFingerprint,
    string ReleaseProjectionFingerprint,
    string Fingerprint);
