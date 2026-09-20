using Asun.Release.Core;
using Asun.Platform.ReleaseIntegration;

public static class ProductionReleaseReplayDescriptor1HundredStageSmoke
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
        for(var i=0;i<10;i++) Check(descriptor.ReleaseManifestFingerprint==manifest.Fingerprint,"Release replay descriptor should preserve manifest identity.");
        for(var i=0;i<10;i++) Check(descriptor.ArtifactCount==manifest.Artifacts.Count,"Release replay descriptor should preserve artifact count.");
        for(var i=0;i<10;i++) Check(descriptor.ReleaseReady==ReleaseReadinessRuntime.Evaluate(manifest).Ready,"Release replay descriptor should preserve factual readiness.");
        for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.Length==64,"Release descriptor fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.IsValid(manifest,descriptor),"Canonical release replay descriptor should validate.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.IsEquivalent(descriptor,descriptor),"Descriptor should be equivalent to itself.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.Create(manifest).DescriptorFingerprint==descriptor.DescriptorFingerprint,"Descriptor creation should be deterministic.");
        for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.All(Uri.IsHexDigit),"Descriptor fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(descriptor.ArtifactCount>0,"Descriptor artifact count should remain positive.");
        for(var i=0;i<10;i++) Check(ReleaseManifestValidationRuntime.IsValid(manifest),"Source release manifest should remain valid.");
        assert(round==100,$"ProductionReleaseReplayDescriptor1HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
