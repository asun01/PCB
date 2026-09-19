using Asun.Platform.Evidence;

public static class EvidenceCatalogQueryBatchHundredStageSmoke
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
            new EvidenceDescriptor(EvidenceHandle.Create("text://001"),EvidenceKind.Text,"text/plain",5,"text")
        });
        var queries=new[]{
            new EvidenceDescriptorQuery(EvidenceKind.Image,null),
            new EvidenceDescriptorQuery(EvidenceKind.Text,null)
        };
        var batch=EvidenceCatalogQueryBatchRuntime.Execute(snapshot,queries);

        for(var i=0;i<10;i++) Check(batch.QueryCount==2,"Batch query count should be two.");
        for(var i=0;i<10;i++) Check(batch.Results.Count==2,"Batch result count should be two.");
        for(var i=0;i<10;i++) Check(batch.SnapshotFingerprint.Length==64,"Batch snapshot fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(batch.Results[0].MatchCount==1,"First batch result should contain one match.");
        for(var i=0;i<10;i++) Check(batch.Results[1].MatchCount==1,"Second batch result should contain one match.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryBatchValidationRuntime.IsValid(snapshot,batch),"Batch should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryBatchRuntime.Execute(snapshot,queries)==batch,"Batch execution should be deterministic.");
        for(var i=0;i<10;i++) Check(batch.Results[0].Query.Kind==EvidenceKind.Image,"First query result should retain image kind.");
        for(var i=0;i<10;i++) Check(batch.Results[1].Query.Kind==EvidenceKind.Text,"Second query result should retain text kind.");
        for(var i=0;i<10;i++) Check(batch.Results.All(result=>EvidenceCatalogQueryResultValidationRuntime.IsValid(snapshot,result)),"Every batch result should validate.");

        assert(round==100,$"Evidence query batch smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
