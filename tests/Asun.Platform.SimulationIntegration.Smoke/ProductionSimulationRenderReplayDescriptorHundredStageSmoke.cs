using Asun.Platform.RenderIntegration;
using Asun.Platform.SimulationIntegration;

public static class ProductionSimulationRenderReplayDescriptorHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var simulation=new[]
        {
            new ProductionSimulationFrameLink(1,"input-a",1,new string('a',64)),
            new ProductionSimulationFrameLink(2,"input-b",2,new string('b',64))
        };
        var summaries=new[]
        {
            new Asun.UI.Viewports.ViewportRenderFrameSummary(1,2,1,1,0,0,1,0),
            new Asun.UI.Viewports.ViewportRenderFrameSummary(2,3,1,1,1,1,2,1)
        };
        var render=new[]
        {
            new ProductionRenderReplayFrameIntegrity(1,"input-a",summaries[0],ViewportRenderFrameFingerprintRuntime.CreateFingerprint(summaries[0])),
            new ProductionRenderReplayFrameIntegrity(2,"input-b",summaries[1],ViewportRenderFrameFingerprintRuntime.CreateFingerprint(summaries[1]))
        };
        var replay=ProductionSimulationRenderReplayRuntime.Create(simulation,render);
        var descriptor=ProductionSimulationRenderReplayDescriptorRuntime.Create(replay);
        var reversed=descriptor.Reverse().ToArray();
        var tampered=descriptor
            .Select(item=>item.Sequence==2 ? item with {RenderFingerprint=new string('z',64)} : item)
            .ToArray();

        for(var i=0;i<10;i++) Check(descriptor.Count==2,"Descriptor should contain every replay frame.");
        for(var i=0;i<10;i++) Check(descriptor[0].Sequence==1,"Descriptor should be canonically ordered.");
        for(var i=0;i<10;i++) Check(descriptor[1].Sequence==2,"Descriptor should preserve the second sequence.");
        for(var i=0;i<10;i++) Check(descriptor[0].SimulationObservationFingerprint==replay[0].SimulationObservationFingerprint,"Descriptor should preserve simulation identity.");
        for(var i=0;i<10;i++) Check(descriptor[1].RenderFingerprint==replay[1].RenderFingerprint,"Descriptor should preserve render identity.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderReplayDescriptorRuntime.IsValid(replay,descriptor),"Canonical descriptor should validate.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderReplayDescriptorRuntime.IsValid(replay,reversed),"Descriptor order should be canonicalized.");
        for(var i=0;i<10;i++) Check(!ProductionSimulationRenderReplayDescriptorRuntime.IsValid(replay,tampered),"Descriptor tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionSimulationRenderReplayDescriptorRuntime.IsValid(replay,descriptor.Skip(1).ToArray()),"Descriptor truncation should be rejected.");
        for(var i=0;i<10;i++) Check(descriptor.All(item=>item.Sequence>0 && item.SimulationObservationFingerprint.Length==64 && item.RenderFingerprint.Length==64),"Descriptor identities should retain fixed-width fingerprints.");

        assert(round==100,$"Simulation Render descriptor smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
