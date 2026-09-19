using Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotIntegrityReportHundredStageSmoke
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
            new EvidenceDescriptor(EvidenceHandle.Create("frame://002"),EvidenceKind.Data,"application/json",20,"two")
        });
        var report=EvidenceCatalogSnapshotIntegrityReportRuntime.Create(snapshot);
        var invalid=report with {DescriptorCount=3};

        for(var i=0;i<10;i++) Check(report.DescriptorCount==2,"Integrity report should preserve descriptor count.");
        for(var i=0;i<10;i++) Check(report.SnapshotFingerprint.Length==64,"Integrity report fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotIntegrityReportValidationRuntime.IsValid(snapshot,report),"Integrity report should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogSnapshotIntegrityReportValidationRuntime.IsValid(snapshot,invalid),"Mismatched descriptor count should be rejected.");
        for(var i=0;i<10;i++) Check(report.SnapshotFingerprint==EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot),"Integrity report fingerprint should match snapshot fingerprint.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotIntegrityReportRuntime.Create(snapshot)==report,"Integrity report creation should be deterministic.");
        for(var i=0;i<10;i++) Check(report.SnapshotFingerprint.All(Uri.IsHexDigit),"Integrity report fingerprint should remain hexadecimal.");
        for(var i=0;i<10;i++) Check(snapshot.Count==report.DescriptorCount,"Report count should remain tied to snapshot count.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotFingerprintValidationRuntime.IsValid(snapshot,report.SnapshotFingerprint),"Report fingerprint should independently validate.");
        for(var i=0;i<10;i++) Check(report.DescriptorCount>0,"Report should retain the non-empty snapshot cardinality.");

        assert(round==100,$"Evidence snapshot integrity report smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
