namespace Asun.Device.Contracts;

public sealed record FrameCaptureMetadata(
    FrameSequence Sequence,
    long Width,
    long Height,
    string PixelFormat,
    DateTimeOffset CapturedAtUtc)
{
    public bool IsValid=>
        Sequence.IsValid &&
        Width>0 &&
        Height>0 &&
        !string.IsNullOrWhiteSpace(PixelFormat);
}
