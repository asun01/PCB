using Asun.Platform.Evidence;

namespace Asun.Platform.PcbEvidenceResolutionIntegration;

public sealed record PcbExecutionEvidenceResolution(
    string ExecutionSnapshotFingerprint,
    string EvidenceProjectionFingerprint,
    string CatalogSnapshotFingerprint,
    IReadOnlyList<EvidenceHandle> RequestedHandles,
    IReadOnlyList<EvidenceHandle> FoundHandles,
    IReadOnlyList<EvidenceHandle> MissingHandles,
    string Fingerprint);
