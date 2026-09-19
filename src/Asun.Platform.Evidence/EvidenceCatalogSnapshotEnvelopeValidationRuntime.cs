namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotEnvelopeValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshotEnvelope envelope)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        return EvidenceCatalogSnapshotFingerprintValidationRuntime.Validate(
            envelope.Snapshot,
            envelope.Fingerprint);
    }

    public static bool IsValid(
        EvidenceCatalogSnapshotEnvelope envelope)=>
        Validate(envelope).Count==0;
}
