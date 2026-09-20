using Asun.Platform.Evidence;
using Asun.Platform.RenderIntegration;

public static class ProductionRenderEvidenceReplayDescriptor3HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var summaries=new[]{
            new Asun.UI.Viewports.ViewportRenderFrameSummary(1,2,1,1,0,0,1,0),
            new Asun.UI.Viewports.ViewportRenderFrameSummary(2,3,1,1,1,1,2,1)};
        var frames=new[]{
            new ProductionRenderReplayFrameIntegrity(1,"input-a",summaries[0],ViewportRenderFrameFingerprintRuntime.CreateFingerprint(summaries[0])),
            new ProductionRenderReplayFrameIntegrity(2,"input-b",summaries[1],ViewportRenderFrameFingerprintRuntime.CreateFingerprint(summaries[1]))};
        var handles=new[]{EvidenceHandle.Create("render/frame/1"),EvidenceHandle.Create("render/frame/2")};
        var descriptors=ProductionRenderEvidenceReplayDescriptorRuntime.Create(frames,handles);
        var missing=descriptors.Skip(1).ToArray();
        var duplicate=descriptors.Concat(new[]{descriptors[0]}).ToArray();
        var malformed=descriptors.Select(item=>item.Sequence==2?item with {DescriptorFingerprint="bad"}:item).ToArray();
        for(var i=0;i<10;i++) Check(!ProductionRenderEvidenceReplayDescriptorRuntime.IsValid(frames,missing),"Missing descriptor should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionRenderEvidenceReplayDescriptorRuntime.IsValid(frames,duplicate),"Duplicate descriptor should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionRenderEvidenceReplayDescriptorRuntime.IsValid(frames,malformed),"Malformed descriptor fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.Validate(frames,missing).Count>0,"Missing descriptor should produce diagnostics.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.Validate(frames,duplicate).Count>0,"Duplicate descriptor should produce diagnostics.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.Validate(frames,malformed).Count>0,"Malformed fingerprint should produce diagnostics.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.IsValid(frames,descriptors),"Baseline descriptor should remain valid.");
        for(var i=0;i<10;i++) Check(descriptors.Count==2,"Baseline descriptor count should remain stable.");
        for(var i=0;i<10;i++) Check(descriptors.All(item=>item.Sequence>0),"Baseline descriptor sequences should remain positive.");
        assert(round==100,$"ProductionRenderEvidenceReplayDescriptor3HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
