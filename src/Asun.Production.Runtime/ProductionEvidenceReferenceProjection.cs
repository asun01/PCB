namespace Asun.Production.Runtime;

public sealed record ProductionEvidenceReferenceProjection(
    Guid ProductionSessionId,
    string ProductionFingerprint,
    IReadOnlyList<ProductionEvidenceFrameReference> Frames,
    string Fingerprint);
