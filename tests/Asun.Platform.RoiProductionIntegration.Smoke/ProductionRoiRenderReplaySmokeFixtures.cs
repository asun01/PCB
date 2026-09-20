using Asun.Platform.RenderIntegration;
using Asun.Platform.RoiProductionIntegration;
using Asun.UI.Viewports;

namespace Asun.Platform.RoiProductionIntegration.Smoke;

internal static class ProductionRoiRenderReplaySmokeFixtures
{
    public static (
        ProductionSessionReport Production,
        ProductionRoiInteractionContext RoiContext,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> RenderFrames)
        Create()
    {
        var baseFixture=ProductionRoiInteractionContextSmokeFixtures.Create();
        var roiContext=ProductionRoiInteractionContextRuntime.Create(
            baseFixture.Production,
            baseFixture.RoiSnapshot);

        var summaries=new[]
        {
            new ViewportRenderFrameSummary(1,2,1,1,1,0,1,0),
            new ViewportRenderFrameSummary(2,3,1,1,1,1,2,1)
        };

        var renderFrames=ProductionRenderReplayFrameIntegrityRuntime.CreateFrames(
            baseFixture.Production,
            summaries);

        return (baseFixture.Production,roiContext,renderFrames);
    }
}
