using Asun.Device.Contracts;
using Asun.Platform.MetrologyProductionIntegration;
using Asun.Platform.ReplayIntegration;
using Asun.Production.Runtime;

namespace Asun.Platform.ReplayIntegration.Smoke;

public sealed record PcbMeasurementProvenanceReplayFixture(
    ProductionMeasurementPcbBinding MeasurementBinding,
    ProductionFrameProvenance Provenance,
    ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor ProvenanceDescriptor);

public static class PcbMeasurementProvenanceReplayFixtureRuntime
{
    public static PcbMeasurementProvenanceReplayFixture Create()
    {
        const string inputFingerprint="1111111111111111111111111111111111111111111111111111111111111111";
        const string replayDescriptorFingerprint="2222222222222222222222222222222222222222222222222222222222222222";
        const string provenanceFingerprint="3333333333333333333333333333333333333333333333333333333333333333";

        var measurement=new ProductionMeasurementPcbBinding(
            1,
            inputFingerprint,
            Asun.Domain.Pcb.PcbFeatureId.Create("U1-ID"),
            "U1",
            "4444444444444444444444444444444444444444444444444444444444444444",
            "5555555555555555555555555555555555555555555555555555555555555555",
            "6666666666666666666666666666666666666666666666666666666666666666");

        var provenance=new ProductionFrameProvenance(
            FrameSequence.Create(1),
            1280,
            960,
            "Gray8",
            DateTimeOffset.UnixEpoch.AddSeconds(1),
            inputFingerprint);

        var descriptor=new ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptor(
            1,
            inputFingerprint,
            1280,
            960,
            "Gray8",
            provenance.CapturedAtUtc,
            replayDescriptorFingerprint,
            provenanceFingerprint);

        return new PcbMeasurementProvenanceReplayFixture(
            measurement,
            provenance,
            descriptor);
    }
}
