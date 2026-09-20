using Asun.Metrology.Core;

namespace Asun.Platform.MetrologyProductionIntegration.Smoke;

public sealed record FrameMeasurementProvenanceFixture(
    ProductionFrameProvenance Provenance,
    ProductionMeasurementFact Measurement);

public static class FrameMeasurementProvenanceFixtureRuntime
{
    public static FrameMeasurementProvenanceFixture Create()
    {
        const string input="1111111111111111111111111111111111111111111111111111111111111111";
        var provenance=new ProductionFrameProvenance(
            FrameSequence.Create(1),
            1920,
            1080,
            "Gray8",
            DateTimeOffset.UnixEpoch.AddSeconds(1),
            input);

        var measurement=new ProductionMeasurementFact(
            1,
            input,
            new MetrologyPoint2D(12,18),
            new MetrologyPoint2D(12.5,18.25),
            0.5590169943749475,
            "2222222222222222222222222222222222222222222222222222222222222222",
            "3333333333333333333333333333333333333333333333333333333333333333");

        return new FrameMeasurementProvenanceFixture(provenance,measurement);
    }
}
