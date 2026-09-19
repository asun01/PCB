using Asun.Device.Contracts;

namespace Asun.Device.Impl;

public sealed record CaptureSessionSnapshot(
    long CapturedCount,
    FrameSequence? FirstSequence,
    FrameSequence? LastSequence,
    IReadOnlyList<string> Fingerprints);

public static class CaptureSessionRuntime
{
    public static async ValueTask<CaptureSessionSnapshot> CaptureAsync(
        IFrameSource source,
        int frameCount,
        CancellationToken cancellationToken=default)
    {
        ArgumentNullException.ThrowIfNull(source);

        if(frameCount<=0)
            throw new ArgumentOutOfRangeException(nameof(frameCount));

        var fingerprints=new List<string>(frameCount);
        FrameSequence? first=null;
        FrameSequence? last=null;

        for(var i=0;i<frameCount;i++)
        {
            var frame=await source.CaptureAsync(cancellationToken);

            if(frame is null)
                throw new InvalidOperationException("Frame source returned no frame before the requested count was reached.");

            if(!CapturedFrameValidationRuntime.IsValid(frame))
                throw new InvalidOperationException("Frame source returned an invalid frame.");

            first ??= frame.Metadata.Sequence;
            last=frame.Metadata.Sequence;
            fingerprints.Add(frame.PayloadFingerprint);
        }

        return new CaptureSessionSnapshot(
            frameCount,
            first,
            last,
            fingerprints);
    }
}
