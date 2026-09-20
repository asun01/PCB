using Asun.Platform.Evidence;

namespace Asun.Platform.CaptureEvidenceIntegration;

public sealed record ProductionCaptureEvidenceFrameReference(
    long Sequence,
    string PayloadFingerprint,
    long Width,
    long Height,
    string PixelFormat,
    IReadOnlyList<EvidenceHandle> Handles);
