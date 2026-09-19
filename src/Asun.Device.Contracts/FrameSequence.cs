namespace Asun.Device.Contracts;

public readonly record struct FrameSequence(long Value)
{
    public bool IsValid=>Value>0;

    public static FrameSequence Create(long value)
    {
        if(value<=0)
            throw new ArgumentOutOfRangeException(nameof(value));
        return new FrameSequence(value);
    }
}
