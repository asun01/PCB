namespace Asun.Device.Contracts;

public interface IFrameSource
{
    ValueTask<CapturedFrame?> CaptureAsync(
        CancellationToken cancellationToken=default);
}
