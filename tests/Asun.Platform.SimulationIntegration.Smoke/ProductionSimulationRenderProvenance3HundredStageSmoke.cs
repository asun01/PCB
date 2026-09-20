using Asun.Platform.RenderIntegration;
using Asun.Platform.SimulationIntegration;

public static class ProductionSimulationRenderProvenance3HundredStageSmoke
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
        var provenance=ProductionSimulationRenderProvenanceRuntime.Create(simulation,render);
        var duplicate=simulation.Select(frame=>frame with {SimulationSequence=1}).ToArray();
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.Validate(simulation,render,provenance).Count==0,"Canonical sequence relation should validate.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.Validate(duplicate,render,provenance).Count>0,"Duplicate simulation sequence should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.CreateReplayDescriptor(provenance)[0].StartsWith("1:input-a:1:"),"Descriptor should be canonically ordered.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.CreateReplayDescriptor(provenance)[1].StartsWith("2:input-b:2:"),"Descriptor should retain second identity.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.CreateFingerprint(provenance)==ProductionSimulationRenderProvenanceRuntime.CreateFingerprint(provenance.Reverse().ToArray()),"Fingerprint should be order-independent.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.CreateReplayDescriptor(provenance).SequenceEqual(ProductionSimulationRenderProvenanceRuntime.CreateReplayDescriptor(provenance.Reverse().ToArray())),"Descriptor should be order-independent.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.Validate(simulation,render,provenance).Count==0,"Repeated provenance validation should remain deterministic.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.IsEquivalent(provenance,provenance.ToArray()),"Equivalent copies should remain equivalent.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.CreateReplayDescriptor(provenance).All(item=>item.Length>0),"Descriptor entries should be non-empty.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.CreateFingerprint(provenance).All(char.IsAsciiHexDigit),"Fingerprint should contain hexadecimal characters.");
        assert(round==100,$"ProductionSimulationRenderProvenance3HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
