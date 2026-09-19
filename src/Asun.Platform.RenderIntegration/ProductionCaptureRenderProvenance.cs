using Asun.UI.Viewports;

namespace Asun.Platform.RenderIntegration;

public sealed record ProductionCaptureRenderProvenance(
    long Sequence,
    string PayloadFingerprint,
    long Width,
    long Height,
    string PixelFormat,
    long RenderGeneration,
    ViewportRenderFrameSummary RenderSummary,
    string RenderFingerprint);
