namespace Asun.Platform.Evidence;

public static class EvidenceCatalogWindowDiagnosticBundleRuntime
{
    public static EvidenceCatalogWindowDiagnosticBundle Create(
        EvidenceCatalogSnapshotWindow window,
        EvidenceCatalogSnapshotWindowQueryResultSet queryResultSet)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(queryResultSet);

        var summary=EvidenceCatalogSnapshotWindowIntegritySummaryRuntime.Create(window);

        if(!EvidenceCatalogSnapshotWindowQueryValidationRuntime.IsValid(
            window,
            queryResultSet))
        {
            throw new ArgumentException(
                "Evidence snapshot window query result set is invalid.",
                nameof(queryResultSet));
        }

        return new EvidenceCatalogWindowDiagnosticBundle(
            summary.WindowFingerprint,
            summary,
            queryResultSet);
    }
}
