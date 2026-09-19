using Asun.Platform.Evidence;

public static class EvidenceCatalogWindowDiagnosticBundleValidationHundredStageSmoke
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
            new EvidenceCatalogSnapshotWindowEntry(1,Envelope("frame://001"))
        });
        var queryResultSet=EvidenceCatalogSnapshotWindowQueryRuntime.Execute(
            window,
            new EvidenceDescriptorQuery(EvidenceKind.Image,null));
        var bundle=EvidenceCatalogWindowDiagnosticBundleRuntime.Create(window,queryResultSet);
        var invalid=bundle with {WindowFingerprint=new string('a',64)};

        for(var i=0;i<10;i++) Check(EvidenceCatalogWindowDiagnosticBundleValidationRuntime.IsValid(window,bundle),"Valid window diagnostic bundle should pass.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogWindowDiagnosticBundleValidationRuntime.IsValid(window,invalid),"Tampered window fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowQueryValidationRuntime.IsValid(window,bundle.QueryResultSet),"Bundle query result set should remain valid.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowIntegritySummaryValidationRuntime.IsValid(window,bundle.WindowSummary),"Bundle window summary should remain valid.");
        for(var i=0;i<10;i++) Check(bundle.QueryResultSet.WindowFingerprint==bundle.WindowFingerprint,"Bundle query result set should share the bundle window fingerprint.");
        for(var i=0;i<10;i++) Check(bundle.WindowSummary.WindowFingerprint==bundle.WindowFingerprint,"Bundle window summary should share the bundle fingerprint.");
        for(var i=0;i<10;i++) Check(bundle.QueryResultSet.EntryCount==window.Count,"Bundle query result count should equal window count.");
        for(var i=0;i<10;i++) Check(bundle.WindowSummary.EntryCount==window.Count,"Bundle summary count should equal window count.");
        for(var i=0;i<10;i++) Check(bundle.QueryResultSet.Results.Single().Result.Handles.Single().IsValid,"Bundle query result handle should be valid.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowValidationRuntime.IsValid(window),"Source window should remain valid.");

        assert(round==100,$"Evidence window diagnostic bundle validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
