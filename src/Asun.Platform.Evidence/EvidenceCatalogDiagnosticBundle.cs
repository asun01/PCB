namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogDiagnosticBundle(
    string SnapshotFingerprint,
    EvidenceCatalogStatistics Statistics,
    EvidenceCatalogQueryBatch QueryBatch,
    EvidenceReferenceResolution ReferenceResolution);
