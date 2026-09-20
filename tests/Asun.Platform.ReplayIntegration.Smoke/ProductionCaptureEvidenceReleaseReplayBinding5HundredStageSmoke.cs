using Asun.Device.Contracts;
using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Production.Runtime;
using Asun.Release.Core;

namespace Asun.Platform.ReplayIntegration.Smoke;

public static class ProductionCaptureEvidenceReleaseReplayBinding5HundredStageSmoke
{
    static ProductionCaptureEvidenceReplayBinding Replay()
{
    const report=new ProductionSessionReport(
        Guid.Parse("00000000-0000-0000-0000-000000000101"),
        "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd",
        1,
        new[]
        {
            new ProductionFrameExecution(
                FrameSequence.Create(1),
                "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                new PipelineExecutionReport(
                    1,
                    new[]{"stage-1"},
                    "1111111111111111111111111111111111111111111111111111111111111111"))
        },
        "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
    var refs=new[]
    {
        new ProductionCaptureEvidenceFrameReference(
            1,
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            640,
            480,
            "Gray8",
            new[]{EvidenceHandle.Create("evidence/001")})
    };
    return ProductionCaptureEvidenceReplayBindingRuntime.Create(report,refs);
}
static ReleaseManifest Manifest() =>
    ReleaseManifestRuntime.Create(
        new ReleaseIdentity("AsunPCB",new Version(1,0,0),"production"),
        new[]
        {
            new ReleaseArtifact(
                "artifacts/pcb-replay.json",
                "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff",
                123)
        });

    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group0=0;group0<10;group0++)
        {
            round++;
            var replay=Replay(); var manifest=Manifest(); var binding=ProductionCaptureEvidenceReleaseReplayBindingRuntime.Create(replay,manifest); var tampered=binding with { ProductionSessionId=Guid.Parse("00000000-0000-0000-0000-000000000202") }; Check(!ProductionCaptureEvidenceReleaseReplayBindingRuntime.IsValid(replay,manifest,tampered) && round==100,"Production session identity drift must be rejected");
        }
        for(var group1=0;group1<10;group1++)
        {
            round++;
            var replay=Replay(); var manifest=Manifest(); var binding=ProductionCaptureEvidenceReleaseReplayBindingRuntime.Create(replay,manifest); var tampered=binding with { ProductionSessionId=Guid.Parse("00000000-0000-0000-0000-000000000202") }; Check(!ProductionCaptureEvidenceReleaseReplayBindingRuntime.IsValid(replay,manifest,tampered) && round==100,"Production session identity drift must be rejected");
        }
        for(var group2=0;group2<10;group2++)
        {
            round++;
            var replay=Replay(); var manifest=Manifest(); var binding=ProductionCaptureEvidenceReleaseReplayBindingRuntime.Create(replay,manifest); var tampered=binding with { ProductionSessionId=Guid.Parse("00000000-0000-0000-0000-000000000202") }; Check(!ProductionCaptureEvidenceReleaseReplayBindingRuntime.IsValid(replay,manifest,tampered) && round==100,"Production session identity drift must be rejected");
        }
        for(var group3=0;group3<10;group3++)
        {
            round++;
            var replay=Replay(); var manifest=Manifest(); var binding=ProductionCaptureEvidenceReleaseReplayBindingRuntime.Create(replay,manifest); var tampered=binding with { ProductionSessionId=Guid.Parse("00000000-0000-0000-0000-000000000202") }; Check(!ProductionCaptureEvidenceReleaseReplayBindingRuntime.IsValid(replay,manifest,tampered) && round==100,"Production session identity drift must be rejected");
        }
        for(var group4=0;group4<10;group4++)
        {
            round++;
            var replay=Replay(); var manifest=Manifest(); var binding=ProductionCaptureEvidenceReleaseReplayBindingRuntime.Create(replay,manifest); var tampered=binding with { ProductionSessionId=Guid.Parse("00000000-0000-0000-0000-000000000202") }; Check(!ProductionCaptureEvidenceReleaseReplayBindingRuntime.IsValid(replay,manifest,tampered) && round==100,"Production session identity drift must be rejected");
        }
        for(var group5=0;group5<10;group5++)
        {
            round++;
            var replay=Replay(); var manifest=Manifest(); var binding=ProductionCaptureEvidenceReleaseReplayBindingRuntime.Create(replay,manifest); var tampered=binding with { ProductionSessionId=Guid.Parse("00000000-0000-0000-0000-000000000202") }; Check(!ProductionCaptureEvidenceReleaseReplayBindingRuntime.IsValid(replay,manifest,tampered) && round==100,"Production session identity drift must be rejected");
        }
        for(var group6=0;group6<10;group6++)
        {
            round++;
            var replay=Replay(); var manifest=Manifest(); var binding=ProductionCaptureEvidenceReleaseReplayBindingRuntime.Create(replay,manifest); var tampered=binding with { ProductionSessionId=Guid.Parse("00000000-0000-0000-0000-000000000202") }; Check(!ProductionCaptureEvidenceReleaseReplayBindingRuntime.IsValid(replay,manifest,tampered) && round==100,"Production session identity drift must be rejected");
        }
        for(var group7=0;group7<10;group7++)
        {
            round++;
            var replay=Replay(); var manifest=Manifest(); var binding=ProductionCaptureEvidenceReleaseReplayBindingRuntime.Create(replay,manifest); var tampered=binding with { ProductionSessionId=Guid.Parse("00000000-0000-0000-0000-000000000202") }; Check(!ProductionCaptureEvidenceReleaseReplayBindingRuntime.IsValid(replay,manifest,tampered) && round==100,"Production session identity drift must be rejected");
        }
        for(var group8=0;group8<10;group8++)
        {
            round++;
            var replay=Replay(); var manifest=Manifest(); var binding=ProductionCaptureEvidenceReleaseReplayBindingRuntime.Create(replay,manifest); var tampered=binding with { ProductionSessionId=Guid.Parse("00000000-0000-0000-0000-000000000202") }; Check(!ProductionCaptureEvidenceReleaseReplayBindingRuntime.IsValid(replay,manifest,tampered) && round==100,"Production session identity drift must be rejected");
        }
        for(var group9=0;group9<10;group9++)
        {
            round++;
            var replay=Replay(); var manifest=Manifest(); var binding=ProductionCaptureEvidenceReleaseReplayBindingRuntime.Create(replay,manifest); var tampered=binding with { ProductionSessionId=Guid.Parse("00000000-0000-0000-0000-000000000202") }; Check(!ProductionCaptureEvidenceReleaseReplayBindingRuntime.IsValid(replay,manifest,tampered) && round==100,"Production session identity drift must be rejected");
        }
        return Task.CompletedTask;
    }
}
