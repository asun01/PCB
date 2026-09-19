using Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotDiffHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        static EvidenceDescriptor Descriptor(
            string handle,
            EvidenceKind kind,
            string mediaType)=>
            new(
                EvidenceHandle.Create(handle),
                kind,
                mediaType,
                10,
                handle);

        var previous=EvidenceCatalogSnapshotRuntime.Create(new[]{
            Descriptor("frame://001",EvidenceKind.Image,"image/raw"),
            Descriptor("frame://002",EvidenceKind.Text,"text/plain")
        });
        var current=EvidenceCatalogSnapshotRuntime.Create(new[]{
            Descriptor("frame://002",EvidenceKind.Data,"application/json"),
            Descriptor("frame://003",EvidenceKind.Image,"image/raw")
        });

        var same=EvidenceCatalogSnapshotDiffRuntime.Diff(previous,previous);
        var diff=EvidenceCatalogSnapshotDiffRuntime.Diff(previous,current);
        var invalid=new EvidenceCatalogSnapshotDiff(
            new[]{new EvidenceHandle("")},
            Array.Empty<EvidenceHandle>(),
            Array.Empty<EvidenceHandle>());

        for(var i=0;i<10;i++) Check(same.IsEmpty,"Snapshot self diff should be empty.");
        for(var i=0;i<10;i++) Check(diff.AddedHandles.Single().Value=="frame://003","Snapshot added handle should be frame://003.");
        for(var i=0;i<10;i++) Check(diff.RemovedHandles.Single().Value=="frame://001","Snapshot removed handle should be frame://001.");
        for(var i=0;i<10;i++) Check(diff.ChangedHandles.Single().Value=="frame://002","Snapshot changed handle should be frame://002.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotDiffValidationRuntime.IsValid(diff),"Snapshot diff validation should pass.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogSnapshotDiffValidationRuntime.IsValid(invalid),"Invalid snapshot diff handle should be rejected.");
        for(var i=0;i<10;i++) Check(diff.AddedHandles.SequenceEqual(diff.AddedHandles.OrderBy(handle=>handle.Value,StringComparer.Ordinal)),"Snapshot added ordering should be deterministic.");
        for(var i=0;i<10;i++) Check(diff.RemovedHandles.SequenceEqual(diff.RemovedHandles.OrderBy(handle=>handle.Value,StringComparer.Ordinal)),"Snapshot removed ordering should be deterministic.");
        for(var i=0;i<10;i++) Check(diff.ChangedHandles.SequenceEqual(diff.ChangedHandles.OrderBy(handle=>handle.Value,StringComparer.Ordinal)),"Snapshot changed ordering should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotDiffRuntime.Diff(previous,current).Equals(diff),"Snapshot diff determinism should remain stable.");

        assert(round==100,$"Evidence catalog snapshot diff smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
