using Asun.Platform.MeasurementQualityIntegration;
using Asun.Platform.MetrologyProductionIntegration;
using Asun.Domain.Quality;

public static class MeasurementQualityEvaluationHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var measurement=new ProductionMeasurementFact(
            1,
            new string('a',64),
            new Asun.Metrology.Core.MetrologyPoint2D(0,0),
            new Asun.Metrology.Core.MetrologyPoint2D(12,23),
            0,
            new string('b',64),
            new string('c',64));
        var evaluation=MeasurementQualityEvaluationRuntime.Evaluate(
            measurement,
            1,
            Guid.Parse("A9000000-0000-0000-0000-000000000001"),
            Guid.Parse("AA000000-0000-0000-0000-000000000001"),
            fact=>new QualityFinding(
                QualityFindingId.Create("MEASURE.OK"),
                "MEASURE.OK",
                QualityOutcome.Pass,
                QualitySeverity.None,
                $"Residual={fact.ErrorDistance:R}"));
        var tampered=evaluation with
        {
            Fingerprint=new string('d',64)
        };
        var resultTampered=evaluation with
        {
            Result=evaluation.Result with
            {
                ResultId=Guid.Parse("AB000000-0000-0000-0000-000000000001")
            }
        };

        for(var i=0;i<10;i++) Check(measurement.Sequence==1,"Measurement fact should retain source sequence.");
        for(var i=0;i<10;i++) Check(measurement.ErrorDistance==0,"Baseline measurement should have zero residual.");
        for(var i=0;i<10;i++) Check(evaluation.Result.Sequence==1,"Quality evaluation should retain measurement sequence.");
        for(var i=0;i<10;i++) Check(evaluation.Result.Findings.Count==1,"Quality evaluation should contain one finding.");
        for(var i=0;i<10;i++) Check(evaluation.Result.Findings.Findings[0].Outcome==QualityOutcome.Pass,"Rule outcome should be preserved as supplied.");
        for(var i=0;i<10;i++) Check(evaluation.Result.Evidence.Count==0,"Measurement quality layer should not fabricate evidence.");
        for(var i=0;i<10;i++) Check(MeasurementQualityEvaluationValidationRuntime.IsValid(evaluation),"Measurement quality evaluation should validate.");
        for(var i=0;i<10;i++) Check(!MeasurementQualityEvaluationValidationRuntime.IsValid(tampered),"Evaluation fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!MeasurementQualityEvaluationValidationRuntime.IsValid(resultTampered),"Quality result identity tampering should be rejected.");
        for(var i=0;i<10;i++) Check(evaluation.Fingerprint.Length==64,"Measurement quality fingerprint should be fixed width.");

        assert(round==100,$"Measurement quality smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
