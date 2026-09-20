namespace Asun.Platform.ReplayIntegration.Smoke;

public sealed record MeasurementQualityEvidenceAuditReplayFixture(
    ProductionMeasurementQualityEvidenceReleaseReplayDescriptor ReplayDescriptor,
    ProductionCaptureEvidenceReleaseAuditTraceReplayBinding AuditBinding);

public static class MeasurementQualityEvidenceAuditReplayFixtureRuntime
{
    public static MeasurementQualityEvidenceAuditReplayFixture Create()
    {
        var descriptor=new ProductionMeasurementQualityEvidenceReleaseReplayDescriptor(
            Guid.Parse("b1000000-0000-0000-0000-000000000001"),
            Guid.Parse("b2000000-0000-0000-0000-000000000001"),
            1,
            "1111111111111111111111111111111111111111111111111111111111111111",
            Guid.Parse("b3000000-0000-0000-0000-000000000001"),
            "U1-ID",
            "2222222222222222222222222222222222222222222222222222222222222222",
            "3333333333333333333333333333333333333333333333333333333333333333",
            "4444444444444444444444444444444444444444444444444444444444444444",
            true,
            "5555555555555555555555555555555555555555555555555555555555555555",
            "6666666666666666666666666666666666666666666666666666666666666666");

        var audit=new ProductionCaptureEvidenceReleaseAuditTraceReplayBinding(
            descriptor.ProductionSessionId,
            "7777777777777777777777777777777777777777777777777777777777777777",
            "8888888888888888888888888888888888888888888888888888888888888888",
            descriptor.ReleaseManifestFingerprint,
            7,
            "9999999999999999999999999999999999999999999999999999999999999999");

        return new MeasurementQualityEvidenceAuditReplayFixture(descriptor,audit);
    }
}
