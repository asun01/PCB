using Asun.Release.Core;
using Asun.Platform.ReleaseIntegration;

public static class ProductionReleaseReplayDescriptor2HundredStageSmoke
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
        var bad=descriptor with {ReleaseManifestFingerprint=new string('c',64)};
        var badCount=descriptor with {ArtifactCount=9};
        for(var i=0;i<10;i++) Check(!ProductionReleaseReplayDescriptorRuntime.IsValid(manifest,bad),"Manifest identity tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionReleaseReplayDescriptorRuntime.IsValid(manifest,badCount),"Artifact count tampering should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.Validate(manifest,bad).Count>0,"Manifest tampering should produce diagnostics.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.Validate(manifest,badCount).Count>0,"Artifact count tampering should produce diagnostics.");
        for(var i=0;i<10;i++) Check(descriptor.ReleaseManifestFingerprint==manifest.Fingerprint,"Baseline manifest identity should remain stable.");
        for(var i=0;i<10;i++) Check(descriptor.ArtifactCount==2,"Baseline artifact count should remain stable.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.IsValid(manifest,descriptor),"Baseline descriptor should remain valid.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.Create(manifest).DescriptorFingerprint==descriptor.DescriptorFingerprint,"Baseline descriptor should remain deterministic.");
        for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.Length==64,"Baseline descriptor fingerprint should remain fixed width.");
        assert(round==100,$"ProductionReleaseReplayDescriptor2HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
