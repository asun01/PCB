using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.QualityEvidenceIntegration;

public static class QualityEvidenceHandleProjectionHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var findingId=QualityFindingId.Create("AOI.IMAGE.MISSING");
        var evidenceKey=QualityEvidenceKey.Create("evidence/frame/1/result");
        var run=QualityInspectionRunRuntime.Create(
            Guid.Parse("A1000000-0000-0000-0000-000000000001"),
            new[]
            {
                new QualityInspectionResult(
                    Guid.Parse("A2000000-0000-0000-0000-000000000001"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("A3000000-0000-0000-0000-000000000001"),
                        1,
                        new QualityFindingSet(
                            new[]
                            {
                                new QualityFinding(
                                    findingId,
                                    "AOI.IMAGE.MISSING",
                                    QualityOutcome.Fail,
                                    QualitySeverity.Critical,
                                    "Required image evidence is missing")
                            }),
                        new QualityFindingEvidenceSet(
                            new[]
                            {
                                new QualityFindingEvidenceLink(
                                    findingId,
                                    evidenceKey)
                            })))
            });
        var bindings=new[]
        {
            new QualityEvidenceHandleBinding(
                findingId,
                evidenceKey,
                EvidenceHandle.Create("evidence/frame/1/result"))
        };
        var projection=QualityEvidenceHandleProjectionRuntime.Create(run,bindings);
        var tampered=projection with
        {
            Bindings=new[]
            {
                projection.Bindings[0] with
                {
                    EvidenceHandle=EvidenceHandle.Create("evidence/frame/1/other")
                }
            }
        };
        var mismatched=projection with
        {
            QualityRunId=Guid.Parse("A4000000-0000-0000-0000-000000000001")
        };

        for(var i=0;i<10;i++) Check(run.ResultCount==1,"Quality run should contain one result.");
        for(var i=0;i<10;i++) Check(run.Results[0].Evidence.Count==1,"Quality run should contain one evidence link.");
        for(var i=0;i<10;i++) Check(projection.QualityRunId==run.RunId,"Projection should preserve Quality run identity.");
        for(var i=0;i<10;i++) Check(projection.Bindings.Count==1,"Projection should contain one binding.");
        for(var i=0;i<10;i++) Check(projection.Bindings[0].FindingId==findingId,"Projection should preserve finding identity.");
        for(var i=0;i<10;i++) Check(projection.Bindings[0].EvidenceKey==evidenceKey,"Projection should preserve Quality evidence key.");
        for(var i=0;i<10;i++) Check(projection.Bindings[0].EvidenceHandle.IsValid,"Projection should preserve an opaque EvidenceHandle.");
        for(var i=0;i<10;i++) Check(QualityEvidenceHandleProjectionValidationRuntime.IsValid(run,projection),"Quality evidence projection should validate.");
        for(var i=0;i<10;i++) Check(!QualityEvidenceHandleProjectionValidationRuntime.IsValid(run,tampered),"Evidence handle tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityEvidenceHandleProjectionValidationRuntime.IsValid(run,mismatched),"Quality run identity tampering should be rejected.");

        assert(round==100,$"Quality evidence handle smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
