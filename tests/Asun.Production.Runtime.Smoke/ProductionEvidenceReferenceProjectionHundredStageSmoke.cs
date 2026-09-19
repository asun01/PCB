using Asun.Device.Impl;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionEvidenceReferenceProjectionHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var program=new InspectionProgram(
            Guid.Parse("6C000000-0000-0000-0000-000000000001"),
            "EvidenceProjectionProgram",
            new Version(1,0,0),
            new[]{
                new ProgramStep(Guid.Parse("6D000000-0000-0000-0000-000000000001"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>()),
                new ProgramStep(Guid.Parse("6D000000-0000-0000-0000-000000000002"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>())
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(new[]{
            new PipelineStage<Asun.Device.Contracts.CapturedFrame>(1,"Acquire",frame=>frame),
            new PipelineStage<Asun.Device.Contracts.CapturedFrame>(2,"Measure",frame=>frame)
        });
        var definition=new ProductionSessionDefinition(
            Guid.Parse("6E000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var production=await ProductionSessionRuntime.RunAsync(
            definition,
            new SimulatedFrameSource(4,4));

        var references=new[]{
            new ProductionEvidenceFrameReference(
                1,
                new[]{EvidenceHandle.Create("production/frame/0001/input"),EvidenceHandle.Create("production/frame/0001/report")}),
            new ProductionEvidenceFrameReference(
                2,
                new[]{EvidenceHandle.Create("production/frame/0002/input"),EvidenceHandle.Create("production/frame/0002/report")})
        };
        var projection=ProductionEvidenceReferenceProjectionRuntime.Create(production,references);
        var tampered=projection with {ProductionFingerprint=new string('d',64)};
        var unsorted=projection with {
            Frames=new[]{
                projection.Frames[0] with {Handles=projection.Frames[0].Handles.Reverse().ToArray()},
                projection.Frames[1]
            }
        };

        for(var i=0;i<10;i++) Check(production.FrameCount==2,"Production report should contain two frames for evidence projection.");
        for(var i=0;i<10;i++) Check(projection.ProductionSessionId==production.SessionId,"Evidence projection should bind production session identity.");
        for(var i=0;i<10;i++) Check(projection.Frames.Count==2,"Evidence projection should contain one frame entry per production frame.");
        for(var i=0;i<10;i++) Check(projection.Frames[0].Sequence==1 && projection.Frames[1].Sequence==2,"Evidence projection should preserve production sequence.");
        for(var i=0;i<10;i++) Check(projection.Frames.All(frame=>frame.Handles.Count==2),"Each production frame should retain two opaque evidence handles.");
        for(var i=0;i<10;i++) Check(projection.Frames.All(frame=>frame.Handles.All(handle=>handle.IsValid)),"Evidence handles should remain valid opaque references.");
        for(var i=0;i<10;i++) Check(ProductionEvidenceReferenceProjectionValidationRuntime.IsValid(production,projection),"Evidence projection should validate.");
        for(var i=0;i<10;i++) Check(!ProductionEvidenceReferenceProjectionValidationRuntime.IsValid(production,tampered),"Tampered production fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionEvidenceReferenceProjectionValidationRuntime.IsValid(production,unsorted),"Non-canonical opaque handle ordering should be rejected.");
        for(var i=0;i<10;i++) Check(projection.Fingerprint.Length==64,"Evidence projection fingerprint should be fixed width.");

        assert(round==100,$"Production evidence projection smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
