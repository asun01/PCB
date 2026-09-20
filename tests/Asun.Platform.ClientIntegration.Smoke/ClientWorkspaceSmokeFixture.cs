using Asun.Device.Contracts;
using Asun.Platform.ClientIntegration;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

namespace Asun.Platform.ClientIntegration.Smoke;

internal static class ClientWorkspaceSmokeFixture
{
    public static ProductionSessionDefinition CreateDefinition(
        Guid? sessionId=null,
        int frameCount=2)
    {
        var program=new InspectionProgram(
            Guid.Parse("72000000-0000-0000-0000-000000000001"),
            "ClientSmokeProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(
                    Guid.Parse("72000000-0000-0000-0000-000000000101"),
                    1,
                    ProgramStepKind.Acquire,
                    "Acquire",
                    new[]{new ProgramParameter("source","simulated")}),
                new ProgramStep(
                    Guid.Parse("72000000-0000-0000-0000-000000000102"),
                    2,
                    ProgramStepKind.Measure,
                    "Measure",
                    new[]{new ProgramParameter("unit","mm")})
            });

        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(new[]
        {
            new PipelineStage<CapturedFrame>(
                1,
                "Acquire",
                frame=>CapturedFrame.Create(frame.Metadata,frame.Payload.ToArray())),
            new PipelineStage<CapturedFrame>(
                2,
                "Measure",
                frame=>frame)
        });

        return new ProductionSessionDefinition(
            sessionId??Guid.Parse("72000000-0000-0000-0000-000000000201"),
            plan,
            pipeline,
            frameCount);
    }

    public static ClientProductionWorkspace CreateWorkspace()=>
        new(new ProductionSessionRuntimeAdapter());
    public static Asun.Release.Core.ReleaseManifest CreateReleaseManifest(
        bool valid=true,
        Guid? versionId=null)
    {
        var identity=new Asun.Release.Core.ReleaseIdentity(
            "Asun PCB",
            new Version(1,0,0),
            "simulation");
        var artifactSha=new string(valid?'c':'Z',64);
        var artifact=new Asun.Release.Core.ReleaseArtifact(
            "simulation/release.pcb",
            artifactSha,
            1024);
        return new Asun.Release.Core.ReleaseManifest(
            identity,
            new[]{artifact},
            versionId is null
                ? new string('d',64)
                : new string('e',64));
    }

}
