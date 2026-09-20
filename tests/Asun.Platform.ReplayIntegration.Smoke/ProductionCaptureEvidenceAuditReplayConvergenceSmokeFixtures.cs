using Asun.Device.Contracts;
using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.Evidence;
using Asun.Platform.PcbAuditReleaseIntegration;
using Asun.Platform.PcbEvidenceReleaseIntegration;
using Asun.Platform.Pipeline;
using Asun.Production.Runtime;
using Asun.Release.Core;

namespace Asun.Platform.ReplayIntegration.Smoke;

internal static class ProductionCaptureEvidenceAuditReplayConvergenceSmokeFixtures
{
    public static (
        ProductionCaptureEvidenceReleaseAuditTraceReplayBinding Binding,
        PcbEvidenceReleaseAuditTraceReplayDescriptor Descriptor)
        Create()
    {
        const string inputFingerprint="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        const string programFingerprint="bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";
        const string productionFingerprint="cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc";
        const string pipelineFingerprint="dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd";
        const string artifactFingerprint="eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee";
        const string transitionFingerprint="ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
        var sessionId=Guid.Parse("00000000-0000-0000-0000-000000000501");
        var qualityRunId=Guid.Parse("00000000-0000-0000-0000-000000000502");

        var report=new ProductionSessionReport(
            sessionId,
            programFingerprint,
            1,
            new[]
            {
                new ProductionFrameExecution(
                    FrameSequence.Create(1),
                    inputFingerprint,
                    new PipelineExecutionReport(
                        1,
                        new[]{"stage-1"},
                        pipelineFingerprint))
            },
            productionFingerprint);

        var releaseManifest=ReleaseManifestRuntime.Create(
            new ReleaseIdentity(
                "AsunPCB",
                new Version(1,0,0),
                "production"),
            new[]
            {
                new ReleaseArtifact(
                    "artifacts/capture-audit-replay.json",
                    artifactFingerprint,
                    128)
            });

        var references=new[]
        {
            new ProductionCaptureEvidenceFrameReference(
                1,
                inputFingerprint,
                640,
                480,
                "Gray8",
                new[]{EvidenceHandle.Create("evidence/capture-audit-001")})
        };

        var releaseReplay=ProductionCaptureEvidenceReleaseReplayBindingRuntime.Create(
            ProductionCaptureEvidenceReplayBindingRuntime.Create(report,references),
            releaseManifest);

        var entry=new PcbEvidenceReleaseAuditTraceEntry(
            1,
            inputFingerprint,
            releaseManifest.Fingerprint,
            transitionFingerprint,
            qualityRunId,
            1,
            1,
            0,
            true);

        var canonical=string.Join(
            "|",
            4,
            releaseManifest.Fingerprint,
            entry.Sequence,
            entry.EvidenceReleaseFactFingerprint,
            entry.AuditTransitionFingerprint,
            entry.QualityRunId,
            entry.RequestedCount,
            entry.FoundCount,
            entry.MissingCount,
            entry.AllRequestedResolved)+ "|";

        var traceFingerprint=Convert.ToHexString(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();

        var trace=new PcbEvidenceReleaseAuditTrace(
            4,
            new[]{entry},
            releaseManifest.Fingerprint,
            traceFingerprint);

        var binding=ProductionCaptureEvidenceReleaseAuditTraceReplayBindingRuntime.Create(
            releaseReplay,
            trace);

        var auditReplay=new PcbAuditReleaseReplayDescriptor(
            transitionFingerprint,
            inputFingerprint,
            qualityRunId,
            "1111111111111111111111111111111111111111111111111111111111111111",
            releaseManifest.Fingerprint);

        var descriptor=PcbEvidenceReleaseAuditTraceReplayDescriptorRuntime.Create(
            trace,
            auditReplay);

        return (binding,descriptor);
    }
}
