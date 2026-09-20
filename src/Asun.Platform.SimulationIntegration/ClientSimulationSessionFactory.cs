using Asun.Device.Contracts;
using Asun.Device.Impl;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;
using Asun.Release.Core;

namespace Asun.Platform.SimulationIntegration;

public static class ClientSimulationSessionFactory
{
    public static ProductionSessionDefinition CreateDefinition(
        Guid? sessionId=null,
        int frameCount=3)
    {
        if(frameCount<=0)
            throw new ArgumentOutOfRangeException(nameof(frameCount));

        var program=new InspectionProgram(
            Guid.Parse("73000000-0000-0000-0000-000000000001"),
            "ClientSimulationProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(
                    Guid.Parse("73000000-0000-0000-0000-000000000101"),
                    1,
                    ProgramStepKind.Acquire,
                    "Acquire",
                    new[]{new ProgramParameter("mode","simulation")}),
                new ProgramStep(
                    Guid.Parse("73000000-0000-0000-0000-000000000102"),
                    2,
                    ProgramStepKind.Measure,
                    "Measure",
                    new[]{new ProgramParameter("unit","mm")}),
                new ProgramStep(
                    Guid.Parse("73000000-0000-0000-0000-000000000103"),
                    3,
                    ProgramStepKind.Inspect,
                    "Inspect",
                    new[]{new ProgramParameter("mode","simulation")})
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
                frame=>frame),
            new PipelineStage<CapturedFrame>(
                3,
                "Inspect",
                frame=>frame)
        });

        return new ProductionSessionDefinition(
            sessionId ?? Guid.Parse("73000000-0000-0000-0000-000000000201"),
            plan,
            pipeline,
            frameCount);
    }

    public static IFrameSource CreateSource()=>
        new SimulatedFrameSource(64,64);

    public static ReleaseManifest CreateReleaseManifest()=>
        new(
            new ReleaseIdentity(
                "Asun PCB Simulation",
                new Version(1,0,0),
                "simulation"),
            new[]
            {
                new ReleaseArtifact(
                    "simulation/client-release.pcb",
                    new string('a',64),
                    2048)
            },
            new string('b',64));
}
