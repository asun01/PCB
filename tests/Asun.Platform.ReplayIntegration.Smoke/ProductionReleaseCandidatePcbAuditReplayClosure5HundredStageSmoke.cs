using Asun.Platform.PcbAuditReleaseIntegration;
using Asun.Platform.ReplayIntegration;
using Asun.Release.Core;

public static class ProductionReleaseCandidatePcbAuditReplayClosure5HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var sessionId=Guid.Parse("F50000000-0000-0000-0000-000000000001");
        var qualityRunId=Guid.Parse("F51000000-0000-0000-0000-000000000001");
        var manifest=ReleaseManifestRuntime.Create(
            new ReleaseIdentity("Asun PCB Release Candidate",new Version(1,0,5),"audit"),
            new[]{new ReleaseArtifact("release/audit/5.logical",new string('a',64),20+5)});
        var readiness=ReleaseReadinessRuntime.Evaluate(manifest).Ready;
        var releaseCandidateAudit=new ProductionExecutionReleaseCandidateAuditClosure(
            sessionId,
            new string('b',64),
            manifest.Fingerprint,
            readiness,
            manifest.Artifacts[0].Path,
            new string('c',64));
        var pcbAuditReplay=new PcbAuditReleaseReplayDescriptor(
            new string('d',64),
            new string('e',64),
            qualityRunId,
            new string('f',64),
            manifest.Fingerprint);
        var closure=ProductionReleaseCandidatePcbAuditReplayClosureRuntime.Create(
            releaseCandidateAudit,
            pcbAuditReplay,
            manifest);
        var tamperedDescriptor=pcbAuditReplay with {AuditWindowFingerprint=new string('7',64)};

        for(var i=0;i<10;i++) Check(closure.ProductionSessionId==sessionId,"Cross-chain closure should preserve Production session identity.");
        for(var i=0;i<10;i++) Check(closure.ReleaseCandidateAuditClosureFingerprint==releaseCandidateAudit.Fingerprint,"Cross-chain closure should preserve Release candidate audit identity.");
        for(var i=0;i<10;i++) Check(closure.PcbAuditReleaseTransitionFingerprint==pcbAuditReplay.TransitionFingerprint,"Cross-chain closure should preserve PCB audit transition identity.");
        for(var i=0;i<10;i++) Check(closure.QualityRunId==qualityRunId,"Cross-chain closure should preserve Quality run identity.");
        for(var i=0;i<10;i++) Check(closure.ReleaseManifestFingerprint==manifest.Fingerprint,"Cross-chain closure should preserve Release manifest identity.");
        for(var i=0;i<10;i++) Check(closure.ReleaseReady==readiness,"Cross-chain closure should preserve factual Release readiness.");
        for(var i=0;i<10;i++) Check(closure.ArtifactPath==manifest.Artifacts[0].Path,"Cross-chain closure should preserve logical artifact path.");
        for(var i=0;i<10;i++) Check(ProductionReleaseCandidatePcbAuditReplayClosureRuntime.IsValid(releaseCandidateAudit,pcbAuditReplay,manifest,closure),"Canonical cross-chain closure should validate.");
        for(var i=0;i<10;i++) Check(ProductionReleaseCandidatePcbAuditReplayClosureRuntime.Create(releaseCandidateAudit,pcbAuditReplay,manifest).Fingerprint==closure.Fingerprint,"Cross-chain closure creation should be deterministic.");
        for(var i=0;i<10;i++) Check(!ProductionReleaseCandidatePcbAuditReplayClosureRuntime.IsValid(releaseCandidateAudit,tamperedDescriptor,manifest,closure),"Cross-chain tampering should be rejected.");

        assert(round==100,$"ProductionReleaseCandidatePcbAuditReplayClosure5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
