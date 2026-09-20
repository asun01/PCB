namespace Asun.Platform.PcbEvidenceReleaseIntegration;

public sealed record PcbEvidenceReleaseFactProjection(
    string ExecutionSnapshotFingerprint,
    string EvidenceProjectionFingerprint,
    string CatalogSnapshotFingerprint,
    string ReleaseManifestFingerprint,
    int RequestedCount,
    int FoundCount,
    int MissingCount,
    bool AllRequestedResolved,
    string Fingerprint);
