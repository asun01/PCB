using Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        static EvidenceCatalogSnapshotEnvelope Envelope(string handle,string name)=>
            EvidenceCatalogSnapshotEnvelopeRuntime.Create(
                EvidenceCatalogSnapshotRuntime.Create(new[]{
                    new EvidenceDescriptor(EvidenceHandle.Create(handle),EvidenceKind.Image,"image/raw",8,name)
                }));

        var unsorted=EvidenceCatalogSnapshotWindowRuntime.Create(new[]{
            new EvidenceCatalogSnapshotWindowEntry(3,Envelope("frame://003","third")),
            new EvidenceCatalogSnapshotWindowEntry(1,Envelope("frame://001","first")),
            new EvidenceCatalogSnapshotWindowEntry(2,Envelope("frame://002","second"))
        });
        var invalidSequence=new EvidenceCatalogSnapshotWindow(new[]{
            new EvidenceCatalogSnapshotWindowEntry(0,Envelope("frame://000","zero"))
        });

        for(var i=0;i<10;i++) Check(unsorted.Count==3,"Window count should remain stable.");
        for(var i=0;i<10;i++) Check(unsorted.Entries[0].Sequence==1,"Window should sort by ascending sequence.");
        for(var i=0;i<10;i++) Check(unsorted.Entries[2].Sequence==3,"Window should retain the highest sequence.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowValidationRuntime.IsValid(unsorted),"Ordered window should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogSnapshotWindowValidationRuntime.IsValid(invalidSequence),"Non-positive sequence should be rejected.");
        for(var i=0;i<10;i++) Check(unsorted.Entries.Select(entry=>entry.Sequence).SequenceEqual(new long[]{1,2,3}),"Window sequence ordering should remain deterministic.");
        for(var i=0;i<10;i++) Check(unsorted.Entries.All(entry=>EvidenceCatalogSnapshotEnvelopeValidationRuntime.IsValid(entry.Envelope)),"Window entries should retain valid envelopes.");
        for(var i=0;i<10;i++) Check(unsorted.Entries[1].Envelope.Snapshot.Find(EvidenceHandle.Create("frame://002")) is not null,"Window entry snapshot lookup should remain available.");
        for(var i=0;i<10;i++) Check(unsorted.Entries[2].Envelope.Fingerprint.Length==64,"Window envelope fingerprint should remain fixed width.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowRuntime.Create(unsorted.Entries).Entries[0].Sequence==1,"Recreation should preserve canonical sequence order.");

        assert(round==100,$"Evidence snapshot window smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
