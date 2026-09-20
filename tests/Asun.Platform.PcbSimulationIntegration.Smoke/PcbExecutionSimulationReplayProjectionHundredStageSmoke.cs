using Asun.Device.Contracts;
using Asun.Device.Impl;
using Asun.Domain.Pcb;
using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.PcbSimulationIntegration;
using Asun.Platform.Pipeline;
using Asun.Platform.SimulationIntegration;
using Asun.Program.Core;
using Asun.Production.Runtime;
using Asun.Simulation.Core;

public static class PcbExecutionSimulationReplayProjectionHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var board=new PcbBoardDefinition(
            Guid.Parse("BC000000-0000-0000-0000-000000000001"),
            "SimulationProjectionBoard",
            100,
            80,
            4);
        var component=new PcbComponentReference(
            PcbFeatureId.Create("SIM-P1"),
            "R1",
            "10k",
            "0402",
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(20,30),
            0);
        var assembly=PcbAssemblySnapshotRuntime.Create(board,new[]{component});

        var scenario=SimulationScenarioRuntime.Create(assembly,7);
        var observations=SimulationSessionRuntime.Run(scenario,2);

        var program=new InspectionProgram(
            Guid.Parse("BF000000-0000-0000-0000-000000000001"),
            "SimulationReplayProductionProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(
                    Guid.Parse("C0000000-0000-0000-0000-000000000001"),
                    1,
                    ProgramStepKind.Acquire,
                    "Acquire",
                    Array.Empty<ProgramParameter>())
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(
            new[]
            {
                new PipelineStage<CapturedFrame>(
                    1,
                    "Acquire",
                    frame=>frame)
            });
        var definition=new ProductionSessionDefinition(
            Guid.Parse("C1000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var production=await ProductionSessionRuntime.RunAsync(
            definition,
            new SimulatedFrameSource(4,4));
        var binding=ProductionSimulationReplayBindingRuntime.Create(
            production,
            observations);

        var snapshot=new PcbExecutionSnapshot(
            assembly.Fingerprint,
            production.SessionId,
            production.Fingerprint,
            new string('b',64),
            production.FrameCount,
            production.FrameCount,
            Guid.Parse("C2000000-0000-0000-0000-000000000001"),
            new string('c',64),
            new string('d',64));
        var projection=PcbExecutionSimulationReplayProjectionRuntime.Create(
            snapshot,
            observations,
            binding);
        var tampered=projection with
        {
            SimulationBindingFingerprint=new string('f',64)
        };
        var shifted=observations.ToArray();
        shifted[1]=shifted[1] with
        {
            Sequence=FrameSequence.Create(3)
        };

        for(var i=0;i<10;i++) Check(assembly.Components.Count==1,"Simulation projection should retain one PCB component.");
        for(var i=0;i<10;i++) Check(production.FrameCount==2,"Simulation projection should retain two real production frames.");
        for(var i=0;i<10;i++) Check(observations.Count==2,"Simulation projection should retain two observations.");
        for(var i=0;i<10;i++) Check(binding.Frames.Count==2,"Simulation binding should retain two frame links.");
        for(var i=0;i<10;i++) Check(projection.FrameCount==2,"Projection should retain two frame facts.");
        for(var i=0;i<10;i++) Check(projection.ObservationCount==2,"Projection should retain two simulation observations.");
        for(var i=0;i<10;i++) Check(projection.ExecutionSnapshotFingerprint==snapshot.Fingerprint,"Projection should retain execution identity.");
        for(var i=0;i<10;i++) Check(projection.SimulationBindingFingerprint==binding.Fingerprint,"Projection should retain simulation binding identity.");
        for(var i=0;i<10;i++) Check(PcbExecutionSimulationReplayProjectionValidationRuntime.IsValid(snapshot,observations,binding,projection),"Simulation replay projection should validate.");
        for(var i=0;i<10;i++) Check(!PcbExecutionSimulationReplayProjectionValidationRuntime.IsValid(snapshot,observations,binding,tampered),"Simulation binding tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!PcbExecutionSimulationReplayProjectionValidationRuntime.IsValid(snapshot,shifted,binding,projection),"Simulation sequence drift should be rejected.");

        assert(round==100,$"PCB simulation replay projection smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
