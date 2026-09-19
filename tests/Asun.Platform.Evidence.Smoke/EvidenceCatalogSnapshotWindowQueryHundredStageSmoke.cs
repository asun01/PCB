using Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowQueryHundredStageSmoke
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
        var resultSet=EvidenceCatalogSnapshotWindowQueryRuntime.Execute(
            window,
            new EvidenceDescriptorQuery(EvidenceKind.Image,null));

        for(var i=0;i<10;i++) Check(resultSet.EntryCount==3,"Window query result set should contain one result per entry.");
        for(var i=0;i<10;i++) Check(resultSet.Results.Count==3,"Window query result count should remain three.");
        for(var i=0;i<10;i++) Check(resultSet.Results[0].Sequence==1,"First query result sequence should be one.");
        for(var i=0;i<10;i++) Check(resultSet.Results[1].Sequence==2,"Second query result sequence should be two.");
        for(var i=0;i<10;i++) Check(resultSet.Results[2].Sequence==3,"Third query result sequence should be three.");
        for(var i=0;i<10;i++) Check(resultSet.Results[0].Result.MatchCount==1,"First image snapshot should return one match.");
        for(var i=0;i<10;i++) Check(resultSet.Results[2].Result.MatchCount==0,"Text snapshot should return zero image matches.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowQueryValidationRuntime.IsValid(window,resultSet),"Window query result set should validate.");
        for(var i=0;i<10;i++) Check(resultSet.WindowFingerprint.Length==64,"Window query result set should bind a fixed-width window fingerprint.");
        for(var i=0;i<10;i++) Check(resultSet.Query.Kind==EvidenceKind.Image,"Window query result set should retain the query kind.");

        assert(round==100,$"Evidence window query smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
