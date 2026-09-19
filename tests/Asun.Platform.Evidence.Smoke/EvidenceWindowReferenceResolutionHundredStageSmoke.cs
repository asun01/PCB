using Asun.Platform.Evidence;

public static class EvidenceWindowReferenceResolutionHundredStageSmoke
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

        var window=EvidenceCatalogSnapshotWindowRuntime.Create(new[]{
            new EvidenceCatalogSnapshotWindowEntry(1,Envelope("frame://001",10)),
            new EvidenceCatalogSnapshotWindowEntry(2,Envelope("frame://002",20))
        });
        var referenceSet=new EvidenceReferenceSet(new[]{
            EvidenceHandle.Create("frame://001"),
            EvidenceHandle.Create("missing://001")
        });
        var result=EvidenceCatalogSnapshotWindowReferenceResolutionRuntime.Resolve(window,referenceSet);

        for(var i=0;i<10;i++) Check(result.Entries.Count==2,"Window reference resolution should contain one entry per snapshot.");
        for(var i=0;i<10;i++) Check(result.Entries[0].Sequence==1,"First resolution entry should preserve sequence one.");
        for(var i=0;i<10;i++) Check(result.Entries[1].Sequence==2,"Second resolution entry should preserve sequence two.");
        for(var i=0;i<10;i++) Check(result.Entries[0].Resolution.FoundHandles.Single().Value=="frame://001","First snapshot should find frame://001.");
        for(var i=0;i<10;i++) Check(result.Entries[1].Resolution.MissingHandles.Single().Value=="missing://001","Second snapshot should retain the missing handle.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowReferenceResolutionValidationRuntime.IsValid(window,result),"Window reference resolution should validate.");
        for(var i=0;i<10;i++) Check(result.WindowFingerprint.Length==64,"Window reference resolution should bind a fixed-width window fingerprint.");
        for(var i=0;i<10;i++) Check(result.ReferenceSet==referenceSet,"Window reference resolution should retain the source reference set.");
        for(var i=0;i<10;i++) Check(result.Entries.All(entry=>entry.Resolution.FoundHandles.Count+entry.Resolution.MissingHandles.Count==referenceSet.Handles.Count),"Every entry should account for all references.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowReferenceResolutionRuntime.Resolve(window,referenceSet)==result,"Window reference resolution should be deterministic.");

        assert(round==100,$"Evidence window reference resolution smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
