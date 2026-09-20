using Asun.Domain.Quality;
using Asun.Platform.QualityReleaseIntegration;
using Asun.Release.Core;

public static class QualityReleaseReplayDescriptor2HundredStageSmoke
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
        var bad=descriptor with {QualityRunId=Guid.NewGuid()};
        var badSummary=descriptor with {QualitySummaryFingerprint=new string('b',64)};
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.IsValid(run,manifest,descriptor),"Baseline descriptor should validate.");
        for(var i=0;i<10;i++) Check(!QualityReleaseReplayDescriptorRuntime.IsValid(run,manifest,bad),"Quality run identity tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityReleaseReplayDescriptorRuntime.IsValid(run,manifest,badSummary),"Summary identity tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityReleaseReplayDescriptorRuntime.IsEquivalent(descriptor,bad),"Run identity mutation should alter descriptor identity.");
        for(var i=0;i<10;i++) Check(!QualityReleaseReplayDescriptorRuntime.IsEquivalent(descriptor,badSummary),"Summary mutation should alter descriptor identity.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.Validate(run,manifest,bad).Count>0,"Run tampering should produce diagnostics.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.Validate(run,manifest,badSummary).Count>0,"Summary tampering should produce diagnostics.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.Create(run,manifest).DescriptorFingerprint==descriptor.DescriptorFingerprint,"Baseline descriptor should remain deterministic.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.IsValid(run,manifest,descriptor),"Baseline descriptor should remain valid.");
        assert(round==100,$"QualityReleaseReplayDescriptor2HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
