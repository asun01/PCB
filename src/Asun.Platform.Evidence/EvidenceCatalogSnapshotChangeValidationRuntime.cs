namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotChangeValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        IReadOnlyList<EvidenceCatalogSnapshotChange> changes)
    {
        ArgumentNullException.ThrowIfNull(changes);

        var errors=new List<string>();
        var handles=new HashSet<EvidenceHandle>();

        foreach(var change in changes)
        {
            if(!change.Handle.IsValid)
                errors.Add("Snapshot change handles must be valid.");

            if(!handles.Add(change.Handle))
                errors.Add("Snapshot change handles must be unique.");

            if(!EvidenceCatalogSnapshotFingerprintValidationRuntime.IsValidFingerprint(change.PreviousDescriptorFingerprint))
                errors.Add("Previous descriptor fingerprints must be valid.");

            if(!EvidenceCatalogSnapshotFingerprintValidationRuntime.IsValidFingerprint(change.CurrentDescriptorFingerprint))
                errors.Add("Current descriptor fingerprints must be valid.");

            if(change.PreviousDescriptorFingerprint==change.CurrentDescriptorFingerprint)
                errors.Add("A snapshot change must alter the descriptor fingerprint.");
        }

        return errors;
    }

    public static bool IsValid(
        IReadOnlyList<EvidenceCatalogSnapshotChange> changes)=>
        Validate(changes).Count==0;
}
