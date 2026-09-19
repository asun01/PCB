using Asun.Device.Contracts;

namespace Asun.Production.Runtime;

public sealed class RecordingFrameSource : IFrameSource
{
    private readonly IFrameSource _inner;
    private readonly List<CapturedFrame> _frames=new();

    public RecordingFrameSource(IFrameSource inner)
    {
        _inner=inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public IReadOnlyList<CapturedFrame> Frames=>_frames.AsReadOnly();

    public async ValueTask<CapturedFrame?> CaptureAsync(
        CancellationToken cancellationToken=default)
    {
        var frame=await _inner.CaptureAsync(cancellationToken);
        if(frame is null)
            return null;

        var owned=CapturedFrame.Create(
            frame.Metadata,
            frame.Payload);
        _frames.Add(owned);
        return owned;
    }
}
