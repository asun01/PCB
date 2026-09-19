using Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowQueryIntegrationHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        static EvidenceCatalogSnapshotEnvelope Envelope(string handle,EvidenceKind kind)=>
            EvidenceCatalogSnapshotEnvelopeRuntime.Create(
                EvidenceCatalogSnapshotRuntime.Create(new[]{
                    new EvidenceDescriptor(EvidenceHandle.Create(handle),kind,kind==EvidenceKind.Image ? "image/raw" : "text/plain",10,handle)
                }));

        var window=EvidenceCatalogSnapshotWindowRuntime.Create(new[]{
            new EvidenceCatalogSnapshotWindowEntry(1,Envelope("frame://001",EvidenceKind.Image)),
            new EvidenceCatalogSnapshotWindowEntry(2,Envelope("frame://002",EvidenceKind.Image)),
            new EvidenceCatalogSnapshotWindowEntry(3,Envelope("text://001",EvidenceKind.Text))
        });
        var query=new EvidenceDescriptorQuery(EvidenceKind.Image,null);
        var resultSet=EvidenceCatalogSnapshotWindowQueryRuntime.Execute(window,query);
        var fingerprint=EvidenceCatalogSnapshotWindowQueryFingerprintRuntime.CreateFingerprint(resultSet);

        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowValidationRuntime.IsValid(window),"Integrated window should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowQueryValidationRuntime.IsValid(window,resultSet),"Integrated window query should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowQueryFingerprintValidationRuntime.IsValid(window,resultSet,fingerprint),"Integrated window query fingerprint should validate.");
        for(var i=0;i<10;i++) Check(resultSet.EntryCount==3,"Integrated result set should preserve window cardinality.");
        for(var i=0;i<10;i++) Check(resultSet.Results[0].Result.MatchCount==1,"Integrated first snapshot should match one image.");
        for(var i=0;i<10;i++) Check(resultSet.Results[1].Result.MatchCount==1,"Integrated second snapshot should match one image.");
        for(var i=0;i<10;i++) Check(resultSet.Results[2].Result.MatchCount==0,"Integrated text snapshot should match zero images.");
        for(var i=0;i<10;i++) Check(resultSet.Results.All(item=>item.Result.Query==query),"Integrated results should retain one query.");
        for(var i=0;i<10;i++) Check(fingerprint==EvidenceCatalogSnapshotWindowQueryFingerprintRuntime.CreateFingerprint(resultSet),"Integrated fingerprint should be deterministic.");
        for(var i=0;i<10;i++) Check(resultSet.WindowFingerprint==EvidenceCatalogSnapshotWindowIntegritySummaryRuntime.Create(window).WindowFingerprint,"Integrated result set should bind the window fingerprint.");

        assert(round==100,$"Evidence window query integration smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
