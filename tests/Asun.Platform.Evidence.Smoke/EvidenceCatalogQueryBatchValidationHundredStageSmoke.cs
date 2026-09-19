using Asun.Platform.Evidence;

public static class EvidenceCatalogQueryBatchValidationHundredStageSmoke
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
        var batch=EvidenceCatalogQueryBatchRuntime.Execute(snapshot,new[]{
            new EvidenceDescriptorQuery(EvidenceKind.Image,null),
            new EvidenceDescriptorQuery(EvidenceKind.Text,null)
        });
        var invalidCount=batch with {QueryCount=1};
        var invalidSnapshot=batch with {SnapshotFingerprint=new string('a',64)};

        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryBatchValidationRuntime.IsValid(snapshot,batch),"Valid batch should pass validation.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogQueryBatchValidationRuntime.IsValid(snapshot,invalidCount),"Mismatched query count should be rejected.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogQueryBatchValidationRuntime.IsValid(snapshot,invalidSnapshot),"Mismatched snapshot fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(batch.Results.All(result=>result.Handles.Count==1),"Each test batch query should have one match.");
        for(var i=0;i<10;i++) Check(batch.Results.Select(result=>result.Query.Kind).Distinct().Count()==2,"Batch should retain distinct query kinds.");
        for(var i=0;i<10;i++) Check(batch.Results.All(result=>result.SnapshotFingerprint==batch.SnapshotFingerprint),"Every result should bind the batch snapshot fingerprint.");
        for(var i=0;i<10;i++) Check(batch.Results.Select(EvidenceCatalogQueryResultFingerprintRuntime.CreateFingerprint).Distinct().Count()==2,"Batch result fingerprints should be unique.");
        for(var i=0;i<10;i++) Check(batch.QueryCount==batch.Results.Count,"Batch count should remain coherent.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotFingerprintValidationRuntime.IsValid(snapshot,batch.SnapshotFingerprint),"Batch snapshot fingerprint should validate independently.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryBatchValidationRuntime.Validate(snapshot,batch).Count==0,"Valid batch should produce no validation errors.");

        assert(round==100,$"Evidence query batch validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
