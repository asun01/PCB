using Asun.Platform.Evidence;
using Asun.Platform.RenderIntegration;

namespace Asun.Platform.RoiProductionIntegration.Smoke;

internal static class ProductionRoiRenderEvidenceReplaySmokeFixtures
{
    public static (
        Asun.Production.Runtime.ProductionSessionReport Production,
        ProductionRoiInteractionContext RoiContext,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> RenderFrames,
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> EvidenceDescriptors)
        Create()
    {
        var baseFixture=ProductionRoiRenderReplaySmokeFixtures.Create();
        var handles=new[]
        {
            EvidenceHandle.Create("roi-render-evidence/1"),
            EvidenceHandle.Create("roi-render-evidence/2")
        };
        var descriptors=ProductionRenderEvidenceReplayDescriptorRuntime.Create(
            baseFixture.RenderFrames,
            handles);

        return (
            baseFixture.Production,
            baseFixture.RoiContext,
            baseFixture.RenderFrames,
            descriptors);
    }
}
