using Asun.Domain.Quality;
using Asun.Platform.QualityReleaseIntegration;
using Asun.Release.Core;

public static class QualityReleaseReplayDescriptor1HundredStageSmoke
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
        for(var i=0;i<10;i++) Check(descriptor.QualityRunId==run.RunId,"Replay descriptor should preserve Quality run identity.");
        for(var i=0;i<10;i++) Check(descriptor.QualitySummaryFingerprint.Length==64,"Summary fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(descriptor.ReleaseManifestFingerprint==manifest.Fingerprint,"Release manifest identity should be preserved.");
        for(var i=0;i<10;i++) Check(descriptor.ProjectionFingerprint.Length==64,"Projection fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.Length==64,"Descriptor fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.IsValid(run,manifest,descriptor),"Canonical release replay descriptor should validate.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.IsEquivalent(descriptor,descriptor),"Descriptor should be equivalent to itself.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.Create(run,manifest).DescriptorFingerprint==descriptor.DescriptorFingerprint,"Descriptor creation should be deterministic.");
        for(var i=0;i<10;i++) Check(descriptor.ReleaseReady==ReleaseReadinessRuntime.Evaluate(manifest).Ready,"Readiness should remain a manifest fact.");
        for(var i=0;i<10;i++) Check(descriptor.QualitySummaryFingerprint.All(Uri.IsHexDigit),"Summary fingerprint should be hexadecimal.");
        assert(round==100,$"QualityReleaseReplayDescriptor1HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
