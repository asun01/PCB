using Asun.Device.Contracts;
using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Production.Runtime;

namespace Asun.Platform.ReplayIntegration.Smoke;

public static class ProductionCaptureEvidenceReplayBinding5HundredStageSmoke
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
        new ProductionCaptureEvidenceFrameReference(
            1,"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",640,480,"Gray8",
            new[]{EvidenceHandle.Create("evidence/001")}),
        new ProductionCaptureEvidenceFrameReference(
            2,"bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",640,480,"Gray8",
            new[]{EvidenceHandle.Create("evidence/002")}),
        new ProductionCaptureEvidenceFrameReference(
            3,"cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",640,480,"Gray8",
            new[]{EvidenceHandle.Create("evidence/003")})
    };
}

    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group=0;group<10;group++)
        {
            for(var i=0;i<10;i++)
            {
                round++;
                var report=Report();
                var refs=References();
                var binding=ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
                var tampered=binding with { ProductionFingerprint="ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" };
                var tamperedValid=ProductionCaptureEvidenceReplayBindingRuntime.IsValid(report,refs,tampered);
                Check(!tamperedValid && (group<9 || round==100),"matrix");
            }
        }

        return Task.CompletedTask;
    }
}
