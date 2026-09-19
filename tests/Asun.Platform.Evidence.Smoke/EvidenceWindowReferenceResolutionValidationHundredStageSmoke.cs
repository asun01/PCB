using Asun.Platform.Evidence;

public static class EvidenceWindowReferenceResolutionValidationHundredStageSmoke
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
                    new EvidenceDescriptor(EvidenceHandle.Create(handle),EvidenceKind.Image,"image/raw",10,handle)
                }));

        var window=EvidenceCatalogSnapshotWindowRuntime.Create(new[]{
            new EvidenceCatalogSnapshotWindowEntry(1,Envelope("frame://001"))
        });
        var referenceSet=new EvidenceReferenceSet(new[]{
            EvidenceHandle.Create("frame://001"),
            EvidenceHandle.Create("missing://001")
        });
        var result=EvidenceCatalogSnapshotWindowReferenceResolutionRuntime.Resolve(window,referenceSet);
        var invalid=result with {Entries=Array.Empty<EvidenceCatalogSnapshotWindowReferenceResolutionEntry>()};

        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowReferenceResolutionValidationRuntime.IsValid(window,result),"Valid window reference resolution should pass.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogSnapshotWindowReferenceResolutionValidationRuntime.IsValid(window,invalid),"Missing window resolution entry should be rejected.");
        for(var i=0;i<10;i++) Check(result.ReferenceSet.Handles.Count==2,"Window reference resolution should retain two handles.");
        for(var i=0;i<10;i++) Check(result.Entries.Count==window.Count,"Entry count should match window count.");
        for(var i=0;i<10;i++) Check(result.Entries[0].Resolution.FoundHandles.Count==1,"Snapshot should resolve one found handle.");
        for(var i=0;i<10;i++) Check(result.Entries[0].Resolution.MissingHandles.Count==1,"Snapshot should resolve one missing handle.");
        for(var i=0;i<10;i++) Check(result.Entries[0].Resolution.SnapshotFingerprint.Length==64,"Each resolution should retain a snapshot fingerprint.");
        for(var i=0;i<10;i++) Check(result.Entries.Select(entry=>entry.Sequence).SequenceEqual(new long[]{1}),"Window resolution sequence should remain canonical.");
        for(var i=0;i<10;i++) Check(EvidenceReferenceSetValidationRuntime.IsValid(result.ReferenceSet),"Window reference set should validate independently.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowValidationRuntime.IsValid(window),"Source window should remain valid.");

        assert(round==100,$"Evidence window reference resolution validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
