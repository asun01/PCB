namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotIntegrityReportValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogSnapshotIntegrityReport report)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(report);

        var errors=new List<string>(
            EvidenceCatalogSnapshotValidationRuntime.Validate(snapshot));

        if(report.DescriptorCount!=snapshot.Count)
            errors.Add("Snapshot integrity report descriptor count must match the snapshot.");

        errors.AddRange(
            EvidenceCatalogSnapshotFingerprintValidationRuntime.Validate(
                snapshot,
                report.SnapshotFingerprint));

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogSnapshotIntegrityReport report)=>
        Validate(snapshot,report).Count==0;
}
