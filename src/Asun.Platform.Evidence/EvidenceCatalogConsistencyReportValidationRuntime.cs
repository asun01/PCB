namespace Asun.Platform.Evidence;

public static class EvidenceCatalogConsistencyReportValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshot snapshot,
        EvidenceReferenceSet referenceSet,
        EvidenceReferenceResolution resolution,
        EvidenceCatalogConsistencyReport report)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(referenceSet);
        ArgumentNullException.ThrowIfNull(resolution);
        ArgumentNullException.ThrowIfNull(report);

        var errors=new List<string>();

        errors.AddRange(
            EvidenceCatalogSnapshotValidationRuntime.Validate(snapshot));

        errors.AddRange(
            EvidenceReferenceResolutionValidationRuntime.Validate(
                snapshot,
                referenceSet,
                resolution));

        var statistics=EvidenceCatalogStatisticsRuntime.Create(snapshot);
        var snapshotFingerprint=EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot);
        var statisticsFingerprint=EvidenceCatalogStatisticsFingerprintRuntime.CreateFingerprint(statistics);
        var referenceFingerprint=EvidenceReferenceResolutionFingerprintRuntime.CreateFingerprint(resolution);

        if(report.SnapshotFingerprint!=snapshotFingerprint)
            errors.Add("Consistency report snapshot fingerprint does not match.");

        if(report.StatisticsFingerprint!=statisticsFingerprint)
            errors.Add("Consistency report statistics fingerprint does not match.");

        if(report.ReferenceResolutionFingerprint!=referenceFingerprint)
            errors.Add("Consistency report reference fingerprint does not match.");

        if(report.DescriptorCount!=snapshot.Count)
            errors.Add("Consistency report descriptor count must match the snapshot.");

        if(report.ReferenceCount!=referenceSet.Handles.Count)
            errors.Add("Consistency report reference count must match the reference set.");

        if(report.MissingReferenceCount!=resolution.MissingHandles.Count)
            errors.Add("Consistency report missing-reference count must match the resolution.");

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshot snapshot,
        EvidenceReferenceSet referenceSet,
        EvidenceReferenceResolution resolution,
        EvidenceCatalogConsistencyReport report)=>
        Validate(snapshot,referenceSet,resolution,report).Count==0;
}
