using Asun.Platform.Evidence;

public static class EvidenceWindowReferenceClosureIntegrationHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        static EvidenceCatalogSnapshotEnvelope Envelope(string handle,EvidenceKind kind)=>
            EvidenceCatalogSnapshotEnvelopeRuntime.Create(
                EvidenceCatalogSnapshotRuntime.Create(new[]{
                    new EvidenceDescriptor(EvidenceHandle.Create(handle),kind,kind==EvidenceKind.Image ? "image/raw" : "text/plain",10,handle)
                }));

        var window=EvidenceCatalogSnapshotWindowRuntime.Create(new[]{
            new EvidenceCatalogSnapshotWindowEntry(1,Envelope("frame://001",EvidenceKind.Image)),
            new EvidenceCatalogSnapshotWindowEntry(2,Envelope("frame://002",EvidenceKind.Image)),
            new EvidenceCatalogSnapshotWindowEntry(3,Envelope("text://001",EvidenceKind.Text))
        });
        var referenceSet=new EvidenceReferenceSet(new[]{
            EvidenceHandle.Create("frame://001"),
            EvidenceHandle.Create("frame://002"),
            EvidenceHandle.Create("missing://001")
        });
        var result=EvidenceCatalogSnapshotWindowReferenceResolutionRuntime.Resolve(window,referenceSet);
        var fingerprint=EvidenceCatalogSnapshotWindowReferenceResolutionFingerprintRuntime.CreateFingerprint(result);

        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowReferenceResolutionValidationRuntime.IsValid(window,result),"Integrated window reference closure should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowReferenceResolutionFingerprintValidationRuntime.IsValid(window,result,fingerprint),"Integrated window reference fingerprint should validate.");
        for(var i=0;i<10;i++) Check(result.Entries.Count==3,"Integrated window closure should preserve three entries.");
        for(var i=0;i<10;i++) Check(result.Entries[0].Resolution.FoundHandles.Count==1,"First snapshot should find one reference.");
        for(var i=0;i<10;i++) Check(result.Entries[1].Resolution.FoundHandles.Count==1,"Second snapshot should find one reference.");
        for(var i=0;i<10;i++) Check(result.Entries[2].Resolution.FoundHandles.Count==0,"Text snapshot should find no image references.");
        for(var i=0;i<10;i++) Check(result.Entries.All(entry=>entry.Resolution.MissingHandles.Single().Value=="missing://001"),"Every snapshot should retain the missing reference.");
        for(var i=0;i<10;i++) Check(result.ReferenceSet.Handles.Count==3,"Integrated closure should preserve three opaque references.");
        for(var i=0;i<10;i++) Check(fingerprint==EvidenceCatalogSnapshotWindowReferenceResolutionFingerprintRuntime.CreateFingerprint(result),"Integrated closure fingerprint should be deterministic.");
        for(var i=0;i<10;i++) Check(result.WindowFingerprint==EvidenceCatalogSnapshotWindowIntegritySummaryRuntime.Create(window).WindowFingerprint,"Integrated closure should bind source window fingerprint.");

        assert(round==100,$"Evidence window reference closure integration smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
