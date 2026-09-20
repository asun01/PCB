using Asun.Domain.Pcb;
using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.PcbSimulationIntegration;
using Asun.Platform.SimulationIntegration;
using Asun.Simulation.Core;

public static class PcbExecutionSimulationReplayProjectionHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
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
        var snapshot=new PcbExecutionSnapshot(
            assembly.Fingerprint,
            Guid.Parse("BD000000-0000-0000-0000-000000000001"),
            new string('a',64),
            new string('b',64),
            2,
            2,
            Guid.Parse("BE000000-0000-0000-0000-000000000001"),
            new string('c',64),
            new string('d',64));
        var binding=new ProductionSimulationReplayBinding(
            snapshot.ProductionSessionId,
            snapshot.ProductionFingerprint,
            new[]
            {
                new ProductionSimulationFrameLink(1,new string('1',64),1,observations[0].Fingerprint),
                new ProductionSimulationFrameLink(2,new string('2',64),2,observations[1].Fingerprint)
            },
            new string('e',64));
        var projection=PcbExecutionSimulationReplayProjectionRuntime.Create(snapshot,observations,binding);
        var tampered=projection with {SimulationBindingFingerprint=new string('f',64)};
        var shifted=observations.ToArray();
        shifted[1]=shifted[1] with {Sequence=Asun.Device.Contracts.FrameSequence.Create(3)};

        for(var i=0;i<10;i++) Check(assembly.Components.Count==1,"Simulation projection should retain one PCB component.");
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
        return ValueTask.CompletedTask;
    }
}
