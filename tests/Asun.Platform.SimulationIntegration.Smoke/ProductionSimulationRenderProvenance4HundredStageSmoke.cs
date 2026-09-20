using Asun.Platform.RenderIntegration;
using Asun.Platform.SimulationIntegration;

public static class ProductionSimulationRenderProvenance4HundredStageSmoke
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
        var shortened=provenance.Skip(1).ToArray();
        var duplicateProvenance=provenance.Concat(new[]{provenance[0]}).ToArray();
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.Validate(simulation,render,shortened).Count>0,"Missing provenance frame should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.Validate(simulation,render,duplicateProvenance).Count>0,"Duplicate provenance frame should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.IsEquivalent(provenance,shortened)==false,"Truncated provenance should not be equivalent.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.CreateFingerprint(provenance)!=ProductionSimulationRenderProvenanceRuntime.CreateFingerprint(shortened),"Truncation should change fingerprint.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.CreateReplayDescriptor(provenance).Count==2,"Canonical descriptor should remain complete.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.CreateReplayDescriptor(shortened).Count==1,"Truncated descriptor should expose truncation.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.Validate(simulation,render,provenance).Count==0,"Valid provenance should remain valid after negative cases.");
        for(var i=0;i<10;i++) Check(provenance.All(x=>x.ProductionInputFingerprint.StartsWith("input-")),"Production input identities should remain explicit.");
        for(var i=0;i<10;i++) Check(provenance.All(x=>x.SimulationObservationFingerprint.Length==64),"Simulation fingerprints should retain fixed width.");
        assert(round==100,$"ProductionSimulationRenderProvenance4HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
