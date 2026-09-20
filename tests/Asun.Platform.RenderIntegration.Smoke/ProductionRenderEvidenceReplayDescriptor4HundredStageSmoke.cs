using Asun.Platform.Evidence;
using Asun.Platform.RenderIntegration;

public static class ProductionRenderEvidenceReplayDescriptor4HundredStageSmoke
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
        var reversed=descriptors.Reverse().ToArray();
        var reorderedHandles=new[]{handles[1],handles[0]};
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.IsEquivalent(descriptors,reversed),"Descriptor order should not affect equivalence.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.IsEquivalent(descriptors,ProductionRenderEvidenceReplayDescriptorRuntime.Create(frames,reorderedHandles))==false,"Changed frame-handle association should change replay identity.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.IsValid(frames,descriptors),"Baseline descriptors should validate.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.Create(frames,handles).SequenceEqual(descriptors),"Repeated creation should preserve canonical descriptor sequence.");
        for(var i=0;i<10;i++) Check(descriptors[0].DescriptorFingerprint!=descriptors[1].DescriptorFingerprint,"Distinct render/evidence frames should have distinct descriptor fingerprints.");
        for(var i=0;i<10;i++) Check(descriptors[0].Sequence<descriptors[1].Sequence,"Descriptor sequences should be ordered.");
        for(var i=0;i<10;i++) Check(descriptors[0].EvidenceHandle.Value!="","First Evidence handle should remain non-empty.");
        for(var i=0;i<10;i++) Check(descriptors[1].EvidenceHandle.Value!="","Second Evidence handle should remain non-empty.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.IsEquivalent(descriptors,descriptors),"Descriptor self-equivalence should hold.");
for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.IsValid(frames,descriptors),"Baseline render/evidence descriptors should remain valid.");

        assert(round==100,$"ProductionRenderEvidenceReplayDescriptor4HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
