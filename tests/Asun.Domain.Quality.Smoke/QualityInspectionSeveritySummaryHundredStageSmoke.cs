using Asun.Domain.Quality;

public static class QualityInspectionSeveritySummaryHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        static QualityFinding Make(
            string id,
            QualitySeverity severity) =>
            new(
                QualityFindingId.Create(id),
                "RULE",
                QualityOutcome.Review,
                severity,
                id);

        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            new QualityInspectionSnapshot(
                Guid.NewGuid(),
                101,
                new QualityFindingSet(new[]{
                    Make("F-001",QualitySeverity.None),
                    Make("F-002",QualitySeverity.Information),
                    Make("F-003",QualitySeverity.Warning),
                    Make("F-004",QualitySeverity.Error),
                    Make("F-005",QualitySeverity.Critical)
                }),
                new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())));

        var summary=QualityInspectionSeveritySummaryRuntime.Create(result);
        var invalid=summary with {CriticalCount=-1};
        var wrong=summary with {WarningCount=summary.WarningCount+1};

        for(var i=0;i<10;i++) Check(QualityInspectionSeveritySummaryValidationRuntime.IsValid(result,summary),$"severity summary validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(summary.NoneCount==1,$"none count round {i+1} should be one.");
        for(var i=0;i<10;i++) Check(summary.InformationCount==1,$"information count round {i+1} should be one.");
        for(var i=0;i<10;i++) Check(summary.WarningCount==1,$"warning count round {i+1} should be one.");
        for(var i=0;i<10;i++) Check(summary.ErrorCount==1,$"error count round {i+1} should be one.");
        for(var i=0;i<10;i++) Check(summary.CriticalCount==1,$"critical count round {i+1} should be one.");
        for(var i=0;i<10;i++) Check(summary.TotalCount==5 && summary.TotalCount==result.Findings.Count,$"severity total round {i+1} should match findings.");
        for(var i=0;i<10;i++) Check(!QualityInspectionSeveritySummaryValidationRuntime.IsValid(result,invalid),$"negative severity count round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityInspectionSeveritySummaryValidationRuntime.IsValid(result,wrong),$"severity total mismatch round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(QualityInspectionSeveritySummaryRuntime.Create(result)==summary,$"severity summary determinism round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection severity summary smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
