using Asun.Device.Contracts;
using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Production.Runtime;

namespace Asun.Platform.ReplayIntegration.Smoke;

public static class ProductionCaptureEvidenceReplayBinding4HundredStageSmoke
{
    static ProductionSessionReport Report()
{
    const fpA="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
    const fpB="bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";
    const fpC="cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc";
    return new ProductionSessionReport(
        Guid.Parse("00000000-0000-0000-0000-000000000101"),
        "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd",
        3,
        new[]
        {
            new ProductionFrameExecution(FrameSequence.Create(1),fpA,new PipelineExecutionReport(1,new[]{"stage-1"},"1111111111111111111111111111111111111111111111111111111111111111")),
            new ProductionFrameExecution(FrameSequence.Create(2),fpB,new PipelineExecutionReport(1,new[]{"stage-2"},"2222222222222222222222222222222222222222222222222222222222222222")),
            new ProductionFrameExecution(FrameSequence.Create(3),fpC,new PipelineExecutionReport(1,new[]{"stage-3"},"3333333333333333333333333333333333333333333333333333333333333333"))
        },
        "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
}
static IReadOnlyList<ProductionCaptureEvidenceFrameReference> References()
{
    return new[]
    {
        new ProductionCaptureEvidenceFrameReference(1,"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",640,480,"Gray8",new[]{EvidenceHandle.Create("evidence/001")}),
        new ProductionCaptureEvidenceFrameReference(2,"bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",640,480,"Gray8",new[]{EvidenceHandle.Create("evidence/002")}),
        new ProductionCaptureEvidenceFrameReference(3,"cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",640,480,"Gray8",new[]{EvidenceHandle.Create("evidence/003")})
    };
}

    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group0=0;group0<1;group0++)
        {
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
        }
        for(var group1=0;group1<1;group1++)
        {
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
        }
        for(var group2=0;group2<1;group2++)
        {
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
        }
        for(var group3=0;group3<1;group3++)
        {
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
        }
        for(var group4=0;group4<1;group4++)
        {
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
        }
        for(var group5=0;group5<1;group5++)
        {
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
        }
        for(var group6=0;group6<1;group6++)
        {
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
        }
        for(var group7=0;group7<1;group7++)
        {
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
        }
        for(var group8=0;group8<1;group8++)
        {
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
        }
        for(var group9=0;group9<1;group9++)
        {
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
            round++;
                var report=Report();
                var refs=References();
                var reordered=refs.Reverse().ToArray();
                var left=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var right=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,reordered);
                Check(ProductionCaptureEvidenceReplayBindingRuntime.IsEquivalent(left,right) &&
                      left.CaptureEvidenceProjectionFingerprint==right.CaptureEvidenceProjectionFingerprint &&
                      (round==100),"replay binding must be order-independent through canonical projection");
        }
        return Task.CompletedTask;
    }
}
