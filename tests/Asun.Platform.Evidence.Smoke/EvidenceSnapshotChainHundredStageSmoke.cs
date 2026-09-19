using Asun.Platform.Evidence;

public static class EvidenceSnapshotChainHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        static EvidenceCatalogSnapshotEnvelope Envelope(string handle,long length)=>
            EvidenceCatalogSnapshotEnvelopeRuntime.Create(
                EvidenceCatalogSnapshotRuntime.Create(new[]{
                    new EvidenceDescriptor(EvidenceHandle.Create(handle),EvidenceKind.Image,"image/raw",length,handle)
                }));

        var previous=EvidenceCatalogSnapshotWindowRuntime.Create(new[]{
            new EvidenceCatalogSnapshotWindowEntry(1,Envelope("frame://001",10)),
            new EvidenceCatalogSnapshotWindowEntry(2,Envelope("frame://002",20))
        });
        var current=EvidenceCatalogSnapshotWindowRuntime.Create(new[]{
            new EvidenceCatalogSnapshotWindowEntry(2,Envelope("frame://002",21)),
            new EvidenceCatalogSnapshotWindowEntry(3,Envelope("frame://003",30))
        });
        var report=EvidenceCatalogSnapshotIntegrityReportRuntime.Create(current.Entries[0].Envelope.Snapshot);
        var changes=EvidenceCatalogSnapshotChangeRuntime.Create(
            previous.Entries[1].Envelope.Snapshot,
            current.Entries[0].Envelope.Snapshot);
        var previousSummary=EvidenceCatalogSnapshotWindowIntegritySummaryRuntime.Create(previous);
        var currentSummary=EvidenceCatalogSnapshotWindowIntegritySummaryRuntime.Create(current);
        var transition=EvidenceCatalogSnapshotWindowTransitionRuntime.Create(previous,current);

        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotIntegrityReportValidationRuntime.IsValid(current.Entries[0].Envelope.Snapshot,report),"Current descriptor integrity report should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotChangeValidationRuntime.IsValid(changes),"Descriptor change projection should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowIntegritySummaryValidationRuntime.IsValid(previous,previousSummary),"Previous window summary should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowIntegritySummaryValidationRuntime.IsValid(current,currentSummary),"Current window summary should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowTransitionValidationRuntime.IsValid(previous,current,transition),"Window transition should validate.");
        for(var i=0;i<10;i++) Check(currentSummary.EntryCount==2,"Current summary should preserve entry count.");
        for(var i=0;i<10;i++) Check(report.DescriptorCount==1,"Latest descriptor report should preserve cardinality.");
        for(var i=0;i<10;i++) Check(changes.Count==1,"Latest shared descriptor transition should contain one change.");
        for(var i=0;i<10;i++) Check(transition.Diff.ChangedSequences.Single()==2,"Integrated transition should expose changed sequence 2.");
        for(var i=0;i<10;i++) Check(currentSummary.WindowFingerprint.Length==64,"Integrated current window fingerprint should be fixed width.");

        assert(round==100,$"Evidence snapshot chain smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
