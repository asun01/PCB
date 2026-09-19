using Asun.Platform.Evidence;

public static class EvidenceCatalogQueryBatchFingerprintHundredStageSmoke
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
            new EvidenceDescriptor(EvidenceHandle.Create("frame://001"),EvidenceKind.Image,"image/raw",10,"one")
        });
        var batch=EvidenceCatalogQueryBatchRuntime.Execute(
            snapshot,
            new[]{new EvidenceDescriptorQuery(EvidenceKind.Image,null)});
        var fingerprint=EvidenceCatalogQueryBatchFingerprintRuntime.CreateFingerprint(batch);
        var invalid=batch with {QueryCount=2};

        for(var i=0;i<10;i++) Check(fingerprint.Length==64,"Batch fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(fingerprint.All(Uri.IsHexDigit),"Batch fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryBatchFingerprintRuntime.CreateFingerprint(batch)==fingerprint,"Batch fingerprint should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryBatchFingerprintValidationRuntime.IsValid(snapshot,batch,fingerprint),"Batch fingerprint should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogQueryBatchFingerprintValidationRuntime.IsValid(snapshot,invalid,fingerprint),"Batch mutation should invalidate the original fingerprint.");
        for(var i=0;i<10;i++) Check(fingerprint.All(character=>char.ToLowerInvariant(character)==character),"Batch fingerprint should remain lowercase.");
        for(var i=0;i<10;i++) Check(batch.Results.Count==1,"Batch should retain one query result.");
        for(var i=0;i<10;i++) Check(batch.Results[0].MatchCount==1,"Batch query result should retain one match.");
        for(var i=0;i<10;i++) Check(batch.SnapshotFingerprint.Length==64,"Batch source fingerprint should remain fixed width.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryBatchValidationRuntime.IsValid(snapshot,batch),"Fingerprint source batch should remain valid.");

        assert(round==100,$"Evidence query batch fingerprint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
