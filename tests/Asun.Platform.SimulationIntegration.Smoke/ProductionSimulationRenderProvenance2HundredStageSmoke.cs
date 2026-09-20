using Asun.Platform.RenderIntegration;
using Asun.Platform.SimulationIntegration;

public static class ProductionSimulationRenderProvenance2HundredStageSmoke
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
        var bad=render.Select(frame=>frame.Sequence==2 ? frame with {ProductionInputFingerprint="wrong"} : frame).ToArray();
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.Validate(simulation,render,provenance).Count==0,"Baseline source alignment should validate.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.Validate(simulation,bad,provenance).Count>0,"Render input-fingerprint drift should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.Validate(simulation,render,provenance.Select(x=>x with {ProductionInputFingerprint="wrong"}).ToArray()).Count>0,"Provenance input tampering should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.Validate(simulation,render,provenance.Select(x=>x with {ProductionSequence=9}).ToArray()).Count>0,"Production sequence tampering should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.Validate(simulation,render,provenance.Select(x=>x with {SimulationSequence=9}).ToArray()).Count>0,"Simulation sequence tampering should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.Validate(simulation,render,provenance.Select(x=>x with {RenderFingerprint="bad"}).ToArray()).Count>0,"Render fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.CreateFingerprint(provenance).Length==64,"Provenance fingerprint should be SHA-256 width.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.IsEquivalent(provenance,provenance),"Provenance should be equivalent to itself.");
        for(var i=0;i<10;i++) Check(!ProductionSimulationRenderProvenanceRuntime.IsEquivalent(provenance,provenance.Select(x=>x with {RenderFingerprint="bad"}).ToArray()),"Fingerprint mutation should break equivalence.");
        assert(round==100,$"ProductionSimulationRenderProvenance2HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
