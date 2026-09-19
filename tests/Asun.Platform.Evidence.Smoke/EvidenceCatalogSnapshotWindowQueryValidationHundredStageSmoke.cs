using Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowQueryValidationHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        static EvidenceCatalogSnapshotEnvelope Envelope(string handle)=>
            EvidenceCatalogSnapshotEnvelopeRuntime.Create(
                EvidenceCatalogSnapshotRuntime.Create(new[]{
                    new EvidenceDescriptor(EvidenceHandle.Create(handle),EvidenceKind.Image,"image/raw",10,handle)
                }));

        var window=EvidenceCatalogSnapshotWindowRuntime.Create(new[]{
            new EvidenceCatalogSnapshotWindowEntry(1,Envelope("frame://001")),
            new EvidenceCatalogSnapshotWindowEntry(2,Envelope("frame://002"))
        });
        var query=new EvidenceDescriptorQuery(EvidenceKind.Image,null);
        var resultSet=EvidenceCatalogSnapshotWindowQueryRuntime.Execute(window,query);
        var invalid=resultSet with {EntryCount=1};

        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowQueryValidationRuntime.IsValid(window,resultSet),"Valid window query result set should pass.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogSnapshotWindowQueryValidationRuntime.IsValid(window,invalid),"Mismatched entry count should be rejected.");
        for(var i=0;i<10;i++) Check(resultSet.Results.All(item=>item.Result.Query==query),"Every result should retain the common query.");
        for(var i=0;i<10;i++) Check(resultSet.Results.Select(item=>item.Sequence).SequenceEqual(new long[]{1,2}),"Result sequences should match the window order.");
        for(var i=0;i<10;i++) Check(resultSet.Results.All(item=>item.Result.Handles.Count==1),"Each image snapshot should return one image handle.");
        for(var i=0;i<10;i++) Check(resultSet.Results.All(item=>item.Result.SnapshotFingerprint.Length==64),"Each result should bind a snapshot fingerprint.");
        for(var i=0;i<10;i++) Check(resultSet.EntryCount==window.Count,"Result-set count should equal window count.");
        for(var i=0;i<10;i++) Check(resultSet.Results.Select(item=>item.Sequence).Distinct().Count()==2,"Result sequences should be unique.");
        for(var i=0;i<10;i++) Check(resultSet.WindowFingerprint==EvidenceCatalogSnapshotWindowIntegritySummaryRuntime.Create(window).WindowFingerprint,"Result set should bind the source window fingerprint.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogDescriptorQueryValidationShim(resultSet.Query),"The common query should remain structurally valid.");

        assert(round==100,$"Evidence window query validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private static bool EvidenceCatalogDescriptorQueryValidationShim(
        EvidenceDescriptorQuery query)=>
        EvidenceDescriptorQueryValidationRuntime.IsValid(query);
}
