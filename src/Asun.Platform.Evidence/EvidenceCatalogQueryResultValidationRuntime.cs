namespace Asun.Platform.Evidence;

public static class EvidenceCatalogQueryResultValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogQueryResult result)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(result);

        var errors=new List<string>(
            EvidenceDescriptorQueryValidationRuntime.Validate(result.Query));

        errors.AddRange(
            EvidenceCatalogSnapshotValidationRuntime.Validate(snapshot));

        var expectedFingerprint=
            EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot);

        if(result.SnapshotFingerprint!=expectedFingerprint)
            errors.Add("Evidence catalog query result snapshot fingerprint does not match the source snapshot.");

        if(result.MatchCount!=result.Handles.Count)
            errors.Add("Evidence catalog query result match count must equal the handle count.");

        if(result.Handles.Any(handle=>!handle.IsValid))
            errors.Add("Evidence catalog query result handles must be valid.");

        if(result.Handles.Count!=result.Handles.Distinct().Count())
            errors.Add("Evidence catalog query result handles must be unique.");

        var expected=EvidenceCatalogQueryRuntime.Find(snapshot,result.Query)
            .Select(descriptor=>descriptor.Handle)
            .ToArray();

        if(!expected.SequenceEqual(result.Handles))
            errors.Add("Evidence catalog query result handles do not match the source query.");

        return errors;
    }

    public static bool IsValid(
        EvidenceCatalogSnapshot snapshot,
        EvidenceCatalogQueryResult result)=>
        Validate(snapshot,result).Count==0;
}
