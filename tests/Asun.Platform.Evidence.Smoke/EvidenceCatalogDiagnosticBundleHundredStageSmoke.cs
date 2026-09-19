using Asun.Platform.Evidence;

public static class EvidenceCatalogDiagnosticBundleHundredStageSmoke
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
            new EvidenceDescriptor(EvidenceHandle.Create("text://001"),EvidenceKind.Text,"text/plain",5,"text")
        });
        var queryBatch=EvidenceCatalogQueryBatchRuntime.Execute(snapshot,new[]{
            new EvidenceDescriptorQuery(EvidenceKind.Image,null),
            new EvidenceDescriptorQuery(EvidenceKind.Text,null)
        });
        var referenceSet=new EvidenceReferenceSet(new[]{
            EvidenceHandle.Create("frame://001"),
            EvidenceHandle.Create("missing://001")
        });
        var resolution=EvidenceReferenceResolutionRuntime.Resolve(snapshot,referenceSet);
        var bundle=EvidenceCatalogDiagnosticBundleRuntime.Create(snapshot,queryBatch,resolution);

        for(var i=0;i<10;i++) Check(bundle.SnapshotFingerprint.Length==64,"Diagnostic bundle snapshot fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(bundle.Statistics.DescriptorCount==2,"Diagnostic bundle statistics should preserve descriptor count.");
        for(var i=0;i<10;i++) Check(bundle.QueryBatch.QueryCount==2,"Diagnostic bundle query batch should preserve two queries.");
        for(var i=0;i<10;i++) Check(bundle.ReferenceResolution.FoundHandles.Count==1,"Diagnostic bundle should preserve one found reference.");
        for(var i=0;i<10;i++) Check(bundle.ReferenceResolution.MissingHandles.Count==1,"Diagnostic bundle should preserve one missing reference.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogDiagnosticBundleValidationRuntime.IsValid(snapshot,bundle),"Diagnostic bundle should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogDiagnosticBundleRuntime.Create(snapshot,queryBatch,resolution)==bundle,"Diagnostic bundle creation should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogStatisticsValidationRuntime.IsValid(snapshot,bundle.Statistics),"Bundle statistics should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryBatchValidationRuntime.IsValid(snapshot,bundle.QueryBatch),"Bundle query batch should validate.");
        for(var i=0;i<10;i++) Check(bundle.SnapshotFingerprint==EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot),"Bundle should bind source snapshot.");

        assert(round==100,$"Evidence diagnostic bundle smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
