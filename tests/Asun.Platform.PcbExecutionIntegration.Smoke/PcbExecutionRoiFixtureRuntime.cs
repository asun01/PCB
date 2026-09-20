using Asun.Platform.RoiProductionIntegration;

namespace Asun.Platform.PcbExecutionIntegration.Smoke;

public sealed record PcbExecutionRoiFixture(
    PcbExecutionSnapshot ExecutionSnapshot,
    ProductionRoiInteractionContext RoiContext);

public static class PcbExecutionRoiFixtureRuntime
{
    public static PcbExecutionRoiFixture Create()
    {
        var execution=new PcbExecutionSnapshot(
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            Guid.Parse("b1000000-0000-0000-0000-000000000001"),
            "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
            "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",
            2,
            2,
            Guid.Parse("b2000000-0000-0000-0000-000000000001"),
            "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd",
            "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");

        var roi=new ProductionRoiInteractionContext(
            execution.ProductionSessionId,
            execution.FrameCount,
            1,
            2,
            execution.ProductionFingerprint,
            "1111111111111111111111111111111111111111111111111111111111111111",
            "2222222222222222222222222222222222222222222222222222222222222222",
            "3333333333333333333333333333333333333333333333333333333333333333",
            1,
            Guid.Parse("b3000000-0000-0000-0000-000000000001"),
            "4444444444444444444444444444444444444444444444444444444444444444");

        return new PcbExecutionRoiFixture(execution,roi);
    }
}
