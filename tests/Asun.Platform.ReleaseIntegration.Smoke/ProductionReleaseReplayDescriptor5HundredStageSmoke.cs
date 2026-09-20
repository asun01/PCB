using Asun.Release.Core;
using Asun.Platform.ReleaseIntegration;

public static class ProductionReleaseReplayDescriptor5HundredStageSmoke
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
        var changed=ReleaseManifestRuntime.Create(
            manifest.Identity,
            new[]{new ReleaseArtifact("production/session.report",new string('d',64),128)});
        var changedDescriptor=ProductionReleaseReplayDescriptorRuntime.Create(changed);
        for(var i=0;i<10;i++) Check(changed.Fingerprint!=manifest.Fingerprint,"Changed release artifact set should alter manifest identity.");
        for(var i=0;i<10;i++) Check(changedDescriptor.DescriptorFingerprint!=descriptor.DescriptorFingerprint,"Changed release artifact set should alter replay identity.");
        for(var i=0;i<10;i++) Check(!ProductionReleaseReplayDescriptorRuntime.IsEquivalent(descriptor,changedDescriptor),"Changed release content should break descriptor equivalence.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.IsValid(changed,changedDescriptor),"Changed release descriptor should validate against changed source.");
        for(var i=0;i<10;i++) Check(changedDescriptor.ArtifactCount==1,"Changed descriptor should report changed artifact count.");
        for(var i=0;i<10;i++) Check(changedDescriptor.ReleaseManifestFingerprint==changed.Fingerprint,"Changed descriptor should preserve changed manifest identity.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.Create(manifest).DescriptorFingerprint==descriptor.DescriptorFingerprint,"Original descriptor creation should remain deterministic.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.IsValid(manifest,descriptor),"Original descriptor should remain valid.");
        for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.All(Uri.IsHexDigit),"Original descriptor fingerprint should remain hexadecimal.");
        assert(round==100,$"ProductionReleaseReplayDescriptor5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
