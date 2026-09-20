using Asun.Platform.RenderIntegration;
using Asun.Platform.SimulationIntegration;

public static class ProductionSimulationRenderProvenance5HundredStageSmoke
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
        var changed=provenance.Select(x=>x.Sequence==null ? x : x with {SimulationObservationFingerprint=new string('z',64)}).ToArray();
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.Validate(simulation,render,provenance).Count==0,"Baseline replay provenance should validate.");
        for(var i=0;i<10;i++) Check(!ProductionSimulationRenderProvenanceRuntime.IsEquivalent(provenance,changed),"Simulation observation mutation should be observable.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.CreateFingerprint(provenance)!=ProductionSimulationRenderProvenanceRuntime.CreateFingerprint(changed),"Simulation mutation should alter fingerprint.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.CreateReplayDescriptor(changed)[0].Contains(new string('z',64)),"Replay descriptor should expose changed simulation identity.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.Validate(simulation,render,changed).Count>0,"Changed simulation identity should fail validation.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.CreateReplayDescriptor(provenance).Count==2,"Replay descriptor count should remain stable.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.CreateFingerprint(provenance).Length==64,"Baseline fingerprint width should remain stable.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.IsEquivalent(provenance,provenance.Reverse().ToArray()),"Canonical equivalence should survive ordering changes.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.CreateReplayDescriptor(provenance).SequenceEqual(ProductionSimulationRenderProvenanceRuntime.CreateReplayDescriptor(provenance.Reverse().ToArray())),"Canonical descriptors should survive ordering changes.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.Validate(simulation,render,provenance).Count==0,"Final provenance validation should remain clean.");
        assert(round==100,$"ProductionSimulationRenderProvenance5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
