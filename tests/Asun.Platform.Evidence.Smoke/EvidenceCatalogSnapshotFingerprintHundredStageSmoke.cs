using Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotFingerprintHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var snapshot=EvidenceCatalogSnapshotRuntime.Create(new[]{
            new EvidenceDescriptor(EvidenceHandle.Create("frame://002"),EvidenceKind.Text,"text/plain",11,"second"),
            new EvidenceDescriptor(EvidenceHandle.Create("frame://001"),EvidenceKind.Image,"image/raw",10,"first")
        });
        var fingerprint=EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot);
        var changed=EvidenceCatalogSnapshotRuntime.Create(new[]{
            new EvidenceDescriptor(EvidenceHandle.Create("frame://002"),EvidenceKind.Text,"text/plain",12,"second"),
            new EvidenceDescriptor(EvidenceHandle.Create("frame://001"),EvidenceKind.Image,"image/raw",10,"first")
        });
        var invalid=new string('a',64);

        for(var i=0;i<10;i++) Check(fingerprint.Length==64,"Snapshot fingerprint should be SHA-256 sized.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotFingerprintValidationRuntime.IsValidFingerprint(fingerprint),"Snapshot fingerprint syntax should be valid.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot)==fingerprint,"Snapshot fingerprint should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotFingerprintValidationRuntime.IsValid(snapshot,fingerprint),"Snapshot fingerprint should validate the source snapshot.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogSnapshotFingerprintValidationRuntime.IsValid(changed,fingerprint),"Snapshot mutation should invalidate the original fingerprint.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotFingerprintValidationRuntime.IsValidFingerprint(invalid),"A valid-length lowercase fingerprint should pass syntax validation.");
        for(var i=0;i<10;i++) Check(snapshot.Descriptors.SequenceEqual(snapshot.Descriptors.OrderBy(descriptor=>descriptor.Handle.Value,StringComparer.Ordinal)),"Snapshot descriptor ordering should be canonical.");
        for(var i=0;i<10;i++) Check(fingerprint.All(character=>char.ToLowerInvariant(character)==character),"Snapshot fingerprint should remain lowercase.");
        for(var i=0;i<10;i++) Check(snapshot.Count==2,"Snapshot count should remain stable.");
        for(var i=0;i<10;i++) Check(snapshot.Descriptors[0].Handle.Value=="frame://001","Canonical ordering should place frame://001 first.");

        assert(round==100,$"Evidence snapshot fingerprint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
