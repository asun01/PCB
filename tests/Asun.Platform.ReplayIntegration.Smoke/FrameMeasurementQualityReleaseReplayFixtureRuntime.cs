using Asun.Platform.MeasurementQualityIntegration;

namespace Asun.Platform.ReplayIntegration.Smoke;

public sealed record FrameMeasurementQualityReleaseReplayFixture(
    ProductionFrameMeasurementQualityProvenanceBinding QualityProvenanceBinding,
    ProductionMeasurementQualityEvidenceReleaseReplayDescriptor ReplayDescriptor);

public static class FrameMeasurementQualityReleaseReplayFixtureRuntime
{
    public static FrameMeasurementQualityReleaseReplayFixture Create()
    {
        const string input="1111111111111111111111111111111111111111111111111111111111111111";
        var quality=new ProductionFrameMeasurementQualityProvenanceBinding(
            1,
            input,
            Guid.Parse("a1000000-0000-0000-0000-000000000001"),
            Guid.Parse("a2000000-0000-0000-0000-000000000001"),
            "3333333333333333333333333333333333333333333333333333333333333333",
            "4444444444444444444444444444444444444444444444444444444444444444",
            "5555555555555555555555555555555555555555555555555555555555555555");

        var descriptor=new ProductionMeasurementQualityEvidenceReleaseReplayDescriptor(
            Guid.Parse("a3000000-0000-0000-0000-000000000001"),
            Guid.Parse("a4000000-0000-0000-0000-000000000001"),
            1,
            input,
            quality.QualityResultId,
            "U1-ID",
            "6666666666666666666666666666666666666666666666666666666666666666",
            "7777777777777777777777777777777777777777777777777777777777777777",
            "8888888888888888888888888888888888888888888888888888888888888888",
            true,
            "9999999999999999999999999999999999999999999999999999999999999999",
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

        return new FrameMeasurementQualityReleaseReplayFixture(quality,descriptor);
    }
}
