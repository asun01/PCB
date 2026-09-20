using Asun.Metrology.Core;
using Asun.Platform.MetrologyProductionIntegration;
using Asun.Domain.Quality;

namespace Asun.Platform.MeasurementQualityIntegration.Smoke;

public sealed record FrameMeasurementQualityFixture(
    ProductionFrameMeasurementProvenanceBinding ProvenanceBinding,
    MeasurementQualityEvaluation Evaluation);

public static class FrameMeasurementQualityFixtureRuntime
{
    public static FrameMeasurementQualityFixture Create()
    {
        var measurement=new ProductionMeasurementFact(
            1,
            "1111111111111111111111111111111111111111111111111111111111111111",
            new MetrologyPoint2D(10,20),
            new MetrologyPoint2D(10.5,20.5),
            0.7071067811865476,
            "2222222222222222222222222222222222222222222222222222222222222222",
            "3333333333333333333333333333333333333333333333333333333333333333");

        var provenance=new ProductionFrameProvenance(
            Asun.Device.Contracts.FrameSequence.Create(1),
            1920,
            1080,
            "Gray8",
            DateTimeOffset.UnixEpoch.AddSeconds(2),
            measurement.ProductionInputFingerprint);

        var provenanceBinding=ProductionFrameMeasurementProvenanceBindingRuntime.Create(
            provenance,
            measurement);

        var evaluation=MeasurementQualityEvaluationRuntime.Evaluate(
            measurement,
            1,
            Guid.Parse("f1000000-0000-0000-0000-000000000001"),
            Guid.Parse("f2000000-0000-0000-0000-000000000001"),
            fact=>new QualityFinding(
                QualityFindingId.Create("MEASURE.OK"),
                "MEASURE.OK",
                QualityOutcome.Pass,
                QualitySeverity.None,
                $"Residual={fact.ErrorDistance:R}"));

        return new FrameMeasurementQualityFixture(provenanceBinding,evaluation);
    }
}
