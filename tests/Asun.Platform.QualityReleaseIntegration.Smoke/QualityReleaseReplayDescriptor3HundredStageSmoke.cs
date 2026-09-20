using Asun.Domain.Quality;
using Asun.Platform.QualityReleaseIntegration;
using Asun.Release.Core;

public static class QualityReleaseReplayDescriptor3HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var run=QualityInspectionRunRuntime.Create(
            Guid.Parse("91000000-0000-0000-0000-000000000001"),
            new[]
            {
                new QualityInspectionResult(
                    Guid.Parse("92000000-0000-0000-0000-000000000001"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("93000000-0000-0000-0000-000000000001"),1,
                        new QualityFindingSet(new[]
                        {
                            new QualityFinding(QualityFindingId.Create("QUALITY.FAIL"),QualityOutcome.Fail,QualitySeverity.Critical,"Demonstration failure")
                        }),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))),
                new QualityInspectionResult(
                    Guid.Parse("92000000-0000-0000-0000-000000000002"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("93000000-0000-0000-0000-000000000002"),2,
                        new QualityFindingSet(new[]
                        {
                            new QualityFinding(QualityFindingId.Create("QUALITY.REVIEW"),QualityOutcome.Review,QualitySeverity.Warning,"Demonstration review")
                        }),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())))
            });
        var manifest=ReleaseManifestRuntime.Create(
            new ReleaseIdentity("Asun PCB",new Version(1,0,0),"stable"),
            new[]{new ReleaseArtifact("quality/results.logical",new string('a',64),32)});
        var descriptor=QualityReleaseReplayDescriptorRuntime.Create(run,manifest);
        var badManifest=descriptor with {ReleaseManifestFingerprint=new string('c',64)};
        var badProjection=descriptor with {ProjectionFingerprint=new string('d',64)};
        var badReady=descriptor with {ReleaseReady=!descriptor.ReleaseReady};
        for(var i=0;i<10;i++) Check(!QualityReleaseReplayDescriptorRuntime.IsValid(run,manifest,badManifest),"Manifest identity tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityReleaseReplayDescriptorRuntime.IsValid(run,manifest,badProjection),"Projection identity tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityReleaseReplayDescriptorRuntime.IsValid(run,manifest,badReady),"Readiness tampering should be rejected.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.Validate(run,manifest,badManifest).Count>0,"Manifest tampering should produce diagnostics.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.Validate(run,manifest,badProjection).Count>0,"Projection tampering should produce diagnostics.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.Validate(run,manifest,badReady).Count>0,"Readiness tampering should produce diagnostics.");
        for(var i=0;i<10;i++) Check(descriptor.ReleaseManifestFingerprint==manifest.Fingerprint,"Baseline manifest identity should remain stable.");
        for(var i=0;i<10;i++) Check(descriptor.ProjectionFingerprint.Length==64,"Baseline projection fingerprint should remain fixed width.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.IsValid(run,manifest,descriptor),"Baseline descriptor should remain valid.");
for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.Create(run,manifest).ProjectionFingerprint==descriptor.ProjectionFingerprint,"Recreated release descriptor should preserve the projection fingerprint.");

        assert(round==100,$"QualityReleaseReplayDescriptor3HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
