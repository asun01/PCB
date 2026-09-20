namespace Asun.Platform.QualityEvidenceIntegration;

public sealed record QualityEvidenceHandleProjection(
    Guid QualityRunId,
    IReadOnlyList<QualityEvidenceHandleBinding> Bindings,
    string Fingerprint);
