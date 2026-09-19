using Asun.Platform.Evidence;

public static class EvidenceCatalogWindowDiagnosticBundleHundredStageSmoke
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
        var queryResultSet=EvidenceCatalogSnapshotWindowQueryRuntime.Execute(
            window,
            new EvidenceDescriptorQuery(EvidenceKind.Image,null));
        var bundle=EvidenceCatalogWindowDiagnosticBundleRuntime.Create(
            window,
            queryResultSet);

        for(var i=0;i<10;i++) Check(bundle.WindowFingerprint.Length==64,"Window diagnostic fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(bundle.WindowSummary.EntryCount==2,"Window diagnostic summary should preserve entry count.");
        for(var i=0;i<10;i++) Check(bundle.QueryResultSet.Results.Count==2,"Window diagnostic query result count should match window count.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogWindowDiagnosticBundleValidationRuntime.IsValid(window,bundle),"Window diagnostic bundle should validate.");
        for(var i=0;i<10;i++) Check(bundle.WindowSummary.FirstSequence==1,"Window diagnostic summary should preserve first sequence.");
        for(var i=0;i<10;i++) Check(bundle.WindowSummary.LastSequence==2,"Window diagnostic summary should preserve last sequence.");
        for(var i=0;i<10;i++) Check(bundle.QueryResultSet.Query.Kind==EvidenceKind.Image,"Window diagnostic bundle should retain query kind.");
        for(var i=0;i<10;i++) Check(bundle.QueryResultSet.Results.All(item=>item.Result.MatchCount==1),"Every window entry should contain one image match.");
        for(var i=0;i<10;i++) Check(bundle.WindowFingerprint==bundle.QueryResultSet.WindowFingerprint,"Window diagnostic components should share the same fingerprint.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowIntegritySummaryValidationRuntime.IsValid(window,bundle.WindowSummary),"Window summary should independently validate.");

        assert(round==100,$"Evidence window diagnostic bundle smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
