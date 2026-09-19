using Asun.Platform.Evidence;

public static class EvidenceCatalogWindowDiagnosticIntegrationHundredStageSmoke
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
        var bundle=EvidenceCatalogWindowDiagnosticBundleRuntime.Create(window,resultSet);
        var fingerprint=EvidenceCatalogWindowDiagnosticBundleFingerprintRuntime.CreateFingerprint(bundle);

        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowValidationRuntime.IsValid(window),"Integrated window should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowQueryValidationRuntime.IsValid(window,resultSet),"Integrated window query should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogWindowDiagnosticBundleValidationRuntime.IsValid(window,bundle),"Integrated diagnostic bundle should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogWindowDiagnosticBundleFingerprintValidationRuntime.IsValid(window,bundle,fingerprint),"Integrated diagnostic bundle fingerprint should validate.");
        for(var i=0;i<10;i++) Check(bundle.QueryResultSet.Results[0].Result.MatchCount==1,"Integrated first image entry should match one image.");
        for(var i=0;i<10;i++) Check(bundle.QueryResultSet.Results[1].Result.MatchCount==1,"Integrated second image entry should match one image.");
        for(var i=0;i<10;i++) Check(bundle.QueryResultSet.Results[2].Result.MatchCount==0,"Integrated text entry should match zero images.");
        for(var i=0;i<10;i++) Check(bundle.WindowSummary.EntryCount==3,"Integrated window summary should preserve three entries.");
        for(var i=0;i<10;i++) Check(bundle.WindowSummary.LastSequence==3,"Integrated window summary should preserve last sequence.");
        for(var i=0;i<10;i++) Check(fingerprint==EvidenceCatalogWindowDiagnosticBundleFingerprintRuntime.CreateFingerprint(bundle),"Integrated window diagnostic fingerprint should be deterministic.");

        assert(round==100,$"Evidence window diagnostic integration smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
