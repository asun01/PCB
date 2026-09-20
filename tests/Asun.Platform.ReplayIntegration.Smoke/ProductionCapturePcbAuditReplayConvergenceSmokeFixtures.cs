using Asun.Platform.PcbAuditReleaseIntegration;
using Asun.Platform.ReplayIntegration;
using Asun.Release.Core;

namespace Asun.Platform.ReplayIntegration.Smoke;

internal static class ProductionCapturePcbAuditReplayConvergenceSmokeFixtures
{
    public static (
        ProductionCaptureEvidenceAuditReplayConvergence Capture,
        ProductionReleaseCandidatePcbAuditReplayClosure Pcb)
        Create()
    {
        var baseFixture=ProductionCaptureEvidenceAuditReplayConvergenceSmokeFixtures.Create();
        var manifest=ReleaseManifestRuntime.Create(
            new ReleaseIdentity(
                "AsunPCB",
                new Version(1,0,0),
                "production"),
            new[]
            {
                new ReleaseArtifact(
                    "artifacts/capture-audit-replay.json",
                    "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee",
                    128)
            });

        var sessionId=baseFixture.Binding.ProductionSessionId;
        var qualityRunId=baseFixture.Descriptor.QualityRunId;
        var readiness=ReleaseReadinessRuntime.Evaluate(manifest).Ready;

        if(baseFixture.Binding.ReleaseManifestFingerprint!=manifest.Fingerprint ||
           baseFixture.Descriptor.ReleaseManifestFingerprint!=manifest.Fingerprint)
        {
            throw new InvalidOperationException("Convergence fixture manifest fingerprint was not reproduced deterministically.");
        }

        var releaseCandidateAudit=new ProductionExecutionReleaseCandidateAuditClosure(
            sessionId,
            new string('b',64),
            manifest.Fingerprint,
            readiness,
            manifest.Artifacts[0].Path,
            new string('c',64));

        var pcbAuditReplay=new PcbAuditReleaseReplayDescriptor(
            new string('d',64),
            new string('f',64),
            qualityRunId,
            new string('1',64),
            manifest.Fingerprint);

        var pcb=ProductionReleaseCandidatePcbAuditReplayClosureRuntime.Create(
            releaseCandidateAudit,
            pcbAuditReplay,
            manifest);

        var capture=ProductionCaptureEvidenceAuditReplayConvergenceRuntime.Create(
            baseFixture.Binding,
            baseFixture.Descriptor);

        return (capture,pcb);
    }
}
