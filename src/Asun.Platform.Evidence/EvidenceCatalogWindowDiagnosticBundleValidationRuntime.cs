namespace Asun.Platform.Evidence;

public static class EvidenceCatalogWindowDiagnosticBundleValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshotWindow window,
        EvidenceCatalogWindowDiagnosticBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(bundle);

        var errors=new List<string>();
        var expectedSummary=
            EvidenceCatalogSnapshotWindowIntegritySummaryRuntime.Create(window);

        errors.AddRange(
            EvidenceCatalogSnapshotWindowIntegritySummaryValidationRuntime.Validate(
                window,
                bundle.WindowSummary));

        errors.AddRange(
            EvidenceCatalogSnapshotWindowQueryValidationRuntime.Validate(
                window,
                bundle.QueryResultSet));

        if(bundle.WindowFingerprint!=expectedSummary.WindowFingerprint)
            errors.Add("Window diagnostic bundle fingerprint must match the source window.");

        if(bundle.WindowSummary!=expectedSummary)
            errors.Add("Window diagnostic bundle summary must match the source window.");

        if(bundle.QueryResultSet.WindowFingerprint!=bundle.WindowFingerprint)
            errors.Add("Window query result set must bind the diagnostic bundle window fingerprint.");

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshotWindow window,
        EvidenceCatalogWindowDiagnosticBundle bundle)=>
        Validate(window,bundle).Count==0;
}
