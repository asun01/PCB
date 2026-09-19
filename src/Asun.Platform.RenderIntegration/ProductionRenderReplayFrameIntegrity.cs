namespace Asun.Platform.RenderIntegration;

public sealed record ProductionRenderReplayFrameIntegrity(
    long Sequence,
    string ProductionInputFingerprint,
    long RenderGeneration,
    string RenderFingerprint);
