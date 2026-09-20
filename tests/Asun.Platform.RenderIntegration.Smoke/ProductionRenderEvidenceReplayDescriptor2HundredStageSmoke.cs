using Asun.Platform.Evidence;
using Asun.Platform.RenderIntegration;

public static class ProductionRenderEvidenceReplayDescriptor2HundredStageSmoke
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
        var tampered=descriptors.Select(item=>item.Sequence==1?item with {RenderFingerprint=new string('a',64)}:item).ToArray();
        var badHandle=descriptors.Select(item=>item.Sequence==2?item with {EvidenceHandle=EvidenceHandle.Create("render/other/2")}:item).ToArray();
        for(var i=0;i<10;i++) Check(!ProductionRenderEvidenceReplayDescriptorRuntime.IsValid(frames,tampered),"Render fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionRenderEvidenceReplayDescriptorRuntime.IsValid(frames,badHandle),"Evidence identity tampering should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.Validate(frames,tampered).Count>0,"Render tampering should produce diagnostics.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.Validate(frames,badHandle).Count>0,"Evidence tampering should produce diagnostics.");
        for(var i=0;i<10;i++) Check(descriptors[0].RenderFingerprint==frames[0].RenderFingerprint,"Baseline render identity should remain stable.");
        for(var i=0;i<10;i++) Check(descriptors[1].EvidenceHandle==handles[1],"Baseline evidence identity should remain stable.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.IsValid(frames,descriptors),"Baseline descriptors should remain valid.");
        for(var i=0;i<10;i++) Check(descriptors.Count==2,"Baseline descriptor count should remain stable.");
        for(var i=0;i<10;i++) Check(descriptors.All(item=>item.DescriptorFingerprint.Length==64),"Baseline descriptor fingerprints should remain fixed width.");
for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.Create(frames,handles)[1].DescriptorFingerprint==descriptors[1].DescriptorFingerprint,"Recreated second descriptor should preserve its canonical fingerprint.");

        assert(round==100,$"ProductionRenderEvidenceReplayDescriptor2HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
