using Asun.Device.Contracts;
using Asun.Device.Impl;

public static class SimulatedFrameSourceHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var source=new SimulatedFrameSource(16,8,"Gray8");

        for(var i=0;i<10;i++)
        {
            var frame=await source.CaptureAsync();
            Check(frame is not null,"Simulated source should produce a frame.");
        }

        for(var i=0;i<10;i++)
        {
            var frame=await source.CaptureAsync();
            Check(frame is not null && frame.Metadata.Sequence.IsValid,"Frame sequence should be valid.");
        }

        for(var i=0;i<10;i++)
        {
            var frame=await source.CaptureAsync();
            Check(frame is not null && frame.Metadata.Width==16 && frame.Metadata.Height==8,"Frame dimensions should remain configured.");
        }

        for(var i=0;i<10;i++)
        {
            var frame=await source.CaptureAsync();
            Check(frame is not null && frame.Payload.Length==128,"Payload size should match width times height.");
        }

        for(var i=0;i<10;i++)
        {
            var frame=await source.CaptureAsync();
            Check(frame is not null && CapturedFrameValidationRuntime.IsValid(frame),"Simulated frame should validate.");
        }

        for(var i=0;i<10;i++)
        {
            var first=await source.CaptureAsync();
            var second=await source.CaptureAsync();
            Check(first is not null && second is not null && second.Metadata.Sequence.Value==first.Metadata.Sequence.Value+1,"Sequences should advance exactly by one.");
        }

        for(var i=0;i<10;i++)
        {
            var frame=await source.CaptureAsync();
            Check(frame is not null && frame.PayloadFingerprint.Length==64,"Payload fingerprint should be SHA-256 sized.");
        }

        for(var i=0;i<10;i++)
        {
            var frame=await source.CaptureAsync();
            Check(frame is not null && frame.Payload.SequenceEqual(frame.Payload.ToArray()),"Frame payload should be stable for the captured frame instance.");
        }

        for(var i=0;i<10;i++)
        {
            var frame=await source.CaptureAsync();
            Check(frame is not null && frame.Metadata.CapturedAtUtc==DateTimeOffset.UnixEpoch.AddMilliseconds(frame.Metadata.Sequence.Value),"Simulation timestamp should derive deterministically from sequence.");
        }

        for(var i=0;i<10;i++)
        {
            var frame=await source.CaptureAsync();
            Check(frame is not null && frame.Metadata.PixelFormat=="Gray8","Simulation pixel format should remain explicit.");
        }

        assert(round==100,$"Simulated frame source smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
