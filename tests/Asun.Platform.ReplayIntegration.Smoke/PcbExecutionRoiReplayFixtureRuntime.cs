using Asun.Platform.PcbExecutionIntegration;

namespace Asun.Platform.ReplayIntegration.Smoke;

public sealed record PcbExecutionRoiReplayFixture(
    PcbExecutionRoiContextBinding PcbRoiBinding,
    ProductionCapturePcbAuditReplayConvergence ReplayConvergence);

public static class PcbExecutionRoiReplayFixtureRuntime
{
    public static PcbExecutionRoiReplayFixture Create()
    {
        var pcbRoi=new PcbExecutionRoiContextBinding(
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            Guid.Parse("c1000000-0000-0000-0000-000000000001"),
            "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
            Guid.Parse("c2000000-0000-0000-0000-000000000001"),
            "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",
            "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd",
            "1111111111111111111111111111111111111111111111111111111111111111",
            "2222222222222222222222222222222222222222222222222222222222222222",
            1,
            Guid.Parse("c3000000-0000-0000-0000-000000000001"),
            "3333333333333333333333333333333333333333333333333333333333333333");

        var convergence=new ProductionCapturePcbAuditReplayConvergence(
            pcbRoi.ProductionSessionId,
            pcbRoi.QualityRunId,
            "4444444444444444444444444444444444444444444444444444444444444444",
            "5555555555555555555555555555555555555555555555555555555555555555",
            "6666666666666666666666666666666666666666666666666666666666666666",
            true,
            "logical/release-artifact",
            "7777777777777777777777777777777777777777777777777777777777777777");

        return new PcbExecutionRoiReplayFixture(pcbRoi,convergence);
    }
}
