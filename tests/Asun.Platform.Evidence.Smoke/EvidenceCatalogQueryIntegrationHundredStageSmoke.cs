using Asun.Platform.Evidence;

public static class EvidenceCatalogQueryIntegrationHundredStageSmoke
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
            new EvidenceDescriptor(EvidenceHandle.Create("frame://002"),EvidenceKind.Image,"image/raw",20,"two"),
            new EvidenceDescriptor(EvidenceHandle.Create("frame://001"),EvidenceKind.Image,"image/raw",10,"one"),
            new EvidenceDescriptor(EvidenceHandle.Create("text://001"),EvidenceKind.Text,"text/plain",5,"text")
        });
        var query=new EvidenceDescriptorQuery(EvidenceKind.Image,"image/raw");
        var result=EvidenceCatalogQueryResultRuntime.Execute(snapshot,query);
        var fingerprint=EvidenceCatalogQueryResultFingerprintRuntime.CreateFingerprint(result);

        for(var i=0;i<10;i++) Check(EvidenceDescriptorQueryValidationRuntime.IsValid(query),"Integrated query should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotValidationRuntime.IsValid(snapshot),"Integrated snapshot should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryResultValidationRuntime.IsValid(snapshot,result),"Integrated query result should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryResultFingerprintValidationRuntime.IsValid(snapshot,result,fingerprint),"Integrated query fingerprint should validate.");
        for(var i=0;i<10;i++) Check(result.MatchCount==2,"Integrated query should return two image descriptors.");
        for(var i=0;i<10;i++) Check(result.Handles.SequenceEqual(new[]{EvidenceHandle.Create("frame://001"),EvidenceHandle.Create("frame://002")}),"Integrated query handles should remain canonical.");
        for(var i=0;i<10;i++) Check(result.SnapshotFingerprint==EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot),"Integrated result should bind the snapshot fingerprint.");
        for(var i=0;i<10;i++) Check(fingerprint==EvidenceCatalogQueryResultFingerprintRuntime.CreateFingerprint(result),"Integrated query fingerprint should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryRuntime.Find(snapshot,query).Count==2,"Integrated query runtime should agree with the result.");
        for(var i=0;i<10;i++) Check(result.Handles.All(handle=>handle.IsValid),"Integrated query handles should remain valid.");

        assert(round==100,$"Evidence catalog query integration smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
