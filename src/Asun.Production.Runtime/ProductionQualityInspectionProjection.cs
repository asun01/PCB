namespace Asun.Production.Runtime;

public sealed record ProductionQualityInspectionProjection(
    Guid ProductionSessionId,
    string ProductionFingerprint,
    Guid QualityRunId,
    IReadOnlyList<ProductionQualityFrameLink> Links,
    string Fingerprint);
