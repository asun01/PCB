using Asun.Device.Impl;
using Asun.Domain.Pcb;
using Asun.Platform.Pipeline;
using Asun.Platform.SimulationIntegration;
using Asun.Program.Core;
using Asun.Production.Runtime;
using Asun.Simulation.Core;

public static class ProductionSimulationReplayBindingHundredStageSmoke
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
            Guid.Parse("75000000-0000-0000-0000-000000000001"),
            "SimulationReplayBoard",
            100,
            80,
            4);
        var component=new PcbComponentReference(
            PcbFeatureId.Create("SIM-R1"),
            "R1",
            "10k",
            "0402",
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(20,30),
            0);
        var assembly=PcbAssemblySnapshotRuntime.Create(board,new[]{component});
        var scenario=SimulationScenarioRuntime.Create(assembly,42);
        var observations=SimulationSessionRuntime.Run(scenario,2);

        var program=new InspectionProgram(
            Guid.Parse("76000000-0000-0000-0000-000000000001"),
            "SimulationReplayProgram",
            new Version(1,0,0),
            new[]{
                new ProgramStep(Guid.Parse("77000000-0000-0000-0000-000000000001"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>())
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(new[]{
            new PipelineStage<Asun.Device.Contracts.CapturedFrame>(1,"Acquire",frame=>frame)
        });
        var definition=new ProductionSessionDefinition(
            Guid.Parse("78000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var production=await ProductionSessionRuntime.RunAsync(
            definition,
            new SimulatedFrameSource(4,4));

        var binding=ProductionSimulationReplayBindingRuntime.Create(production,observations);
        var tampered=binding with {ProductionFingerprint=new string('a',64)};
        var shifted=observations.ToArray();
        shifted[1]=shifted[1] with {Sequence=Asun.Device.Contracts.FrameSequence.Create(3)};

        for(var i=0;i<10;i++) Check(observations.Count==2,"Simulation session should produce two observations.");
        for(var i=0;i<10;i++) Check(production.FrameCount==2,"Production session should produce two frames.");
        for(var i=0;i<10;i++) Check(observations[0].Sequence.Value==1 && observations[1].Sequence.Value==2,"Simulation observations should be contiguous.");
        for(var i=0;i<10;i++) Check(observations.All(observation=>observation.Fingerprint.Length==64),"Simulation observations should retain SHA-256 fingerprints.");
        for(var i=0;i<10;i++) Check(binding.ProductionSessionId==production.SessionId,"Simulation binding should retain production session identity.");
        for(var i=0;i<10;i++) Check(binding.Frames.Count==2,"Simulation binding should contain one frame link per observation.");
        for(var i=0;i<10;i++) Check(binding.Frames.All(frame=>frame.ProductionSequence==frame.SimulationSequence),"Simulation binding should align production and simulation sequences.");
        for(var i=0;i<10;i++) Check(ProductionSimulationReplayBindingValidationRuntime.IsValid(production,observations,binding),"Simulation binding should validate.");
        for(var i=0;i<10;i++) Check(!ProductionSimulationReplayBindingValidationRuntime.IsValid(production,observations,tampered),"Tampered production fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionSimulationReplayBindingValidationRuntime.IsValid(production,shifted,binding),"Simulation sequence drift should be rejected.");

        assert(round==100,$"Production simulation replay binding smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
