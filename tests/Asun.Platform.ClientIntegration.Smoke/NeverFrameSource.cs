using Asun.Device.Contracts;

namespace Asun.Platform.ClientIntegration.Smoke;

internal sealed class NeverFrameSource : IFrameSource
{
    public ValueTask<CapturedFrame?> CaptureAsync(
        CancellationToken cancellationToken=default)=>
        ValueTask.FromResult<CapturedFrame?>(null);
}
