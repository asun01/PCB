namespace Asun.Device.Contracts;

/// <summary>
/// Vendor-neutral acquisition request. Hardware-specific configuration belongs
/// behind adapters and is intentionally absent here.
/// </summary>
public sealed record AcquisitionRequest(
    string DeviceId,
    DateTimeOffset RequestedAtUtc);

public sealed record AcquisitionFrame(
    string DeviceId,
    long Sequence,
    DateTimeOffset CapturedAtUtc,
    int Width,
    int Height);

public interface IFrameSource
{
    ValueTask<AcquisitionFrame> AcquireAsync(
        AcquisitionRequest request,
        CancellationToken cancellationToken = default);
}
