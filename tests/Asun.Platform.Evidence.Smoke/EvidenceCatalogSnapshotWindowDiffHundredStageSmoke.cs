using Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowDiffHundredStageSmoke
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
                    new EvidenceDescriptor(EvidenceHandle.Create(handle),EvidenceKind.Data,"application/json",length,handle)
                }));

        var previous=EvidenceCatalogSnapshotWindowRuntime.Create(new[]{
            new EvidenceCatalogSnapshotWindowEntry(1,Envelope("frame://001",10)),
            new EvidenceCatalogSnapshotWindowEntry(2,Envelope("frame://002",20))
        });
        var current=EvidenceCatalogSnapshotWindowRuntime.Create(new[]{
            new EvidenceCatalogSnapshotWindowEntry(2,Envelope("frame://002",21)),
            new EvidenceCatalogSnapshotWindowEntry(3,Envelope("frame://003",30))
        });
        var diff=EvidenceCatalogSnapshotWindowDiffRuntime.Diff(previous,current);
        var invalid=new EvidenceCatalogSnapshotWindowDiff(
            new[]{1L},new[]{1L},Array.Empty<long>());

        for(var i=0;i<10;i++) Check(diff.AddedSequences.Single()==3,"Window diff added sequence should be 3.");
        for(var i=0;i<10;i++) Check(diff.RemovedSequences.Single()==1,"Window diff removed sequence should be 1.");
        for(var i=0;i<10;i++) Check(diff.ChangedSequences.Single()==2,"Window diff changed sequence should be 2.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowDiffValidationRuntime.IsValid(diff),"Window diff should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogSnapshotWindowDiffValidationRuntime.IsValid(invalid),"Overlapping diff categories should be rejected.");
        for(var i=0;i<10;i++) Check(diff.AddedSequences.SequenceEqual(diff.AddedSequences.OrderBy(sequence=>sequence)),"Added sequence ordering should be deterministic.");
        for(var i=0;i<10;i++) Check(diff.RemovedSequences.SequenceEqual(diff.RemovedSequences.OrderBy(sequence=>sequence)),"Removed sequence ordering should be deterministic.");
        for(var i=0;i<10;i++) Check(diff.ChangedSequences.SequenceEqual(diff.ChangedSequences.OrderBy(sequence=>sequence)),"Changed sequence ordering should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowDiffRuntime.Diff(previous,current).Equals(diff),"Window diff determinism should remain stable.");
        for(var i=0;i<10;i++) Check(!diff.IsEmpty,"Window diff should not be empty.");

        assert(round==100,$"Evidence snapshot window diff smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
