namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotEnvelopeRuntime
{
    public static EvidenceCatalogSnapshotEnvelope Create(
        EvidenceCatalogSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if(!EvidenceCatalogSnapshotValidationRuntime.IsValid(snapshot))
            throw new ArgumentException(
                "Evidence catalog snapshot is invalid.",
                nameof(snapshot));

        return new EvidenceCatalogSnapshotEnvelope(
            snapshot,
            EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot));
    }
}
