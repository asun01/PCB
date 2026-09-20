namespace Asun.Platform.RoiProductionIntegration.Smoke;

public sealed record RoiInputRenderReplayFixture(
    ProductionRoiInteractionContext RoiContext,
    ProductionRoiRenderReplayContext RenderContext);

public static class RoiInputRenderReplayFixtureRuntime
{
    public static RoiInputRenderReplayFixture Create()
    {
        var session=Guid.Parse("d1000000-0000-0000-0000-000000000001");
        var roi=new ProductionRoiInteractionContext(
            session,
            2,
            1,
            2,
            "1111111111111111111111111111111111111111111111111111111111111111",
            "2222222222222222222222222222222222222222222222222222222222222222",
            "3333333333333333333333333333333333333333333333333333333333333333",
            1,
            Guid.Parse("d2000000-0000-0000-0000-000000000001"),
            "4444444444444444444444444444444444444444444444444444444444444444");

        var render=new ProductionRoiRenderReplayContext(
            session,
            roi.BindingFingerprint,
            2,
            1,
            2,
            "5555555555555555555555555555555555555555555555555555555555555555",
            "6666666666666666666666666666666666666666666666666666666666666666");

        return new RoiInputRenderReplayFixture(roi,render);
    }
}
