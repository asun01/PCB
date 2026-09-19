using Asun.Platform.Evidence;

public static class EvidenceCatalogDiagnosticIntegrationHundredStageSmoke
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
            new EvidenceDescriptor(EvidenceHandle.Create("frame://002"),EvidenceKind.Image,"image/raw",20,"two"),
            new EvidenceDescriptor(EvidenceHandle.Create("text://001"),EvidenceKind.Text,"text/plain",5,"text")
        });
        var batch=EvidenceCatalogQueryBatchRuntime.Execute(snapshot,new[]{
            new EvidenceDescriptorQuery(EvidenceKind.Image,null),
            new EvidenceDescriptorQuery(EvidenceKind.Text,null)
        });
        var referenceSet=new EvidenceReferenceSet(new[]{
            EvidenceHandle.Create("frame://001"),
            EvidenceHandle.Create("missing://001"),
            EvidenceHandle.Create("text://001")
        });
        var resolution=EvidenceReferenceResolutionRuntime.Resolve(snapshot,referenceSet);
        var bundle=EvidenceCatalogDiagnosticBundleRuntime.Create(snapshot,batch,resolution);
        var fingerprint=EvidenceCatalogDiagnosticBundleFingerprintRuntime.CreateFingerprint(bundle);

        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotValidationRuntime.IsValid(snapshot),"Integrated snapshot should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogStatisticsValidationRuntime.IsValid(snapshot,bundle.Statistics),"Integrated statistics should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryBatchValidationRuntime.IsValid(snapshot,bundle.QueryBatch),"Integrated query batch should validate.");
        for(var i=0;i<10;i++) Check(EvidenceReferenceResolutionValidationRuntime.IsValid(snapshot,referenceSet,bundle.ReferenceResolution),"Integrated reference closure should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogDiagnosticBundleValidationRuntime.IsValid(snapshot,bundle),"Integrated diagnostic bundle should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogDiagnosticBundleFingerprintValidationRuntime.IsValid(snapshot,bundle,fingerprint),"Integrated diagnostic fingerprint should validate.");
        for(var i=0;i<10;i++) Check(bundle.Statistics.KnownByteLengthTotal==35,"Integrated statistics should preserve byte total.");
        for(var i=0;i<10;i++) Check(bundle.QueryBatch.QueryCount==2,"Integrated query batch should preserve two queries.");
        for(var i=0;i<10;i++) Check(bundle.ReferenceResolution.MissingHandles.Single().Value=="missing://001","Integrated reference closure should preserve one missing handle.");
        for(var i=0;i<10;i++) Check(fingerprint==EvidenceCatalogDiagnosticBundleFingerprintRuntime.CreateFingerprint(bundle),"Integrated diagnostic fingerprint should be deterministic.");

        assert(round==100,$"Evidence diagnostic integration smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
