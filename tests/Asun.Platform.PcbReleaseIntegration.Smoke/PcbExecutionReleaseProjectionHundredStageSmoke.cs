using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.PcbReleaseIntegration;
using Asun.Release.Core;

public static class PcbExecutionReleaseProjectionHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var executionSnapshot=new PcbExecutionSnapshot(
            new string('a',64),
            Guid.Parse("B6000000-0000-0000-0000-000000000001"),
            new string('b',64),
            new string('c',64),
            2,
            2,
            Guid.Parse("B7000000-0000-0000-0000-000000000001"),
            new string('d',64),
            new string('e',64));
        var manifest=ReleaseManifestRuntime.Create(
            new ReleaseIdentity("Asun PCB",new Version(1,0,0),"stable"),
            new[]{
                new ReleaseArtifact(
                    "pcb/execution.snapshot",
                    new string('f',64),
                    64)
            });
        var projection=PcbExecutionReleaseProjectionRuntime.Create(executionSnapshot,manifest);
        var tampered=projection with
        {
            ExecutionSnapshotFingerprint=new string('1',64)
        };
        var readinessTampered=projection with
        {
            ReleaseReady=!projection.ReleaseReady
        };

        for(var i=0;i<10;i++) Check(executionSnapshot.FrameCount==2,"Execution snapshot should retain two frames.");
        for(var i=0;i<10;i++) Check(manifest.Artifacts.Count==1,"Release manifest should contain one logical artifact.");
        for(var i=0;i<10;i++) Check(ReleaseManifestValidationRuntime.IsValid(manifest),"Release manifest should validate.");
        for(var i=0;i<10;i++) Check(projection.AssemblyFingerprint==executionSnapshot.AssemblyFingerprint,"Release projection should bind assembly identity.");
        for(var i=0;i<10;i++) Check(projection.ExecutionSnapshotFingerprint==executionSnapshot.Fingerprint,"Release projection should bind execution snapshot identity.");
        for(var i=0;i<10;i++) Check(projection.ReleaseManifestFingerprint==manifest.Fingerprint,"Release projection should bind manifest identity.");
        for(var i=0;i<10;i++) Check(projection.ArtifactCount==manifest.Artifacts.Count,"Release projection should retain artifact count.");
        for(var i=0;i<10;i++) Check(PcbExecutionReleaseProjectionValidationRuntime.IsValid(executionSnapshot,manifest,projection),"PCB execution release projection should validate.");
        for(var i=0;i<10;i++) Check(!PcbExecutionReleaseProjectionValidationRuntime.IsValid(executionSnapshot,manifest,tampered),"Execution snapshot tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!PcbExecutionReleaseProjectionValidationRuntime.IsValid(executionSnapshot,manifest,readinessTampered),"Release readiness tampering should be rejected.");

        assert(round==100,$"PCB execution release smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
