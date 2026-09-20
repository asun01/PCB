using Asun.Device.Impl;
using Asun.Platform.Pipeline;
using Asun.Platform.PipelineProductionIntegration;
using Asun.Program.Core;
using Asun.Production.Runtime;
using Asun.Release.Core;

public static class ProductionPipelineReleaseHandoffHundredStageSmoke
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
            Guid.Parse("AF000000-0000-0000-0000-000000000001"),
            "PipelineReleaseProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(Guid.Parse("B0000000-0000-0000-0000-000000000001"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>()),
                new ProgramStep(Guid.Parse("B0000000-0000-0000-0000-000000000002"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>())
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(
            new[]
            {
                new PipelineStage<CapturedFrame>(1,"Acquire",frame=>frame),
                new PipelineStage<CapturedFrame>(2,"Measure",frame=>frame)
            });
        var definition=new ProductionSessionDefinition(
            Guid.Parse("B1000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var production=await ProductionSessionRuntime.RunAsync(
            definition,
            new SimulatedFrameSource(4,4));
        var audit=ProductionPipelineReplayAuditRuntime.Create(definition,production);
        var manifest=ReleaseManifestRuntime.Create(
            new ReleaseIdentity("Asun PCB",new Version(1,0,0),"stable"),
            new[]
            {
                new ReleaseArtifact("pipeline/replay.logical",new string('a',64),32)
            });
        var handoff=ProductionPipelineReleaseHandoffRuntime.Create(audit,manifest);
        var tampered=handoff with {PipelineReplayFingerprint=new string('b',64)};
        var manifestTampered=manifest with {Fingerprint=new string('c',64)};

        for(var i=0;i<10;i++) Check(production.FrameCount==2,"Production should retain two pipeline frames.");
        for(var i=0;i<10;i++) Check(audit.Frames.Count==2,"Replay audit should retain two frames.");
        for(var i=0;i<10;i++) Check(handoff.SessionId==audit.SessionId,"Handoff should preserve Production session identity.");
        for(var i=0;i<10;i++) Check(handoff.ProgramFingerprint==audit.ProgramFingerprint,"Handoff should preserve Program identity.");
        for(var i=0;i<10;i++) Check(handoff.PipelineReplayFingerprint==audit.Fingerprint,"Handoff should preserve replay identity.");
        for(var i=0;i<10;i++) Check(handoff.ReleaseManifestFingerprint==manifest.Fingerprint,"Handoff should preserve logical Release manifest identity.");
        for(var i=0;i<10;i++) Check(handoff.ReleaseReady==ReleaseReadinessRuntime.Evaluate(manifest).Ready,"Handoff readiness should remain a manifest fact.");
        for(var i=0;i<10;i++) Check(ProductionPipelineReleaseHandoffRuntime.IsValid(audit,manifest,handoff),"Pipeline Release handoff should validate.");
        for(var i=0;i<10;i++) Check(!ProductionPipelineReleaseHandoffRuntime.IsValid(audit,manifest,tampered),"Replay identity tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionPipelineReleaseHandoffRuntime.IsValid(audit,manifestTampered,handoff),"Manifest identity tampering should be rejected.");

        assert(round==100,$"Pipeline Release handoff smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
