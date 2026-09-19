using Asun.Device.Contracts;
using System.Security.Cryptography;

namespace Asun.Device.Impl;

public sealed class SimulatedFrameSource : IFrameSource
{
    private readonly int _width;
    private readonly int _height;
    private readonly string _pixelFormat;
    private long _sequence;

    public SimulatedFrameSource(
        int width,
        int height,
        string pixelFormat="Gray8")
    {
        if(width<=0)
            throw new ArgumentOutOfRangeException(nameof(width));
        if(height<=0)
            throw new ArgumentOutOfRangeException(nameof(height));
        if(string.IsNullOrWhiteSpace(pixelFormat))
            throw new ArgumentException("Pixel format cannot be blank.",nameof(pixelFormat));

        _width=width;
        _height=height;
        _pixelFormat=pixelFormat.Trim();
    }

    public ValueTask<CapturedFrame?> CaptureAsync(
        CancellationToken cancellationToken=default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sequence=FrameSequence.Create(
            Interlocked.Increment(ref _sequence));

        var payloadLength=checked(_width*_height);
        var payload=new byte[payloadLength];

        for(var index=0;index<payload.Length;index++)
            payload[index]=(byte)((index+sequence.Value)%256);

        var metadata=new FrameCaptureMetadata(
            sequence,
            _width,
            _height,
            _pixelFormat,
            DateTimeOffset.UnixEpoch.AddMilliseconds(sequence.Value));

        return ValueTask.FromResult<CapturedFrame?>(
            CapturedFrame.Create(metadata,payload));
    }
}
