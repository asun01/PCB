namespace Asun.Platform.Evidence;

public sealed record EvidenceCatalogWindowDiagnosticBundle(
    string WindowFingerprint,
    EvidenceCatalogSnapshotWindowIntegritySummary WindowSummary,
    EvidenceCatalogSnapshotWindowQueryResultSet QueryResultSet);
