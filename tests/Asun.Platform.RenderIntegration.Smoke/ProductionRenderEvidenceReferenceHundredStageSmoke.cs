using Asun.Platform.Evidence;
using Asun.Platform.RenderIntegration;
using Asun.UI.Viewports;

public static class ProductionRenderEvidenceReferenceHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var summaries=new[]
        {
            new ViewportRenderFrameSummary(1,2,1,1,0,0,1,0),
            new ViewportRenderFrameSummary(2,3,1,1,1,1,2,1)
        };
        var productionFrames=new[]
        {
            new ProductionRenderReplayFrameIntegrity(1,"input-a",summaries[0],ViewportRenderFrameFingerprintRuntime.CreateFingerprint(summaries[0])),
            new ProductionRenderReplayFrameIntegrity(2,"input-b",summaries[1],ViewportRenderFrameFingerprintRuntime.CreateFingerprint(summaries[1]))
        };
        var handles=new[]
        {
            EvidenceHandle.Create("render/frame/1"),
            EvidenceHandle.Create("render/frame/2")
        };
        var references=ProductionRenderEvidenceReferenceRuntime.Create(productionFrames,handles);
        var tampered=references
            .Select(reference=>reference.Sequence==1
                ? reference with {RenderFingerprint=new string('a',64)}
                : reference)
            .ToArray();
        var duplicated=references
            .Select(reference=>reference.Sequence==2
                ? reference with {EvidenceHandle=references[0].EvidenceHandle}
                : reference)
            .ToArray();
        var missing=references.Skip(1).ToArray();

        for(var i=0;i<10;i++) Check(references.Count==2,"Render Evidence projection should contain two references.");
        for(var i=0;i<10;i++) Check(references[0].Sequence==1 && references[1].Sequence==2,"Render sequence should remain deterministic.");
        for(var i=0;i<10;i++) Check(references[0].EvidenceHandle.IsValid && references[1].EvidenceHandle.IsValid,"Evidence handles should remain opaque and valid.");
        for(var i=0;i<10;i++) Check(references[0].RenderFingerprint==productionFrames[0].RenderFingerprint,"First render fingerprint should be preserved.");
        for(var i=0;i<10;i++) Check(references[1].RenderFingerprint==productionFrames[1].RenderFingerprint,"Second render fingerprint should be preserved.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReferenceRuntime.IsValid(productionFrames,references),"Render Evidence projection should validate.");
        for(var i=0;i<10;i++) Check(ProductionRenderEvidenceReferenceRuntime.IsValid(productionFrames,references.Reverse().ToArray()),"Reference ordering should be canonicalized by sequence.");
        for(var i=0;i<10;i++) Check(!ProductionRenderEvidenceReferenceRuntime.IsValid(productionFrames,tampered),"Render fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionRenderEvidenceReferenceRuntime.IsValid(productionFrames,duplicated),"Duplicate opaque Evidence handles should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionRenderEvidenceReferenceRuntime.IsValid(productionFrames,missing),"Missing Render Evidence reference should be rejected.");

        assert(round==100,$"Production Render Evidence reference smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
