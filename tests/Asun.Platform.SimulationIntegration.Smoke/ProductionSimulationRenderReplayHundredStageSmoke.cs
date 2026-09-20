using Asun.Platform.RenderIntegration;
using Asun.Platform.SimulationIntegration;

public static class ProductionSimulationRenderReplayHundredStageSmoke
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
        var tampered=replay
            .Select(frame=>frame.Sequence==1
                ? frame with {SimulationObservationFingerprint=new string('c',64)}
                : frame)
            .ToArray();
        var shifted=replay
            .Select(frame=>frame.Sequence==2
                ? frame with {Sequence=3}
                : frame)
            .ToArray();
        var missing=replay.Skip(1).ToArray();
        var duplicateSimulation=simulation
            .Select(frame=>frame with {SimulationSequence=1})
            .ToArray();
        var invalidRender=render
            .Select(frame=>frame.Sequence==2 ? frame with {RenderFingerprint="bad"} : frame)
            .ToArray();

        for(var i=0;i<10;i++) Check(simulation.Length==2,"Simulation replay input should contain two frames.");
        for(var i=0;i<10;i++) Check(render.Length==2,"Render replay input should contain two frames.");
        for(var i=0;i<10;i++) Check(replay.Count==2,"Simulation/Render replay should contain two aligned frames.");
        for(var i=0;i<10;i++) Check(replay[0].Sequence==1 && replay[1].Sequence==2,"Replay sequence should be deterministic.");
        for(var i=0;i<10;i++) Check(replay[0].SimulationObservationFingerprint==simulation[0].SimulationObservationFingerprint,"First simulation fingerprint should be preserved.");
        for(var i=0;i<10;i++) Check(replay[1].RenderFingerprint==render[1].RenderFingerprint,"Second render fingerprint should be preserved.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderReplayRuntime.IsValid(simulation,render,replay),"Simulation/Render replay should validate.");
        for(var i=0;i<10;i++) Check(ProductionSimulationRenderReplayRuntime.IsValid(simulation,render,replay.Reverse().ToArray()),"Replay ordering should be canonicalized.");
        for(var i=0;i<10;i++) Check(!ProductionSimulationRenderReplayRuntime.IsValid(simulation,render,tampered),"Simulation fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionSimulationRenderReplayRuntime.IsValid(simulation,render,shifted),"Simulation/Render sequence drift should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionSimulationRenderReplayRuntime.IsValid(duplicateSimulation,render,replay),"Duplicate simulation sequence should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionSimulationRenderReplayRuntime.IsValid(simulation,invalidRender,replay),"Invalid render fingerprint should be rejected.");

        assert(round==100,$"Simulation Render replay smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
