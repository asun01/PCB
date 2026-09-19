using Asun.Platform.Evidence;

public static class EvidenceReferenceResolutionValidationHundredStageSmoke
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
            new EvidenceDescriptor(EvidenceHandle.Create("frame://001"),EvidenceKind.Image,"image/raw",10,"one")
        });
        var referenceSet=new EvidenceReferenceSet(new[]{
            EvidenceHandle.Create("frame://001"),
            EvidenceHandle.Create("frame://002")
        });
        var resolution=EvidenceReferenceResolutionRuntime.Resolve(snapshot,referenceSet);
        var invalid=resolution with {MissingHandles=Array.Empty<EvidenceHandle>()};

        for(var i=0;i<10;i++) Check(EvidenceReferenceResolutionValidationRuntime.IsValid(snapshot,referenceSet,resolution),"Valid resolution should pass.");
        for(var i=0;i<10;i++) Check(!EvidenceReferenceResolutionValidationRuntime.IsValid(snapshot,referenceSet,invalid),"Incomplete resolution should be rejected.");
        for(var i=0;i<10;i++) Check(resolution.FoundHandles.Count+resolution.MissingHandles.Count==referenceSet.Handles.Count,"Resolution should account for all references.");
        for(var i=0;i<10;i++) Check(resolution.FoundHandles.Count==1,"Resolution should preserve one found handle.");
        for(var i=0;i<10;i++) Check(resolution.MissingHandles.Count==1,"Resolution should preserve one missing handle.");
        for(var i=0;i<10;i++) Check(resolution.FoundHandles[0].Value=="frame://001","Found resolution handle should remain stable.");
        for(var i=0;i<10;i++) Check(resolution.MissingHandles[0].Value=="frame://002","Missing resolution handle should remain stable.");
        for(var i=0;i<10;i++) Check(resolution.SnapshotFingerprint==EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot),"Resolution snapshot fingerprint should match.");
        for(var i=0;i<10;i++) Check(EvidenceReferenceSetValidationRuntime.IsValid(referenceSet),"Source reference set should remain valid.");
        for(var i=0;i<10;i++) Check(EvidenceReferenceResolutionValidationRuntime.Validate(snapshot,referenceSet,resolution).Count==0,"Valid resolution should produce no errors.");

        assert(round==100,$"Evidence reference resolution validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
