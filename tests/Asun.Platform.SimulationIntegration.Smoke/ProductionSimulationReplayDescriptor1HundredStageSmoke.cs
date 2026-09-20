using Asun.Platform.SimulationIntegration;
using Asun.Simulation.Core;

public static class ProductionSimulationReplayDescriptor1HundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var board=new Asun.Domain.Pcb.PcbBoardDefinition(Guid.Parse("75000000-0000-0000-0000-000000000001"),"SimulationReplayBoard",100,80,4);
        var component=new Asun.Domain.Pcb.PcbComponentReference(Asun.Domain.Pcb.PcbFeatureId.Create("SIM-R1"),"R1","10k","0402",0,Asun.Domain.Pcb.PcbLayerSide.Top,new Asun.Domain.Pcb.PcbCoordinate(20,30),0);
        var assembly=Asun.Domain.Pcb.PcbAssemblySnapshotRuntime.Create(board,new[]{component});
        var scenario=SimulationScenarioRuntime.Create(assembly,42);
        var observations=SimulationSessionRuntime.Run(scenario,2);
        var program=new Asun.Program.Core.InspectionProgram(Guid.Parse("76000000-0000-0000-0000-000000000001"),"SimulationReplayDescriptorProgram",new Version(1,0,0),new[]{new Asun.Program.Core.ProgramStep(Guid.Parse("77000000-0000-0000-0000-000000000001"),1,Asun.Program.Core.ProgramStepKind.Acquire,"Acquire",Array.Empty<Asun.Program.Core.ProgramParameter>())});
        var plan=Asun.Program.Core.ProgramExecutionPlanRuntime.Create(program);
        var pipeline=Asun.Platform.Pipeline.PipelineDefinitionRuntime.Create(new[]{new Asun.Platform.Pipeline.PipelineStage<Asun.Device.Contracts.CapturedFrame>(1,"Acquire",frame=>frame)});
        var definition=new Asun.Production.Runtime.ProductionSessionDefinition(Guid.Parse("78000000-0000-0000-0000-000000000001"),plan,pipeline,2);
        var production=await Asun.Production.Runtime.ProductionSessionRuntime.RunAsync(definition,new Asun.Device.Impl.SimulatedFrameSource(4,4));
        var binding=ProductionSimulationReplayBindingRuntime.Create(production,observations);
        var descriptor=ProductionSimulationReplayDescriptorRuntime.Create(production,observations,binding);
        for(var i=0;i<10;i++) Check(descriptor.ProductionSessionId==production.SessionId,"Replay descriptor should preserve production session identity.");
        for(var i=0;i<10;i++) Check(descriptor.ProductionFingerprint==production.Fingerprint,"Replay descriptor should preserve production fingerprint.");
        for(var i=0;i<10;i++) Check(descriptor.FrameCount==2,"Replay descriptor should preserve frame count.");
        for(var i=0;i<10;i++) Check(descriptor.BindingFingerprint==binding.Fingerprint,"Replay descriptor should preserve binding identity.");
        for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.Length==64,"Replay descriptor fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(ProductionSimulationReplayDescriptorRuntime.IsValid(production,observations,binding,descriptor),"Canonical simulation replay descriptor should validate.");
        for(var i=0;i<10;i++) Check(ProductionSimulationReplayDescriptorRuntime.IsEquivalent(descriptor,descriptor),"Descriptor should be equivalent to itself.");
        for(var i=0;i<10;i++) Check(ProductionSimulationReplayDescriptorRuntime.Create(production,observations,binding).DescriptorFingerprint==descriptor.DescriptorFingerprint,"Descriptor creation should be deterministic.");
        for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.All(Uri.IsHexDigit),"Descriptor fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(observations.Count==descriptor.FrameCount,"Simulation observation count should align with descriptor frame count.");
        assert(round==100,$"ProductionSimulationReplayDescriptor1HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
