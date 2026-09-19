namespace Asun.Platform.RenderIntegration;

public sealed record ProductionRenderFrameProjection(
    Guid ProductionSessionId,
    string ProductionFingerprint,
    IReadOnlyList<ProductionRenderFrameEntry> Frames,
    string Fingerprint);
