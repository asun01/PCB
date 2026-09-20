using Asun.Domain.Pcb;
using Asun.Platform.MetrologyProductionIntegration;

namespace Asun.Platform.PcbExecutionIntegration.Smoke;

public sealed record PcbMeasurementComponentFixture(
    PcbExecutionSnapshot ExecutionSnapshot,
    PcbAssemblySnapshot Assembly,
    IReadOnlyList<ProductionMeasurementPcbBinding> MeasurementBindings);

public static class PcbMeasurementComponentFixtureRuntime
{
    public static PcbMeasurementComponentFixture Create()
    {
        var board=new PcbBoardDefinition(
            "FixtureBoard",
            2,
            100,
            100);

        var c1=new PcbComponentReference(
            PcbFeatureId.Create("C1-ID"),
            "C1",
            "100nF",
            "0402",
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(10,10),
            0);

        var c2=new PcbComponentReference(
            PcbFeatureId.Create("R1-ID"),
            "R1",
            "10K",
            "0402",
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(20,20),
            0);

        var assembly=PcbAssemblySnapshotRuntime.Create(
            board,
            new[] { c1, c2 });

        var execution=new PcbExecutionSnapshot(
            assembly.Fingerprint,
            Guid.Parse("d1000000-0000-0000-0000-000000000001"),
            "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
            "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",
            2,
            2,
            Guid.Parse("d2000000-0000-0000-0000-000000000001"),
            "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd",
            "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");

        var bindings=new[]
        {
            new ProductionMeasurementPcbBinding(
                1,
                "1111111111111111111111111111111111111111111111111111111111111111",
                c1.Id,
                c1.Designator,
                "2222222222222222222222222222222222222222222222222222222222222222",
                "3333333333333333333333333333333333333333333333333333333333333333",
                "4444444444444444444444444444444444444444444444444444444444444444"),
            new ProductionMeasurementPcbBinding(
                2,
                "5555555555555555555555555555555555555555555555555555555555555555",
                c2.Id,
                c2.Designator,
                "6666666666666666666666666666666666666666666666666666666666666666",
                "7777777777777777777777777777777777777777777777777777777777777777",
                "8888888888888888888888888888888888888888888888888888888888888888")
        };

        return new PcbMeasurementComponentFixture(execution,assembly,bindings);
    }
}
