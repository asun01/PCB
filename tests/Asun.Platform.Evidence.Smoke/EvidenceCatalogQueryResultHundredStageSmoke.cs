using Asun.Platform.Evidence;

public static class EvidenceCatalogQueryResultHundredStageSmoke
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
        var result=EvidenceCatalogQueryResultRuntime.Execute(
            snapshot,
            new EvidenceDescriptorQuery(EvidenceKind.Image,"image/raw"));
        var invalid=result with {MatchCount=2};

        for(var i=0;i<10;i++) Check(result.MatchCount==1,"Query result should preserve match count.");
        for(var i=0;i<10;i++) Check(result.Handles.Single().Value=="frame://001","Query result should preserve the matching handle.");
        for(var i=0;i<10;i++) Check(result.SnapshotFingerprint.Length==64,"Query result should bind the snapshot fingerprint.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryResultValidationRuntime.IsValid(snapshot,result),"Query result should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogQueryResultValidationRuntime.IsValid(snapshot,invalid),"Tampered query result count should be rejected.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryResultRuntime.Execute(snapshot,result.Query)==result,"Query result execution should be deterministic.");
        for(var i=0;i<10;i++) Check(result.Query.Kind==EvidenceKind.Image,"Query result should retain query kind.");
        for(var i=0;i<10;i++) Check(result.Query.MediaType=="image/raw","Query result should retain query media type.");
        for(var i=0;i<10;i++) Check(result.Handles.All(handle=>handle.IsValid),"Query result handles should be valid.");
        for(var i=0;i<10;i++) Check(result.MatchCount==result.Handles.Count,"Query result count should equal handle count.");

        assert(round==100,$"Evidence catalog query result smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
