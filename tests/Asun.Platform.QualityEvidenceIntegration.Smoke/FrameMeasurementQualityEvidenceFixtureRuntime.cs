using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.MeasurementQualityIntegration;

namespace Asun.Platform.QualityEvidenceIntegration.Smoke;

public sealed record FrameMeasurementQualityEvidenceFixture(
    ProductionFrameMeasurementQualityProvenanceBinding QualityProvenanceBinding,
    ProductionMeasurementQualityEvidenceBinding EvidenceBinding);

public static class FrameMeasurementQualityEvidenceFixtureRuntime
{
    public static FrameMeasurementQualityEvidenceFixture Create()
    {
        var qualityBinding=new ProductionFrameMeasurementQualityProvenanceBinding(
            1,
            "1111111111111111111111111111111111111111111111111111111111111111",
            Guid.Parse("f1000000-0000-0000-0000-000000000001"),
            Guid.Parse("f2000000-0000-0000-0000-000000000001"),
            "3333333333333333333333333333333333333333333333333333333333333333",
            "4444444444444444444444444444444444444444444444444444444444444444",
            "5555555555555555555555555555555555555555555555555555555555555555");

        var evidence=new ProductionMeasurementQualityEvidenceBinding(
            1,
            qualityBinding.ProductionInputFingerprint,
            qualityBinding.QualityResultId,
            QualityFindingId.Create("MEASURE.OK"),
            "U1-ID",
            "6666666666666666666666666666666666666666666666666666666666666666",
            1,
            "7777777777777777777777777777777777777777777777777777777777777777",
            "8888888888888888888888888888888888888888888888888888888888888888");

        return new FrameMeasurementQualityEvidenceFixture(qualityBinding,evidence);
    }
}
