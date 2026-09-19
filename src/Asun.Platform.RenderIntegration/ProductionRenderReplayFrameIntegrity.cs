using Asun.UI.Viewports;

namespace Asun.Platform.RenderIntegration;

public sealed record ProductionRenderReplayFrameIntegrity(
    long Sequence,
    string ProductionInputFingerprint,
    ViewportRenderFrameSummary RenderSummary,
    string RenderFingerprint);
