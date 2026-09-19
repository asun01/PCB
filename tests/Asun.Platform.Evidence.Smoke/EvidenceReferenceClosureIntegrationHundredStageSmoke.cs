using Asun.Platform.Evidence;

public static class EvidenceReferenceClosureIntegrationHundredStageSmoke
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
            new EvidenceDescriptor(EvidenceHandle.Create("frame://001"),EvidenceKind.Image,"image/raw",10,"one"),
            new EvidenceDescriptor(EvidenceHandle.Create("frame://002"),EvidenceKind.Image,"image/raw",20,"two")
        });
        var referenceSet=new EvidenceReferenceSet(new[]{
            EvidenceHandle.Create("frame://001"),
            EvidenceHandle.Create("frame://003"),
            EvidenceHandle.Create("frame://002")
        });
        var resolution=EvidenceReferenceResolutionRuntime.Resolve(snapshot,referenceSet);
        var fingerprint=EvidenceReferenceResolutionFingerprintRuntime.CreateFingerprint(resolution);

        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotValidationRuntime.IsValid(snapshot),"Integrated snapshot should validate.");
        for(var i=0;i<10;i++) Check(EvidenceReferenceSetValidationRuntime.IsValid(referenceSet),"Integrated reference set should validate.");
        for(var i=0;i<10;i++) Check(EvidenceReferenceResolutionValidationRuntime.IsValid(snapshot,referenceSet,resolution),"Integrated reference resolution should validate.");
        for(var i=0;i<10;i++) Check(EvidenceReferenceResolutionFingerprintValidationRuntime.IsValid(snapshot,referenceSet,resolution,fingerprint),"Integrated resolution fingerprint should validate.");
        for(var i=0;i<10;i++) Check(resolution.FoundHandles.Count==2,"Integrated closure should find two handles.");
        for(var i=0;i<10;i++) Check(resolution.MissingHandles.Single().Value=="frame://003","Integrated closure should expose one missing handle.");
        for(var i=0;i<10;i++) Check(resolution.FoundHandles.SequenceEqual(new[]{EvidenceHandle.Create("frame://001"),EvidenceHandle.Create("frame://002")}),"Integrated found handles should remain canonical.");
        for(var i=0;i<10;i++) Check(referenceSet.Handles.Count==3,"Integrated reference set should preserve three opaque handles.");
        for(var i=0;i<10;i++) Check(fingerprint==EvidenceReferenceResolutionFingerprintRuntime.CreateFingerprint(resolution),"Integrated resolution fingerprint should be deterministic.");
        for(var i=0;i<10;i++) Check(resolution.SnapshotFingerprint==EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot),"Integrated resolution should bind the source snapshot.");

        assert(round==100,$"Evidence reference closure integration smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
