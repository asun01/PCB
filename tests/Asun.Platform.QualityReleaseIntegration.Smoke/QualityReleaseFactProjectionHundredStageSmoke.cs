using Asun.Domain.Quality;
using Asun.Platform.QualityReleaseIntegration;
using Asun.Release.Core;

public static class QualityReleaseFactProjectionHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var run=QualityInspectionRunRuntime.Create(
            Guid.Parse("91000000-0000-0000-0000-000000000001"),
            new[]
            {
                new QualityInspectionResult(
                    Guid.Parse("92000000-0000-0000-0000-000000000001"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("93000000-0000-0000-0000-000000000001"),
                        1,
                        new QualityFindingSet(
                            new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("QUALITY.FAIL"),
                                    QualityOutcome.Fail,
                                    QualitySeverity.Critical,
                                    "Demonstration failure")
                            }),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))),
                new QualityInspectionResult(
                    Guid.Parse("92000000-0000-0000-0000-000000000002"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("93000000-0000-0000-0000-000000000002"),
                        2,
                        new QualityFindingSet(
                            new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("QUALITY.REVIEW"),
                                    QualityOutcome.Review,
                                    QualitySeverity.Warning,
                                    "Demonstration review")
                            }),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())))
            });
        var manifest=ReleaseManifestRuntime.Create(
            new ReleaseIdentity("Asun PCB",new Version(1,0,0),"stable"),
            new[]{
                new ReleaseArtifact(
                    "quality/results.logical",
                    new string('a',64),
                    32)
            });
        var projection=QualityReleaseFactProjectionRuntime.Create(run,manifest);
        var tampered=projection with {CriticalCount=0};
        var manifestTampered=manifest with {Fingerprint=new string('b',64)};

        for(var i=0;i<10;i++) Check(run.ResultCount==2,"Quality run should contain two results.");
        for(var i=0;i<10;i++) Check(QualityInspectionRunSummaryRuntime.Create(run).FailCount==1,"Quality summary should report one fail finding.");
        for(var i=0;i<10;i++) Check(QualityInspectionRunSummaryRuntime.Create(run).ReviewCount==1,"Quality summary should report one review finding.");
        for(var i=0;i<10;i++) Check(QualityInspectionRunSummaryRuntime.Create(run).CriticalCount==1,"Quality summary should report one critical finding.");
        for(var i=0;i<10;i++) Check(manifest.Artifacts.Count==1,"Release manifest should contain one logical artifact.");
        for(var i=0;i<10;i++) Check(ReleaseReadinessRuntime.Evaluate(manifest).Ready,"Release readiness should remain a factual manifest property.");
        for(var i=0;i<10;i++) Check(projection.QualityRunId==run.RunId,"Projection should retain Quality run identity.");
        for(var i=0;i<10;i++) Check(QualityReleaseFactProjectionValidationRuntime.IsValid(run,manifest,projection),"Quality release projection should validate.");
        for(var i=0;i<10;i++) Check(!QualityReleaseFactProjectionValidationRuntime.IsValid(run,manifest,tampered),"Tampered Quality counts should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityReleaseFactProjectionValidationRuntime.IsValid(run,manifestTampered,projection),"Tampered release manifest should be rejected.");

        assert(round==100,$"Quality release fact smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
