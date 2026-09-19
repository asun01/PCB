using Asun.Platform.Evidence;

public static class EvidenceCatalogConsistencyReportHundredStageSmoke
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
            new EvidenceDescriptor(EvidenceHandle.Create("frame://002"),EvidenceKind.Image,"image/raw",20,"two")
        });
        var referenceSet=new EvidenceReferenceSet(new[]{
            EvidenceHandle.Create("frame://001"),
            EvidenceHandle.Create("frame://003")
        });
        var resolution=EvidenceReferenceResolutionRuntime.Resolve(snapshot,referenceSet);
        var report=EvidenceCatalogConsistencyReportRuntime.Create(snapshot,referenceSet,resolution);
        var invalid=report with {MissingReferenceCount=0};

        for(var i=0;i<10;i++) Check(report.DescriptorCount==2,"Consistency report should preserve descriptor count.");
        for(var i=0;i<10;i++) Check(report.ReferenceCount==2,"Consistency report should preserve reference count.");
        for(var i=0;i<10;i++) Check(report.MissingReferenceCount==1,"Consistency report should preserve missing reference count.");
        for(var i=0;i<10;i++) Check(report.SnapshotFingerprint.Length==64,"Consistency report snapshot fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(report.StatisticsFingerprint.Length==64,"Consistency report statistics fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(report.ReferenceResolutionFingerprint!.Length==64,"Consistency report reference fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogConsistencyReportValidationRuntime.IsValid(snapshot,referenceSet,resolution,report),"Consistency report should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogConsistencyReportValidationRuntime.IsValid(snapshot,referenceSet,resolution,invalid),"Tampered missing-reference count should be rejected.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogConsistencyReportRuntime.Create(snapshot,referenceSet,resolution)==report,"Consistency report creation should be deterministic.");
        for(var i=0;i<10;i++) Check(report.SnapshotFingerprint==EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot),"Consistency report should bind the source snapshot fingerprint.");

        assert(round==100,$"Evidence catalog consistency report smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
