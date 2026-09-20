using Asun.Release.Core;
using Asun.Platform.ReleaseIntegration;

public static class ProductionReleaseReplayDescriptor3HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var manifest=ReleaseManifestRuntime.Create(
            new ReleaseIdentity("Asun PCB",new Version(1,0,0),"stable"),
            new[]
            {
                new ReleaseArtifact("production/session.report",new string('a',64),128),
                new ReleaseArtifact("evidence/session.references",new string('b',64),96)
            });
        var descriptor=ProductionReleaseReplayDescriptorRuntime.Create(manifest);
        var badReady=descriptor with {ReleaseReady=!descriptor.ReleaseReady};
        var malformed=descriptor with {DescriptorFingerprint="bad"};
        var upper=descriptor with {DescriptorFingerprint=new string('F',64)};
        for(var i=0;i<10;i++) Check(!ProductionReleaseReplayDescriptorRuntime.IsValid(manifest,badReady),"Readiness tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionReleaseReplayDescriptorRuntime.IsValid(manifest,malformed),"Malformed descriptor fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionReleaseReplayDescriptorRuntime.IsValid(manifest,upper),"Uppercase fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.Validate(manifest,badReady).Count>0,"Readiness tampering should produce diagnostics.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.Validate(manifest,malformed).Count>0,"Malformed fingerprint should produce diagnostics.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.Validate(manifest,upper).Count>0,"Uppercase fingerprint should produce diagnostics.");
        for(var i=0;i<10;i++) Check(descriptor.ReleaseReady==ReleaseReadinessRuntime.Evaluate(manifest).Ready,"Baseline readiness should remain stable.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.IsValid(manifest,descriptor),"Baseline descriptor should remain valid.");
for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.IsValid(manifest,descriptor),"Baseline release descriptor should remain valid.");
        for(var i=0;i<10;i++) Check(descriptor.ReleaseManifestFingerprint==manifest.Fingerprint,"Baseline release manifest identity should remain stable.");

        assert(round==100,$"ProductionReleaseReplayDescriptor3HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
