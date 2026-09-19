using Asun.Platform.Evidence;

public static class EvidenceCatalogWindowDiagnosticBundleFingerprintHundredStageSmoke
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
        var fingerprint=EvidenceCatalogWindowDiagnosticBundleFingerprintRuntime.CreateFingerprint(bundle);
        var invalid=bundle with {WindowSummary=bundle.WindowSummary with {EntryCount=2}};

        for(var i=0;i<10;i++) Check(fingerprint.Length==64,"Window diagnostic bundle fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(fingerprint.All(Uri.IsHexDigit),"Window diagnostic bundle fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogWindowDiagnosticBundleFingerprintRuntime.CreateFingerprint(bundle)==fingerprint,"Window diagnostic bundle fingerprint should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogWindowDiagnosticBundleFingerprintValidationRuntime.IsValid(window,bundle,fingerprint),"Window diagnostic bundle fingerprint should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogWindowDiagnosticBundleFingerprintValidationRuntime.IsValid(window,invalid,fingerprint),"Window diagnostic bundle mutation should invalidate the original fingerprint.");
        for(var i=0;i<10;i++) Check(fingerprint.All(character=>char.ToLowerInvariant(character)==character),"Window diagnostic bundle fingerprint should remain lowercase.");
        for(var i=0;i<10;i++) Check(bundle.QueryResultSet.Results.Count==1,"Window diagnostic bundle should retain one query result.");
        for(var i=0;i<10;i++) Check(bundle.WindowSummary.EntryCount==1,"Window diagnostic bundle should retain one summary entry.");
        for(var i=0;i<10;i++) Check(bundle.WindowFingerprint.Length==64,"Window diagnostic source fingerprint should remain fixed width.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogWindowDiagnosticBundleValidationRuntime.IsValid(window,bundle),"Fingerprint source window diagnostic bundle should remain valid.");

        assert(round==100,$"Evidence window diagnostic bundle fingerprint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
