using Asun.Platform.RenderIntegration;
using Asun.Platform.SimulationIntegration;

public static class ProductionSimulationRenderProvenance1HundredStageSmoke
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
        for(var i=0;i<10;i++) Check(provenance.Count==2,"Provenance count should match source frames.");
        for(var i=0;i<10;i++) Check(provenance[0].ProductionSequence==1 && provenance[1].ProductionSequence==2,"Production sequence should be preserved.");
        for(var i=0;i<10;i++) Check(provenance[0].SimulationSequence==1 && provenance[1].SimulationSequence==2,"Simulation sequence should be preserved.");
        for(var i=0;i<10;i++) Check(provenance[0].ProductionInputFingerprint=="input-a","First production input identity should be preserved.");
        for(var i=0;i<10;i++) Check(provenance[1].ProductionInputFingerprint=="input-b","Second production input identity should be preserved.");
        for(var i=0;i<10;i++) Check(provenance[0].SimulationObservationFingerprint==simulation[0].SimulationObservationFingerprint,"Simulation identity should be preserved.");
        for(var i=0;i<10;i++) Check(provenance[1].RenderFingerprint==render[1].RenderFingerprint,"Render identity should be preserved.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.IsEquivalent(provenance,provenance.Reverse().ToArray()),"Ordering should not change provenance identity.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.Validate(simulation,render,provenance).Count==0,"Canonical provenance should validate.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderProvenanceRuntime.CreateReplayDescriptor(provenance).Count==2,"Replay descriptor should cover every provenance frame.");
        assert(round==100,$"ProductionSimulationRenderProvenance1HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
