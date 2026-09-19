namespace Asun.Platform.Evidence;

public static class EvidenceCatalogQueryResultRuntime
{
    public static EvidenceCatalogQueryResult Execute(
        EvidenceCatalogSnapshot snapshot,
        EvidenceDescriptorQuery query)
    {
        var matches=EvidenceCatalogQueryRuntime.Find(snapshot,query);

        return new EvidenceCatalogQueryResult(
            query,
            EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot),
            matches.Select(descriptor=>descriptor.Handle).ToArray(),
            matches.Count);
    }
}
