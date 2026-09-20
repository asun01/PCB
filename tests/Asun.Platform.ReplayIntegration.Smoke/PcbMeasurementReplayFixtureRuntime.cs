using Asun.Platform.MetrologyProductionIntegration;
using Asun.Platform.PcbExecutionIntegration;

namespace Asun.Platform.ReplayIntegration.Smoke;

public sealed record PcbMeasurementReplayFixture(
    PcbExecutionMeasurementComponentBinding ExecutionBinding,
    IReadOnlyList<ProductionMeasurementPcbBinding> MeasurementBindings,
    IReadOnlyList<ProductionMeasurementQualityEvidenceReleaseReplayDescriptor> ReplayDescriptors);

public static class PcbMeasurementReplayFixtureRuntime
{
    public static PcbMeasurementReplayFixture Create()
    {
        var assembly=new PcbAssemblySnapshot(
            null!,
            Array.Empty<Asun.Domain.Pcb.PcbComponentReference>(),
            null!,
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

        var execution=new PcbExecutionMeasurementComponentBinding(
            assembly.Fingerprint,
            Guid.Parse("e1000000-0000-0000-0000-000000000001"),
            2,
            new[]
            {
                Asun.Domain.Pcb.PcbFeatureId.Create("C1-ID"),
                Asun.Domain.Pcb.PcbFeatureId.Create("R1-ID")
            },
            new[]{"C1","R1"},
            "1111111111111111111111111111111111111111111111111111111111111111",
            "2222222222222222222222222222222222222222222222222222222222222222");

        var measurements=new[]
        {
            new ProductionMeasurementPcbBinding(
                1,
                "3333333333333333333333333333333333333333333333333333333333333333",
                Asun.Domain.Pcb.PcbFeatureId.Create("C1-ID"),
                "C1",
                "4444444444444444444444444444444444444444444444444444444444444444",
                "5555555555555555555555555555555555555555555555555555555555555555",
                "6666666666666666666666666666666666666666666666666666666666666666"),
            new ProductionMeasurementPcbBinding(
                2,
                "7777777777777777777777777777777777777777777777777777777777777777",
                Asun.Domain.Pcb.PcbFeatureId.Create("R1-ID"),
                "R1",
                "8888888888888888888888888888888888888888888888888888888888888888",
                "9999999999999999999999999999999999999999999999999999999999999999",
                "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")
        };

        var descriptors=new[]
        {
            new ProductionMeasurementQualityEvidenceReleaseReplayDescriptor(
                execution.ProductionSessionId,
                Guid.Parse("e2000000-0000-0000-0000-000000000001"),
                1,
                measurements[0].ProductionInputFingerprint,
                Guid.Parse("e3000000-0000-0000-0000-000000000001"),
                "C1-ID",
                "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
                "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",
                "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd",
                true,
                "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee",
                "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff"),
            new ProductionMeasurementQualityEvidenceReleaseReplayDescriptor(
                execution.ProductionSessionId,
                Guid.Parse("e2000000-0000-0000-0000-000000000001"),
                2,
                measurements[1].ProductionInputFingerprint,
                Guid.Parse("e4000000-0000-0000-0000-000000000001"),
                "R1-ID",
                "1111111111111111111111111111111111111111111111111111111111111111",
                "2222222222222222222222222222222222222222222222222222222222222222",
                "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd",
                true,
                "3333333333333333333333333333333333333333333333333333333333333333",
                "4444444444444444444444444444444444444444444444444444444444444444")
        };

        return new PcbMeasurementReplayFixture(execution,measurements,descriptors);
    }
}
