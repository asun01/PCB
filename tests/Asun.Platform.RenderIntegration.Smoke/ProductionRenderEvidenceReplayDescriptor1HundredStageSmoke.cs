using Asun.Platform.Evidence;
using Asun.Platform.RenderIntegration;

public static class ProductionRenderEvidenceReplayDescriptor1HundredStageSmoke
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
        for(var i=0;i<10;i++) Check(descriptors.Count==2,"Replay descriptor count should match render frames.");
        for(var i=0;i<10;i++) Check(descriptors[0].Sequence==1 && descriptors[1].Sequence==2,"Replay descriptor sequence should be canonical.");
        for(var i=0;i<10;i++) Check(descriptors[0].RenderFingerprint==frames[0].RenderFingerprint,"First render identity should be preserved.");
        for(var i=0;i<10;i++) Check(descriptors[1].EvidenceHandle==handles[1],"Second opaque Evidence identity should be preserved.");
        for(var i=0;i<10;i++) Check(descriptors.All(item=>item.DescriptorFingerprint.Length==64),"Descriptor fingerprints should be fixed width.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.IsValid(frames,descriptors),"Canonical render/evidence descriptors should validate.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.IsEquivalent(descriptors,descriptors.Reverse().ToArray()),"Descriptor ordering should be canonical.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReplayDescriptorRuntime.Create(frames,handles)[0].DescriptorFingerprint==descriptors[0].DescriptorFingerprint,"Descriptor creation should be deterministic.");
        for(var i=0;i<10;i++) Check(descriptors.All(item=>item.EvidenceHandle.IsValid),"Opaque Evidence handles should remain valid.");
        for(var i=0;i<10;i++) Check(descriptors.All(item=>item.DescriptorFingerprint.All(Uri.IsHexDigit)),"Descriptor fingerprints should be hexadecimal.");
        assert(round==100,$"ProductionRenderEvidenceReplayDescriptor1HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
