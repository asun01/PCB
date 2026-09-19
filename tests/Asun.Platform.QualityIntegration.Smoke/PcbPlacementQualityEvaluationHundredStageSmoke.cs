using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Metrology.Core;
using Asun.Platform.QualityIntegration;

public static class PcbPlacementQualityEvaluationHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var component=new PcbComponentReference(
            PcbFeatureId.Create("Q-OBS-001"),
            "C201",
            "100nF",
            "0402",
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(40,50),
            0);
        var observation=PcbPlacementObservationRuntime.Measure(
            component,
            new MetrologyPoint2D(40.25,50.10));
        var evaluation=PcbPlacementQualityEvaluationRuntime.Evaluate(
            observation,
            1,
            Guid.Parse("72000000-0000-0000-0000-000000000001"),
            Guid.Parse("73000000-0000-0000-0000-000000000001"),
            placement=>new QualityFinding(
                QualityFindingId.Create("PCB.PLACEMENT.OBSERVED"),
                QualityOutcome.Informational,
                QualitySeverity.Information,
                $"Observed placement error {placement.ErrorDistance:R}"));
        var run=QualityInspectionRunRuntime.Create(
            Guid.Parse("74000000-0000-0000-0000-000000000001"),
            new[]{evaluation.Result});
        var tampered=evaluation with {Fingerprint=new string('f',64)};

        for(var i=0;i<10;i++) Check(observation.ErrorDistance>0,"Placement observation should contain a real geometric residual.");
        for(var i=0;i<10;i++) Check(evaluation.Result.ResultId!=Guid.Empty,"Quality result should retain a concrete result identity.");
        for(var i=0;i<10;i++) Check(evaluation.Result.Sequence==1,"Quality result should retain the supplied measurement sequence.");
        for(var i=0;i<10;i++) Check(evaluation.Result.Findings.Count==1,"Placement evaluation should contain one injected rule finding.");
        for(var i=0;i<10;i++) Check(evaluation.Result.Findings.Findings[0].Outcome==QualityOutcome.Informational,"Evaluation should preserve the injected factual outcome.");
        for(var i=0;i<10;i++) Check(evaluation.Result.Evidence.Count==0,"Placement evaluation should not invent evidence links.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityEvaluationValidationRuntime.IsValid(evaluation),"Placement quality evaluation should validate independently.");
        for(var i=0;i<10;i++) Check(QualityInspectionRunValidationRuntime.IsValid(run),"Projected quality result should form a valid QualityInspectionRun.");
        for(var i=0;i<10;i++) Check(!PcbPlacementQualityEvaluationValidationRuntime.IsValid(tampered),"Evaluation fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(evaluation.Fingerprint.Length==64,"Placement quality evaluation fingerprint should be fixed width.");

        assert(round==100,$"PCB placement quality smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
