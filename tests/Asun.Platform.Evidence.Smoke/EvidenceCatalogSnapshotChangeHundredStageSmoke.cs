using Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotChangeHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        static EvidenceDescriptor Descriptor(string handle,long length)=>
            new(EvidenceHandle.Create(handle),EvidenceKind.Data,"application/json",length,handle);

        var previous=EvidenceCatalogSnapshotRuntime.Create(new[]{
            Descriptor("frame://001",10),
            Descriptor("frame://002",20)
        });
        var current=EvidenceCatalogSnapshotRuntime.Create(new[]{
            Descriptor("frame://001",11),
            Descriptor("frame://002",20)
        });
        var changes=EvidenceCatalogSnapshotChangeRuntime.Create(previous,current);
        var invalid=new[]{changes[0] with {CurrentDescriptorFingerprint=changes[0].PreviousDescriptorFingerprint}};

        for(var i=0;i<10;i++) Check(changes.Count==1,"Snapshot change list should contain one changed descriptor.");
        for(var i=0;i<10;i++) Check(changes[0].Handle.Value=="frame://001","Changed descriptor handle should be frame://001.");
        for(var i=0;i<10;i++) Check(changes[0].PreviousDescriptorFingerprint.Length==64,"Previous descriptor fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(changes[0].CurrentDescriptorFingerprint.Length==64,"Current descriptor fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(changes[0].PreviousDescriptorFingerprint!=changes[0].CurrentDescriptorFingerprint,"Changed descriptor fingerprints should differ.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotChangeValidationRuntime.IsValid(changes),"Snapshot change list should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogSnapshotChangeValidationRuntime.IsValid(invalid),"A non-changing fingerprint pair should be rejected.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotChangeRuntime.Create(previous,current).SequenceEqual(changes),"Snapshot change creation should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotChangeRuntime.Create(current,current).Count==0,"Identical snapshots should have no descriptor changes.");
        for(var i=0;i<10;i++) Check(changes.All(change=>change.Handle.IsValid),"All reported change handles should be valid.");

        assert(round==100,$"Evidence snapshot change smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
