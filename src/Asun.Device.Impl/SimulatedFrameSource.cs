using Asun.Device.Contracts;

namespace Asun.Device.Impl;

/// <summary>
/// Deterministic frame-source adapter for development and replay-free pipeline
/// testing. It does not emulate vendor behavior or image pixels.
/// </summary>
public sealed class SimulatedFrameSource : IFrameSource
{
    private readonly int _width;
    private readonly int _height;
    private long _sequence;

    public SimulatedFrameSource(int width, int height)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        _width = width;
        _height = height;
    }

    public ValueTask<AcquisitionFrame> AcquireAsync(
        AcquisitionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var sequence = Interlocked.Increment(ref _sequence);

        return ValueTask.FromResult(new AcquisitionFrame(
            request.DeviceId,
            sequence,
            request.RequestedAtUtc,
            _width,
            _height));
    }
}
