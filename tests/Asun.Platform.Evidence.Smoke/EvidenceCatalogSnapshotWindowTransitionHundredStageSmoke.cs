using Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowTransitionHundredStageSmoke
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
        var transition=EvidenceCatalogSnapshotWindowTransitionRuntime.Create(previous,current);
        var invalid=transition with {CurrentWindowFingerprint=transition.PreviousWindowFingerprint};

        for(var i=0;i<10;i++) Check(transition.PreviousWindowFingerprint.Length==64,"Previous window fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(transition.CurrentWindowFingerprint.Length==64,"Current window fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowTransitionValidationRuntime.IsValid(previous,current,transition),"Window transition should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogSnapshotWindowTransitionValidationRuntime.IsValid(previous,current,invalid),"Tampered current window fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(transition.Diff.AddedSequences.Single()==3,"Transition diff should preserve added sequence.");
        for(var i=0;i<10;i++) Check(transition.Diff.RemovedSequences.Single()==1,"Transition diff should preserve removed sequence.");
        for(var i=0;i<10;i++) Check(transition.Diff.ChangedSequences.Single()==2,"Transition diff should preserve changed sequence.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowTransitionRuntime.Create(previous,current).Equals(transition),"Window transition creation should be deterministic.");
        for(var i=0;i<10;i++) Check(transition.PreviousWindowFingerprint!=transition.CurrentWindowFingerprint,"Window fingerprints should differ after a mutation.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowDiffValidationRuntime.IsValid(transition.Diff),"Transition diff should independently validate.");

        assert(round==100,$"Evidence window transition smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
