using Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var third=new EvidenceDescriptor(
            EvidenceHandle.Create("frame://003"),
            EvidenceKind.Image,
            "image/raw",
            30,
            "three");
        var first=new EvidenceDescriptor(
            EvidenceHandle.Create("frame://001"),
            EvidenceKind.Image,
            "image/raw",
            10,
            "one");
        var second=new EvidenceDescriptor(
            EvidenceHandle.Create("frame://002"),
            EvidenceKind.Text,
            "text/plain",
            20,
            "two");

        var snapshot=EvidenceCatalogSnapshotRuntime.Create(
            new[]{third,first,second});
        var duplicate=new EvidenceCatalogSnapshot(new[]{first,first});

        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotValidationRuntime.IsValid(snapshot),"Snapshot validation should pass.");
        for(var i=0;i<10;i++) Check(snapshot.Count==3,"Snapshot count should remain three.");
        for(var i=0;i<10;i++) Check(snapshot.Descriptors[0].Handle.Value=="frame://001","Snapshot ordering should start with frame://001.");
        for(var i=0;i<10;i++) Check(snapshot.Descriptors[1].Handle.Value=="frame://002","Snapshot ordering should continue with frame://002.");
        for(var i=0;i<10;i++) Check(snapshot.Descriptors[2].Handle.Value=="frame://003","Snapshot ordering should end with frame://003.");
        for(var i=0;i<10;i++) Check(snapshot.Find(EvidenceHandle.Create("frame://002"))==second,"Snapshot lookup should return the requested descriptor.");
        for(var i=0;i<10;i++) Check(snapshot.Find(EvidenceHandle.Create("frame://999")) is null,"Missing snapshot lookup should remain empty.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogSnapshotValidationRuntime.IsValid(duplicate),"Duplicate snapshot handles should be rejected.");
        for(var i=0;i<10;i++) Check(snapshot.Descriptors.Select(item=>item.Handle).Distinct().Count()==3,"Snapshot handles should remain unique.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotRuntime.Create(new[]{third,first,second}).Descriptors.SequenceEqual(snapshot.Descriptors),"Snapshot determinism should remain stable.");

        assert(round==100,$"Evidence catalog snapshot smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
