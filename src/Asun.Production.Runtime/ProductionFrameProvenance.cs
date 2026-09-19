using Asun.Device.Contracts;

namespace Asun.Production.Runtime;

public sealed record ProductionFrameProvenance(
    FrameSequence Sequence,
    long Width,
    long Height,
    string PixelFormat,
    DateTimeOffset CapturedAtUtc,
    string PayloadFingerprint);
