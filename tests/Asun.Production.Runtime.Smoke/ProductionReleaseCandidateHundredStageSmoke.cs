using Asun.Device.Impl;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;
using Asun.Release.Core;

public static class ProductionReleaseCandidateHundredStageSmoke
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
            Guid.Parse("50000000-0000-0000-0000-000000000001"),
            "ReleaseProductionProgram",
            new Version(1,1,0),
            new[]{
                new ProgramStep(
                    Guid.Parse("51000000-0000-0000-0000-000000000001"),
                    1,
                    ProgramStepKind.Acquire,
                    "Acquire",
                    Array.Empty<ProgramParameter>()),
                new ProgramStep(
                    Guid.Parse("51000000-0000-0000-0000-000000000002"),
                    2,
                    ProgramStepKind.Measure,
                    "Measure",
                    Array.Empty<ProgramParameter>())
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(new[]{
            new PipelineStage<CapturedFrame>(1,"Acquire",frame=>frame),
            new PipelineStage<CapturedFrame>(2,"Measure",frame=>frame)
        });
        var definition=new ProductionSessionDefinition(
            Guid.Parse("52000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var report=await ProductionSessionRuntime.RunAsync(
            definition,
            new SimulatedFrameSource(4,4));
        var manifest=ProductionReleaseCandidateRuntime.Create(
            new ReleaseIdentity("Asun PCB",new Version(1,1,0),"stable"),
            definition,
            report,
            $"production/{report.SessionId:N}.report");
        var invalid=manifest with {Fingerprint=new string('f',64)};

        for(var i=0;i<10;i++) Check(ProductionSessionValidationRuntime.IsValid(definition,report),"Production source report should validate.");
        for(var i=0;i<10;i++) Check(ReleaseManifestValidationRuntime.IsValid(manifest),"Production release manifest should validate.");
        for(var i=0;i<10;i++) Check(ProductionReleaseCandidateValidationRuntime.IsValid(manifest,definition,report),"Production release candidate should validate.");
        for(var i=0;i<10;i++) Check(manifest.Artifacts.Count==1,"Release candidate should contain one logical artifact.");
        for(var i=0;i<10;i++) Check(manifest.Artifacts[0].ByteLength>0,"Logical release artifact should have non-zero canonical byte length.");
        for(var i=0;i<10;i++) Check(manifest.Artifacts[0].Sha256.Length==64,"Logical release artifact should use SHA-256.");
        for(var i=0;i<10;i++) Check(ReleaseReadinessRuntime.Evaluate(manifest).Ready,"Production release candidate should be release-ready.");
        for(var i=0;i<10;i++) Check(!ProductionReleaseCandidateValidationRuntime.IsValid(invalid,definition,report),"Tampered release candidate should be rejected.");
        for(var i=0;i<10;i++) Check(manifest.Fingerprint==ReleaseManifestRuntime.Create(manifest.Identity,manifest.Artifacts).Fingerprint,"Release candidate manifest should be deterministic.");
        for(var i=0;i<10;i++) Check(manifest.Artifacts[0].Path.StartsWith("production/",StringComparison.Ordinal),"Release artifact path should identify the production report projection.");

        assert(round==100,$"Production release candidate smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
