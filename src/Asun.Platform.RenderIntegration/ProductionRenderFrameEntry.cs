using Asun.UI.Viewports;

namespace Asun.Platform.RenderIntegration;

public sealed record ProductionRenderFrameEntry(
    long Sequence,
    string ProductionInputFingerprint,
    ViewportRenderFrameSummary RenderSummary);
