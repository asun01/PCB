using Asun.Device.Contracts;
using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Production.Runtime;

namespace Asun.Platform.ReplayIntegration.Smoke;

public static class ProductionCaptureEvidenceReplayBinding2HundredStageSmoke
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
        for(var group0=0;group0<10;group0++)
        {
            round++;
            var report=Report(); var refs=References().ToArray(); refs[group0%3]=refs[i%3] with { PayloadFingerprint="ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" }; Check(ProductionCaptureEvidenceReplayBindingRuntime.Validate(report,refs).Count>0 && (group0==9 || round==100),"payload drift must invalidate replay binding");
        }
        for(var group1=0;group1<10;group1++)
        {
            round++;
            var report=Report(); var refs=References().ToArray(); refs[group1%3]=refs[i%3] with { PayloadFingerprint="ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" }; Check(ProductionCaptureEvidenceReplayBindingRuntime.Validate(report,refs).Count>0 && (group1==9 || round==100),"payload drift must invalidate replay binding");
        }
        for(var group2=0;group2<10;group2++)
        {
            round++;
            var report=Report(); var refs=References().ToArray(); refs[group2%3]=refs[i%3] with { PayloadFingerprint="ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" }; Check(ProductionCaptureEvidenceReplayBindingRuntime.Validate(report,refs).Count>0 && (group2==9 || round==100),"payload drift must invalidate replay binding");
        }
        for(var group3=0;group3<10;group3++)
        {
            round++;
            var report=Report(); var refs=References().ToArray(); refs[group3%3]=refs[i%3] with { PayloadFingerprint="ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" }; Check(ProductionCaptureEvidenceReplayBindingRuntime.Validate(report,refs).Count>0 && (group3==9 || round==100),"payload drift must invalidate replay binding");
        }
        for(var group4=0;group4<10;group4++)
        {
            round++;
            var report=Report(); var refs=References().ToArray(); refs[group4%3]=refs[i%3] with { PayloadFingerprint="ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" }; Check(ProductionCaptureEvidenceReplayBindingRuntime.Validate(report,refs).Count>0 && (group4==9 || round==100),"payload drift must invalidate replay binding");
        }
        for(var group5=0;group5<10;group5++)
        {
            round++;
            var report=Report(); var refs=References().ToArray(); refs[group5%3]=refs[i%3] with { PayloadFingerprint="ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" }; Check(ProductionCaptureEvidenceReplayBindingRuntime.Validate(report,refs).Count>0 && (group5==9 || round==100),"payload drift must invalidate replay binding");
        }
        for(var group6=0;group6<10;group6++)
        {
            round++;
            var report=Report(); var refs=References().ToArray(); refs[group6%3]=refs[i%3] with { PayloadFingerprint="ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" }; Check(ProductionCaptureEvidenceReplayBindingRuntime.Validate(report,refs).Count>0 && (group6==9 || round==100),"payload drift must invalidate replay binding");
        }
        for(var group7=0;group7<10;group7++)
        {
            round++;
            var report=Report(); var refs=References().ToArray(); refs[group7%3]=refs[i%3] with { PayloadFingerprint="ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" }; Check(ProductionCaptureEvidenceReplayBindingRuntime.Validate(report,refs).Count>0 && (group7==9 || round==100),"payload drift must invalidate replay binding");
        }
        for(var group8=0;group8<10;group8++)
        {
            round++;
            var report=Report(); var refs=References().ToArray(); refs[group8%3]=refs[i%3] with { PayloadFingerprint="ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" }; Check(ProductionCaptureEvidenceReplayBindingRuntime.Validate(report,refs).Count>0 && (group8==9 || round==100),"payload drift must invalidate replay binding");
        }
        for(var group9=0;group9<10;group9++)
        {
            round++;
            var report=Report(); var refs=References().ToArray(); refs[group9%3]=refs[i%3] with { PayloadFingerprint="ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" }; Check(ProductionCaptureEvidenceReplayBindingRuntime.Validate(report,refs).Count>0 && (group9==9 || round==100),"payload drift must invalidate replay binding");
        }
        return Task.CompletedTask;
    }
}
