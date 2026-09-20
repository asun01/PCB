using Asun.Device.Contracts;
using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Platform.PcbEvidenceReleaseIntegration;
using Asun.Production.Runtime;
using Asun.Release.Core;

namespace Asun.Platform.ReplayIntegration.Smoke;

public static class ProductionCaptureEvidenceReleaseAuditTraceReplayBinding3HundredStageSmoke
{
    static ProductionCaptureEvidenceReleaseReplayBinding Replay()
{
    var report=new ProductionSessionReport(
        Guid.Parse("00000000-0000-0000-0000-000000000101"),
        "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd",
        1,
        new[]
        {
            new ProductionFrameExecution(
                FrameSequence.Create(1),
                "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                new PipelineExecutionReport(1,new[]{"stage-1"},"1111111111111111111111111111111111111111111111111111111111111111"))
        },
        "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
    var refs=new[]
    {
        new ProductionCaptureEvidenceFrameReference(
            1,
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            640,480,"Gray8",
            new[]{EvidenceHandle.Create("evidence/001")})
    };
    var replay=ProductionCaptureEvidenceReplayBindingRuntime.Create(
        report,refs);
    return ProductionCaptureEvidenceReleaseReplayBindingRuntime.Create(
        replay,
        ReleaseManifestRuntime.Create(
            new ReleaseIdentity("AsunPCB",new Version(1,0,0),"production"),
            new[]
            {
                new ReleaseArtifact(
                    "artifacts/pcb-replay.json",
                    "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff",
                    123)
            }));
}
static PcbEvidenceReleaseAuditTrace AuditTrace(string manifest)
{
    const string evidence="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
    const string transition="bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";
    var qualityRunId=Guid.Parse("00000000-0000-0000-0000-000000000303");
    const int capacity=4;
    var entry=new PcbEvidenceReleaseAuditTraceEntry(
        1,evidence,manifest,transition,qualityRunId,1,1,0,true);
    var canonical=string.Join(
        "|",
        capacity,
        manifest,
        entry.Sequence,
        entry.EvidenceReleaseFactFingerprint,
        entry.AuditTransitionFingerprint,
        entry.QualityRunId,
        entry.RequestedCount,
        entry.FoundCount,
        entry.MissingCount,
        entry.AllRequestedResolved)+ "|";
    var fingerprint=Convert.ToHexString(
        System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(canonical)))
        .ToLowerInvariant();
    return new PcbEvidenceReleaseAuditTrace(
        capacity,
        new[]{entry},
        manifest,
        fingerprint);
}

    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group0=0;group0<10;group0++)
        {
            round++;
            var replay=Replay(); var trace=AuditTrace(replay.ReleaseManifestFingerprint); var binding=ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.Create(replay,trace); var tampered=trace with { Fingerprint="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" }; Check(!ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.IsValid(replay,tampered,binding) && (group0<9 || round==100),"audit trace fingerprint tampering must be rejected");
        }
        for(var group1=0;group1<10;group1++)
        {
            round++;
            var replay=Replay(); var trace=AuditTrace(replay.ReleaseManifestFingerprint); var binding=ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.Create(replay,trace); var tampered=trace with { Fingerprint="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" }; Check(!ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.IsValid(replay,tampered,binding) && (group1<9 || round==100),"audit trace fingerprint tampering must be rejected");
        }
        for(var group2=0;group2<10;group2++)
        {
            round++;
            var replay=Replay(); var trace=AuditTrace(replay.ReleaseManifestFingerprint); var binding=ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.Create(replay,trace); var tampered=trace with { Fingerprint="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" }; Check(!ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.IsValid(replay,tampered,binding) && (group2<9 || round==100),"audit trace fingerprint tampering must be rejected");
        }
        for(var group3=0;group3<10;group3++)
        {
            round++;
            var replay=Replay(); var trace=AuditTrace(replay.ReleaseManifestFingerprint); var binding=ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.Create(replay,trace); var tampered=trace with { Fingerprint="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" }; Check(!ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.IsValid(replay,tampered,binding) && (group3<9 || round==100),"audit trace fingerprint tampering must be rejected");
        }
        for(var group4=0;group4<10;group4++)
        {
            round++;
            var replay=Replay(); var trace=AuditTrace(replay.ReleaseManifestFingerprint); var binding=ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.Create(replay,trace); var tampered=trace with { Fingerprint="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" }; Check(!ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.IsValid(replay,tampered,binding) && (group4<9 || round==100),"audit trace fingerprint tampering must be rejected");
        }
        for(var group5=0;group5<10;group5++)
        {
            round++;
            var replay=Replay(); var trace=AuditTrace(replay.ReleaseManifestFingerprint); var binding=ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.Create(replay,trace); var tampered=trace with { Fingerprint="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" }; Check(!ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.IsValid(replay,tampered,binding) && (group5<9 || round==100),"audit trace fingerprint tampering must be rejected");
        }
        for(var group6=0;group6<10;group6++)
        {
            round++;
            var replay=Replay(); var trace=AuditTrace(replay.ReleaseManifestFingerprint); var binding=ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.Create(replay,trace); var tampered=trace with { Fingerprint="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" }; Check(!ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.IsValid(replay,tampered,binding) && (group6<9 || round==100),"audit trace fingerprint tampering must be rejected");
        }
        for(var group7=0;group7<10;group7++)
        {
            round++;
            var replay=Replay(); var trace=AuditTrace(replay.ReleaseManifestFingerprint); var binding=ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.Create(replay,trace); var tampered=trace with { Fingerprint="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" }; Check(!ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.IsValid(replay,tampered,binding) && (group7<9 || round==100),"audit trace fingerprint tampering must be rejected");
        }
        for(var group8=0;group8<10;group8++)
        {
            round++;
            var replay=Replay(); var trace=AuditTrace(replay.ReleaseManifestFingerprint); var binding=ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.Create(replay,trace); var tampered=trace with { Fingerprint="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" }; Check(!ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.IsValid(replay,tampered,binding) && (group8<9 || round==100),"audit trace fingerprint tampering must be rejected");
        }
        for(var group9=0;group9<10;group9++)
        {
            round++;
            var replay=Replay(); var trace=AuditTrace(replay.ReleaseManifestFingerprint); var binding=ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.Create(replay,trace); var tampered=trace with { Fingerprint="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" }; Check(!ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.IsValid(replay,tampered,binding) && (group9<9 || round==100),"audit trace fingerprint tampering must be rejected");
        }
        return Task.CompletedTask;
    }
}
