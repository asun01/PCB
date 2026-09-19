using Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotIntegrationHundredStageSmoke
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

        var first=EvidenceCatalogSnapshotWindowRuntime.Create(new[]{
            new EvidenceCatalogSnapshotWindowEntry(1,Envelope("frame://001",10)),
            new EvidenceCatalogSnapshotWindowEntry(2,Envelope("frame://002",20))
        });
        var second=EvidenceCatalogSnapshotWindowRuntime.Create(new[]{
            new EvidenceCatalogSnapshotWindowEntry(2,Envelope("frame://002",21)),
            new EvidenceCatalogSnapshotWindowEntry(3,Envelope("frame://003",30))
        });

        var diff=EvidenceCatalogSnapshotWindowDiffRuntime.Diff(first,second);

        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowValidationRuntime.IsValid(first),"First window should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowValidationRuntime.IsValid(second),"Second window should validate.");
        for(var i=0;i<10;i++) Check(first.Entries.All(entry=>EvidenceCatalogSnapshotEnvelopeValidationRuntime.IsValid(entry.Envelope)),"First envelopes should validate.");
        for(var i=0;i<10;i++) Check(second.Entries.All(entry=>EvidenceCatalogSnapshotEnvelopeValidationRuntime.IsValid(entry.Envelope)),"Second envelopes should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowDiffValidationRuntime.IsValid(diff),"Integrated window diff should validate.");
        for(var i=0;i<10;i++) Check(diff.AddedSequences.Single()==3,"Integrated diff should preserve added sequence.");
        for(var i=0;i<10;i++) Check(diff.RemovedSequences.Single()==1,"Integrated diff should preserve removed sequence.");
        for(var i=0;i<10;i++) Check(diff.ChangedSequences.Single()==2,"Integrated diff should preserve changed sequence.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotFingerprintValidationRuntime.IsValid(second.Entries[1].Envelope.Snapshot,second.Entries[1].Envelope.Fingerprint),"Latest envelope fingerprint should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowDiffRuntime.Diff(first,second).Equals(diff),"Integrated diff should be deterministic.");

        assert(round==100,$"Evidence snapshot integration smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
