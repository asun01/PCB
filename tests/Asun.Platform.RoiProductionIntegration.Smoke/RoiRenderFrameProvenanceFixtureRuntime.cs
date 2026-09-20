using Asun.Device.Contracts;
using Asun.Platform.Pipeline;
using Asun.Platform.RenderIntegration;
using Asun.Production.Runtime;
using Asun.UI.Viewports;

namespace Asun.Platform.RoiProductionIntegration.Smoke;

public sealed record RoiRenderFrameProvenanceFixture(
    ProductionSessionReport ProductionReport,
    ProductionRoiInteractionContext RoiContext,
    ProductionFrameProvenance Provenance,
    ProductionRenderReplayFrameIntegrity RenderFrame);

public static class RoiRenderFrameProvenanceFixtureRuntime
{
    public static RoiRenderFrameProvenanceFixture Create()
    {
        var session=Guid.Parse("c1000000-0000-0000-0000-000000000001");
        var input="1111111111111111111111111111111111111111111111111111111111111111";

        var report=new ProductionSessionReport(
            session,
            "program",
            1,
            new[]
            {
                new ProductionFrameExecution(
                    FrameSequence.Create(1),
                    input,
                    new PipelineExecutionReport(1,new[]{"Acquire"},"pipeline"))
            },
            "2222222222222222222222222222222222222222222222222222222222222222");

        var roi=new ProductionRoiInteractionContext(
            session,
            1,
            1,
            1,
            report.Fingerprint,
            "3333333333333333333333333333333333333333333333333333333333333333",
            "4444444444444444444444444444444444444444444444444444444444444444",
            1,
            Guid.Parse("c2000000-0000-0000-0000-000000000001"),
            "5555555555555555555555555555555555555555555555555555555555555555");

        var provenance=new ProductionFrameProvenance(
            FrameSequence.Create(1),
            1920,
            1080,
            "Gray8",
            DateTimeOffset.UnixEpoch.AddSeconds(1),
            input);

        var summary=new ViewportRenderFrameSummary(
            1,
            10,
            1,
            1,
            1,
            0,
            1,
            0);

        var render=new ProductionRenderReplayFrameIntegrity(
            1,
            input,
            summary,
            ViewportRenderFrameFingerprintRuntime.CreateFingerprint(summary));

        return new RoiRenderFrameProvenanceFixture(report,roi,provenance,render);
    }
}
