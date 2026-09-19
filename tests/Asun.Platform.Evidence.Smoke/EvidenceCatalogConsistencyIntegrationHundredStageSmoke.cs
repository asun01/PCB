using Asun.Platform.Evidence;

public static class EvidenceCatalogConsistencyIntegrationHundredStageSmoke
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
        var referenceSet=new EvidenceReferenceSet(new[]{
            EvidenceHandle.Create("frame://001"),
            EvidenceHandle.Create("frame://003"),
            EvidenceHandle.Create("text://001")
        });
        var resolution=EvidenceReferenceResolutionRuntime.Resolve(snapshot,referenceSet);
        var statistics=EvidenceCatalogStatisticsRuntime.Create(snapshot);
        var report=EvidenceCatalogConsistencyReportRuntime.Create(snapshot,referenceSet,resolution);

        for(var i=0;i<10;i++) Check(EvidenceCatalogStatisticsValidationRuntime.IsValid(snapshot,statistics),"Integrated statistics should validate.");
        for(var i=0;i<10;i++) Check(EvidenceReferenceResolutionValidationRuntime.IsValid(snapshot,referenceSet,resolution),"Integrated reference resolution should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogConsistencyReportValidationRuntime.IsValid(snapshot,referenceSet,resolution,report),"Integrated consistency report should validate.");
        for(var i=0;i<10;i++) Check(report.DescriptorCount==3,"Integrated report should preserve three descriptors.");
        for(var i=0;i<10;i++) Check(report.ReferenceCount==3,"Integrated report should preserve three references.");
        for(var i=0;i<10;i++) Check(report.MissingReferenceCount==1,"Integrated report should preserve one missing reference.");
        for(var i=0;i<10;i++) Check(statistics.KnownByteLengthTotal==35,"Integrated statistics should preserve byte total.");
        for(var i=0;i<10;i++) Check(statistics.KindCounts.First(item=>item.Kind==EvidenceKind.Image).Count==2,"Integrated statistics should preserve image count.");
        for(var i=0;i<10;i++) Check(statistics.KindCounts.First(item=>item.Kind==EvidenceKind.Text).Count==1,"Integrated statistics should preserve text count.");
        for(var i=0;i<10;i++) Check(report.ReferenceResolutionFingerprint==EvidenceReferenceResolutionFingerprintRuntime.CreateFingerprint(resolution),"Integrated report should retain deterministic reference fingerprint.");

        assert(round==100,$"Evidence consistency integration smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
