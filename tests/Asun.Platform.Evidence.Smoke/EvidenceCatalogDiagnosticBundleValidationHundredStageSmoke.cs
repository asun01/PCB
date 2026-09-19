using Asun.Platform.Evidence;

public static class EvidenceCatalogDiagnosticBundleValidationHundredStageSmoke
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
        var invalid=bundle with {SnapshotFingerprint=new string('a',64)};

        for(var i=0;i<10;i++) Check(EvidenceCatalogDiagnosticBundleValidationRuntime.IsValid(snapshot,bundle),"Valid diagnostic bundle should pass.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogDiagnosticBundleValidationRuntime.IsValid(snapshot,invalid),"Tampered snapshot fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogStatisticsValidationRuntime.IsValid(snapshot,bundle.Statistics),"Bundle statistics should remain valid.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryBatchValidationRuntime.IsValid(snapshot,bundle.QueryBatch),"Bundle query batch should remain valid.");
        for(var i=0;i<10;i++) Check(EvidenceReferenceResolutionValidationRuntime.IsValid(snapshot,referenceSet,bundle.ReferenceResolution),"Bundle reference resolution should remain valid.");
        for(var i=0;i<10;i++) Check(bundle.Statistics.DescriptorCount==snapshot.Count,"Bundle descriptor count should match snapshot.");
        for(var i=0;i<10;i++) Check(bundle.QueryBatch.SnapshotFingerprint==bundle.SnapshotFingerprint,"Bundle query batch should use same snapshot fingerprint.");
        for(var i=0;i<10;i++) Check(bundle.ReferenceResolution.SnapshotFingerprint==bundle.SnapshotFingerprint,"Bundle reference resolution should use same snapshot fingerprint.");
        for(var i=0;i<10;i++) Check(bundle.QueryBatch.Results.Count==1,"Bundle should preserve one query result.");
        for(var i=0;i<10;i++) Check(bundle.ReferenceResolution.FoundHandles.Count==1,"Bundle should preserve one found reference.");

        assert(round==100,$"Evidence diagnostic bundle validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
