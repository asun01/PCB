using Asun.Platform.Evidence;
using Asun.Platform.RenderIntegration;

public static class ProductionRenderEvidenceReplayDescriptor5HundredStageSmoke
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
        var changedFrame=frames.Select(frame=>frame.Sequence==2?frame with {RenderFingerprint=new string('b',64)}:frame).ToArray();
        var changed=ProductionRenderEvidenceReplayDescriptorRuntime.Create(changedFrame,handles);
        for(var i=0;i<10;i++) Check(changed[1].DescriptorFingerprint!=descriptors[1].DescriptorFingerprint,"Render content change should alter descriptor identity.");
        for(var i=0;i<10;i++) Check(!ProductionRenderEvidenceReplayDescriptorRuntime.IsEquivalent(descriptors,changed),"Changed render content should break descriptor equivalence.");
        for(var i=0;i<10;i++) Check(!ProductionRenderEvidenceReplayDescriptorRuntime.IsValid(frames,changed),"Changed render content should fail source validation.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.Validate(frames,changed).Count>0,"Changed render content should produce diagnostics.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.Create(frames,handles)[0].Sequence==1,"Canonical recreation should retain first sequence.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.Create(frames,handles)[1].Sequence==2,"Canonical recreation should retain second sequence.");
        for(var i=0;i<10;i++) Check(descriptors[0].DescriptorFingerprint.Length==64,"Baseline first descriptor fingerprint should remain valid.");
        for(var i=0;i<10;i++) Check(descriptors[1].DescriptorFingerprint.Length==64,"Baseline second descriptor fingerprint should remain valid.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.IsValid(frames,descriptors),"Baseline render/evidence replay should remain valid.");
for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.Create(frames,handles).Count==2,"Recreated descriptor set should preserve cardinality.");

        assert(round==100,$"ProductionRenderEvidenceReplayDescriptor5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
