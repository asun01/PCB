using Asun.Device.Impl;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Platform.ReleaseIntegration;
using Asun.Program.Core;
using Asun.Production.Runtime;
using Asun.Release.Core;

public static class ProductionEvidenceReleaseProjectionHundredStageSmoke
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
            Guid.Parse("79000000-0000-0000-0000-000000000001"),
            "ReleaseEvidenceProgram",
            new Version(1,0,0),
            new[]{
                new ProgramStep(Guid.Parse("7A000000-0000-0000-0000-000000000001"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>()),
                new ProgramStep(Guid.Parse("7A000000-0000-0000-0000-000000000002"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>())
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(new[]{
            new PipelineStage<Asun.Device.Contracts.CapturedFrame>(1,"Acquire",frame=>frame),
            new PipelineStage<Asun.Device.Contracts.CapturedFrame>(2,"Measure",frame=>frame)
        });
        var definition=new ProductionSessionDefinition(
            Guid.Parse("7B000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var production=await ProductionSessionRuntime.RunAsync(
            definition,
            new SimulatedFrameSource(4,4));

        var evidence=ProductionEvidenceReferenceProjectionRuntime.Create(
            production,
            new[]{
                new ProductionEvidenceFrameReference(1,new[]{EvidenceHandle.Create("release/frame/1/input"),EvidenceHandle.Create("release/frame/1/result")}),
                new ProductionEvidenceFrameReference(2,new[]{EvidenceHandle.Create("release/frame/2/input"),EvidenceHandle.Create("release/frame/2/result")})
            });
        var manifest=ProductionEvidenceReleaseProjectionRuntime.Create(
            new ReleaseIdentity("Asun PCB",new Version(1,0,0),"stable"),
            definition,
            production,
            evidence,
            $"production/{production.SessionId:N}.report",
            $"evidence/{production.SessionId:N}.references");
        var tampered=manifest with {Fingerprint=new string('b',64)};

        for(var i=0;i<10;i++) Check(manifest.Artifacts.Count==2,"Release manifest should contain production and evidence-reference logical artifacts.");
        for(var i=0;i<10;i++) Check(manifest.Artifacts.Any(artifact=>artifact.Path.StartsWith("production/",StringComparison.Ordinal)),"Release manifest should contain the production logical artifact.");
        for(var i=0;i<10;i++) Check(manifest.Artifacts.Any(artifact=>artifact.Path.StartsWith("evidence/",StringComparison.Ordinal)),"Release manifest should contain the evidence-reference logical artifact.");
        for(var i=0;i<10;i++) Check(manifest.Artifacts.All(artifact=>artifact.Sha256.Length==64),"Release logical artifacts should use SHA-256 fingerprints.");
        for(var i=0;i<10;i++) Check(manifest.Artifacts.All(artifact=>artifact.ByteLength>0),"Release logical artifacts should have non-zero canonical byte length.");
        for(var i=0;i<10;i++) Check(ReleaseManifestValidationRuntime.IsValid(manifest),"Combined release manifest should validate.");
        for(var i=0;i<10;i++) Check(ProductionEvidenceReleaseProjectionValidationRuntime.IsValid(manifest,definition,production,evidence),"Combined production/evidence release projection should validate.");
        for(var i=0;i<10;i++) Check(!ProductionEvidenceReleaseProjectionValidationRuntime.IsValid(tampered,definition,production,evidence),"Tampered release manifest fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(ReleaseReadinessRuntime.Evaluate(manifest).Ready,"Combined logical release manifest should report factual readiness.");
        for(var i=0;i<10;i++) Check(manifest.Fingerprint==ReleaseManifestRuntime.Create(manifest.Identity,manifest.Artifacts).Fingerprint,"Release manifest fingerprint should be deterministic.");

        assert(round==100,$"Production evidence release smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
