using Asun.Domain.Quality;
using Asun.Platform.MeasurementQualityIntegration;
using Asun.Platform.MetrologyProductionIntegration;
using Asun.Platform.RoiProductionIntegration;
using Asun.Metrology.Core;

namespace Asun.Platform.RoiProductionIntegration.Smoke;

internal static class ProductionRoiMeasurementQualitySmokeFixtures
{
    public static (
        Asun.Production.Runtime.ProductionSessionReport Production,
        ProductionRoiInteractionContext RoiContext,
        MeasurementQualityEvaluation Evaluation)
        Create()
    {
        var baseFixture=ProductionRoiInteractionContextSmokeFixtures.Create();
        var roiContext=ProductionRoiInteractionContextRuntime.Create(
            baseFixture.Production,
            baseFixture.RoiSnapshot);

        var measurement=new ProductionMeasurementFact(
            1,
            baseFixture.Production.Frames.Single(frame=>frame.Sequence.Value==1).InputFingerprint,
            new MetrologyPoint2D(10,20),
            new MetrologyPoint2D(10.25,20),
            0.25,
            new string('a',64),
            new string('b',64));

        var evaluation=MeasurementQualityEvaluationRuntime.Evaluate(
            measurement,
            1,
            Guid.Parse("76000000-0000-0000-0000-000000000001"),
            Guid.Parse("77000000-0000-0000-0000-000000000001"),
            _=>new QualityFinding(
                QualityFindingId.Create("ROI-MEASUREMENT-R1"),
                "ROI-MEASUREMENT",
                QualityOutcome.Pass,
                QualitySeverity.None,
                "Measurement correlation accepted."));

        return (baseFixture.Production,roiContext,evaluation);
    }
}
