using Asun.Platform.Evidence;

public static class EvidenceCatalogQueryBatchIntegrationHundredStageSmoke
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
            new EvidenceDescriptor(EvidenceHandle.Create("frame://001"),EvidenceKind.Image,"image/raw",10,"one"),
            new EvidenceDescriptor(EvidenceHandle.Create("frame://002"),EvidenceKind.Image,"image/raw",20,"two"),
            new EvidenceDescriptor(EvidenceHandle.Create("text://001"),EvidenceKind.Text,"text/plain",5,"text")
        });
        var queries=new[]{
            new EvidenceDescriptorQuery(EvidenceKind.Image,null),
            new EvidenceDescriptorQuery(null,"image/raw"),
            new EvidenceDescriptorQuery(EvidenceKind.Text,null)
        };
        var batch=EvidenceCatalogQueryBatchRuntime.Execute(snapshot,queries);
        var fingerprint=EvidenceCatalogQueryBatchFingerprintRuntime.CreateFingerprint(batch);

        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryBatchValidationRuntime.IsValid(snapshot,batch),"Integrated batch should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryBatchFingerprintValidationRuntime.IsValid(snapshot,batch,fingerprint),"Integrated batch fingerprint should validate.");
        for(var i=0;i<10;i++) Check(batch.QueryCount==3,"Integrated batch should retain three queries.");
        for(var i=0;i<10;i++) Check(batch.Results[0].MatchCount==2,"Image-kind query should return two matches.");
        for(var i=0;i<10;i++) Check(batch.Results[1].MatchCount==2,"Media-only query should return two matches.");
        for(var i=0;i<10;i++) Check(batch.Results[2].MatchCount==1,"Text-kind query should return one match.");
        for(var i=0;i<10;i++) Check(batch.Results.All(result=>result.SnapshotFingerprint==batch.SnapshotFingerprint),"All batch results should bind one snapshot fingerprint.");
        for(var i=0;i<10;i++) Check(batch.Results.Select(EvidenceCatalogQueryResultFingerprintRuntime.CreateFingerprint).Distinct().Count()==3,"Integrated batch result fingerprints should be unique.");
        for(var i=0;i<10;i++) Check(fingerprint==EvidenceCatalogQueryBatchFingerprintRuntime.CreateFingerprint(batch),"Integrated batch fingerprint should remain deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotFingerprintValidationRuntime.IsValid(snapshot,batch.SnapshotFingerprint),"Integrated batch source snapshot fingerprint should validate.");

        assert(round==100,$"Evidence query batch integration smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
