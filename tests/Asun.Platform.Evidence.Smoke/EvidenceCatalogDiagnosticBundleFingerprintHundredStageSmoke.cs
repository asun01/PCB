using Asun.Platform.Evidence;

public static class EvidenceCatalogDiagnosticBundleFingerprintHundredStageSmoke
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
            new EvidenceDescriptor(EvidenceHandle.Create("frame://001"),EvidenceKind.Image,"image/raw",10,"one")
        });
        var batch=EvidenceCatalogQueryBatchRuntime.Execute(snapshot,new[]{
            new EvidenceDescriptorQuery(EvidenceKind.Image,null)
        });
        var referenceSet=new EvidenceReferenceSet(new[]{EvidenceHandle.Create("frame://001")});
        var resolution=EvidenceReferenceResolutionRuntime.Resolve(snapshot,referenceSet);
        var bundle=EvidenceCatalogDiagnosticBundleRuntime.Create(snapshot,batch,resolution);
        var fingerprint=EvidenceCatalogDiagnosticBundleFingerprintRuntime.CreateFingerprint(bundle);
        var invalid=bundle with {Statistics=bundle.Statistics with {KnownByteLengthTotal=11}};

        for(var i=0;i<10;i++) Check(fingerprint.Length==64,"Diagnostic bundle fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(fingerprint.All(Uri.IsHexDigit),"Diagnostic bundle fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogDiagnosticBundleFingerprintRuntime.CreateFingerprint(bundle)==fingerprint,"Diagnostic bundle fingerprint should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogDiagnosticBundleFingerprintValidationRuntime.IsValid(snapshot,bundle,fingerprint),"Diagnostic bundle fingerprint should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogDiagnosticBundleFingerprintValidationRuntime.IsValid(snapshot,invalid,fingerprint),"Diagnostic bundle mutation should invalidate the original fingerprint.");
        for(var i=0;i<10;i++) Check(fingerprint.All(character=>char.ToLowerInvariant(character)==character),"Diagnostic bundle fingerprint should remain lowercase.");
        for(var i=0;i<10;i++) Check(bundle.QueryBatch.QueryCount==1,"Diagnostic bundle should retain one query.");
        for(var i=0;i<10;i++) Check(bundle.ReferenceResolution.FoundHandles.Count==1,"Diagnostic bundle should retain one found reference.");
        for(var i=0;i<10;i++) Check(bundle.Statistics.DescriptorCount==1,"Diagnostic bundle should retain one descriptor.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogDiagnosticBundleValidationRuntime.IsValid(snapshot,bundle),"Fingerprint source bundle should remain valid.");

        assert(round==100,$"Evidence diagnostic bundle fingerprint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
