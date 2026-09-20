using Asun.Release.Core;
using Asun.Platform.ReleaseIntegration;

public static class ProductionReleaseReplayDescriptor4HundredStageSmoke
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
        var reordered=new ReleaseManifest(
            manifest.Identity,
            manifest.Artifacts.Reverse().ToArray(),
            manifest.Fingerprint);
        var reorderedDescriptor=ProductionReleaseReplayDescriptorRuntime.Create(reordered);
        for(var i=0;i<10;i++) Check(reorderedDescriptor.ReleaseManifestFingerprint==descriptor.ReleaseManifestFingerprint,"Release manifest identity should remain stable under equivalent artifact ordering.");
        for(var i=0;i<10;i++) Check(reorderedDescriptor.ArtifactCount==descriptor.ArtifactCount,"Equivalent artifact ordering should preserve count.");
        for(var i=0;i<10;i++) Check(reorderedDescriptor.ReleaseReady==descriptor.ReleaseReady,"Equivalent artifact ordering should preserve readiness.");
        for(var i=0;i<10;i++) Check(reorderedDescriptor.DescriptorFingerprint==descriptor.DescriptorFingerprint,"Equivalent manifest representation should preserve replay identity.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.IsEquivalent(descriptor,reorderedDescriptor),"Equivalent manifests should produce equivalent descriptors.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.IsValid(reordered,reorderedDescriptor),"Reordered manifest should validate.");
        for(var i=0;i<10;i++) Check(ProductionReleaseReplayDescriptorRuntime.IsValid(manifest,descriptor),"Original manifest should remain valid.");
        for(var i=0;i<10;i++) Check(descriptor.ArtifactCount==2,"Original artifact count should remain stable.");
        for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.Length==64,"Original descriptor fingerprint should remain fixed width.");
        assert(round==100,$"ProductionReleaseReplayDescriptor4HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
