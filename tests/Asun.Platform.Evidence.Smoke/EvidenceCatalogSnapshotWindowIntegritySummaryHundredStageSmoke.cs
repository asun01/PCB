using Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowIntegritySummaryHundredStageSmoke
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
                    new EvidenceDescriptor(EvidenceHandle.Create(handle),EvidenceKind.Image,"image/raw",8,handle)
                }));

        var window=EvidenceCatalogSnapshotWindowRuntime.Create(new[]{
            new EvidenceCatalogSnapshotWindowEntry(2,Envelope("frame://002")),
            new EvidenceCatalogSnapshotWindowEntry(1,Envelope("frame://001"))
        });
        var summary=EvidenceCatalogSnapshotWindowIntegritySummaryRuntime.Create(window);
        var invalid=summary with {LastSequence=3};

        for(var i=0;i<10;i++) Check(summary.EntryCount==2,"Window summary should preserve entry count.");
        for(var i=0;i<10;i++) Check(summary.FirstSequence==1,"Window summary first sequence should be 1.");
        for(var i=0;i<10;i++) Check(summary.LastSequence==2,"Window summary last sequence should be 2.");
        for(var i=0;i<10;i++) Check(summary.WindowFingerprint.Length==64,"Window summary fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowIntegritySummaryValidationRuntime.IsValid(window,summary),"Window summary should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogSnapshotWindowIntegritySummaryValidationRuntime.IsValid(window,invalid),"Tampered last sequence should be rejected.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowIntegritySummaryRuntime.Create(window)==summary,"Window summary creation should be deterministic.");
        for(var i=0;i<10;i++) Check(summary.WindowFingerprint.All(Uri.IsHexDigit),"Window summary fingerprint should remain hexadecimal.");
        for(var i=0;i<10;i++) Check(window.Entries[0].Sequence==1,"Window should remain canonically sorted.");
        for(var i=0;i<10;i++) Check(summary.EntryCount==window.Count,"Window summary count should remain tied to the window.");

        assert(round==100,$"Evidence window integrity summary smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
