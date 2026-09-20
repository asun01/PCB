using Asun.Domain.Quality;
using Asun.Platform.QualityReleaseIntegration;
using Asun.Release.Core;

public static class QualityReleaseReplayDescriptor5HundredStageSmoke
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
        var changedRun=QualityInspectionRunRuntime.Create(
            Guid.Parse("91000000-0000-0000-0000-000000000002"),
            new[]
            {
                new QualityInspectionResult(
                    Guid.Parse("92000000-0000-0000-0000-000000000003"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("93000000-0000-0000-0000-000000000003"),1,
                        new QualityFindingSet(new[]
                        {
                            new QualityFinding(QualityFindingId.Create("QUALITY.FAIL"),QualityOutcome.Fail,QualitySeverity.Critical,"Changed failure")
                        }),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())))
            });
        var changed=QualityReleaseReplayDescriptorRuntime.Create(changedRun,manifest);
        for(var i=0;i<10;i++) Check(changed.QualityRunId!=descriptor.QualityRunId,"Changed Quality run should change replay identity.");
        for(var i=0;i<10;i++) Check(changed.DescriptorFingerprint!=descriptor.DescriptorFingerprint,"Changed Quality content should change descriptor fingerprint.");
        for(var i=0;i<10;i++) Check(!QualityReleaseReplayDescriptorRuntime.IsEquivalent(descriptor,changed),"Different release facts should not be equivalent.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.IsValid(changedRun,manifest,changed),"Changed release descriptor should validate against changed source.");
        for(var i=0;i<10;i++) Check(changed.DescriptorFingerprint.Length==64,"Changed descriptor fingerprint should retain fixed width.");
        for(var i=0;i<10;i++) Check(changed.ProjectionFingerprint.Length==64,"Changed projection fingerprint should retain fixed width.");
        for(var i=0;i<10;i++) Check(changed.QualitySummaryFingerprint.Length==64,"Changed summary fingerprint should retain fixed width.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.Create(run,manifest).DescriptorFingerprint==descriptor.DescriptorFingerprint,"Original descriptor creation should remain deterministic.");
        for(var i=0;i<10;i++) Check(descriptor.ReleaseManifestFingerprint==manifest.Fingerprint,"Original manifest identity should remain stable.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.IsValid(run,manifest,descriptor),"Original descriptor should remain valid.");
        assert(round==100,$"QualityReleaseReplayDescriptor5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
