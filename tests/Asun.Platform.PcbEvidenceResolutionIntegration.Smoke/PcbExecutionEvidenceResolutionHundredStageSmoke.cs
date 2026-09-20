using Asun.Platform.Evidence;
using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.PcbEvidenceResolutionIntegration;
using Asun.Production.Runtime;

public static class PcbExecutionEvidenceResolutionHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var snapshot=new PcbExecutionSnapshot(
            new string('a',64),
            Guid.Parse("B8000000-0000-0000-0000-000000000001"),
            new string('b',64),
            new string('c',64),
            2,
            2,
            Guid.Parse("B9000000-0000-0000-0000-000000000001"),
            new string('d',64),
            new string('e',64));
        var evidence=new ProductionEvidenceReferenceProjection(
            snapshot.ProductionSessionId,
            snapshot.ProductionFingerprint,
            new[]
            {
                new ProductionEvidenceFrameReference(1,new[]{EvidenceHandle.Create("catalog/a")}),
                new ProductionEvidenceFrameReference(2,new[]{EvidenceHandle.Create("catalog/b"),EvidenceHandle.Create("catalog/missing")})
            },
            new string('f',64));
        var catalog=EvidenceCatalogSnapshotRuntime.Create(
            new[]
            {
                new EvidenceDescriptor(EvidenceHandle.Create("catalog/a"),EvidenceKind.Image,"image/png",10,"A"),
                new EvidenceDescriptor(EvidenceHandle.Create("catalog/b"),EvidenceKind.Image,"image/png",20,"B")
            });
        var resolution=PcbExecutionEvidenceResolutionRuntime.Resolve(snapshot,evidence,catalog);
        var tampered=resolution with
        {
            MissingHandles=new[]{EvidenceHandle.Create("catalog/b")}
        };
        var reordered=resolution with
        {
            RequestedHandles=resolution.RequestedHandles.Reverse().ToArray()
        };

        for(var i=0;i<10;i++) Check(catalog.Count==2,"Evidence catalog should contain two descriptors.");
        for(var i=0;i<10;i++) Check(resolution.RequestedHandles.Count==3,"Resolution should request three distinct handles.");
        for(var i=0;i<10;i++) Check(resolution.FoundHandles.Count==2,"Resolution should find two catalog handles.");
        for(var i=0;i<10;i++) Check(resolution.MissingHandles.Count==1,"Resolution should report one missing handle.");
        for(var i=0;i<10;i++) Check(resolution.FoundHandles.Any(handle=>handle.Value=="catalog/a"),"Resolution should include found handle A.");
        for(var i=0;i<10;i++) Check(resolution.FoundHandles.Any(handle=>handle.Value=="catalog/b"),"Resolution should include found handle B.");
        for(var i=0;i<10;i++) Check(resolution.MissingHandles.Single().Value=="catalog/missing","Resolution should identify the missing handle.");
        for(var i=0;i<10;i++) Check(PcbExecutionEvidenceResolutionValidationRuntime.IsValid(snapshot,evidence,catalog,resolution),"Evidence resolution should validate.");
        for(var i=0;i<10;i++) Check(!PcbExecutionEvidenceResolutionValidationRuntime.IsValid(snapshot,evidence,catalog,tampered),"Tampered found/missing resolution should be rejected.");
        for(var i=0;i<10;i++) Check(!PcbExecutionEvidenceResolutionValidationRuntime.IsValid(snapshot,evidence,catalog,reordered),"Non-canonical requested handle ordering should be rejected.");

        assert(round==100,$"PCB execution evidence resolution smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
