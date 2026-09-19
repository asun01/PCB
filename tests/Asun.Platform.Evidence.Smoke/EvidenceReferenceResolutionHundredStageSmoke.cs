using Asun.Platform.Evidence;

public static class EvidenceReferenceResolutionHundredStageSmoke
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
            EvidenceHandle.Create("frame://003")
        });
        var resolution=EvidenceReferenceResolutionRuntime.Resolve(snapshot,referenceSet);

        for(var i=0;i<10;i++) Check(resolution.FoundHandles.Single().Value=="frame://001","Existing handle should resolve as found.");
        for(var i=0;i<10;i++) Check(resolution.MissingHandles.Single().Value=="frame://003","Absent handle should resolve as missing.");
        for(var i=0;i<10;i++) Check(resolution.FoundHandles.Count==1,"Resolution should contain one found handle.");
        for(var i=0;i<10;i++) Check(resolution.MissingHandles.Count==1,"Resolution should contain one missing handle.");
        for(var i=0;i<10;i++) Check(EvidenceReferenceResolutionValidationRuntime.IsValid(snapshot,referenceSet,resolution),"Reference resolution should validate.");
        for(var i=0;i<10;i++) Check(resolution.FoundHandles.Intersect(resolution.MissingHandles).Count()==0,"Found and missing handles should be disjoint.");
        for(var i=0;i<10;i++) Check(resolution.SnapshotFingerprint.Length==64,"Resolution should bind a fixed-width snapshot fingerprint.");
        for(var i=0;i<10;i++) Check(EvidenceReferenceResolutionRuntime.Resolve(snapshot,referenceSet)==resolution,"Resolution should be deterministic.");
        for(var i=0;i<10;i++) Check(resolution.FoundHandles.SequenceEqual(resolution.FoundHandles.OrderBy(handle=>handle.Value,StringComparer.Ordinal)),"Found handles should be canonical.");
        for(var i=0;i<10;i++) Check(resolution.MissingHandles.SequenceEqual(resolution.MissingHandles.OrderBy(handle=>handle.Value,StringComparer.Ordinal)),"Missing handles should be canonical.");

        assert(round==100,$"Evidence reference resolution smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
