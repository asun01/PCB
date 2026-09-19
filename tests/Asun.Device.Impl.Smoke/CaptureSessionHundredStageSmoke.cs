using Asun.Device.Contracts;
using Asun.Device.Impl;

public static class CaptureSessionHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        for(var i=0;i<10;i++)
        {
            var source=new SimulatedFrameSource(8,8);
            var session=await CaptureSessionRuntime.CaptureAsync(source,5);
            Check(session.CapturedCount==5,"Capture session should contain the requested frame count.");
        }

        for(var i=0;i<10;i++)
        {
            var source=new SimulatedFrameSource(8,8);
            var session=await CaptureSessionRuntime.CaptureAsync(source,5);
            Check(session.FirstSequence?.Value==1 && session.LastSequence?.Value==5,"Session sequence range should start at one and end at the requested count.");
        }

        for(var i=0;i<10;i++)
        {
            var source=new SimulatedFrameSource(8,8);
            var session=await CaptureSessionRuntime.CaptureAsync(source,3);
            Check(session.Fingerprints.Count==3 && session.Fingerprints.All(value=>value.Length==64),"Session should preserve one fingerprint per frame.");
        }

        for(var i=0;i<10;i++)
        {
            var source=new SimulatedFrameSource(4,4);
            var session=await CaptureSessionRuntime.CaptureAsync(source,2);
            Check(session.Fingerprints[0]!=session.Fingerprints[1],"Sequential simulated frames should have distinct payload fingerprints.");
        }

        for(var i=0;i<10;i++)
        {
            using var cancellation=new CancellationTokenSource();
            cancellation.Cancel();
            var threw=false;
            try
            {
                await CaptureSessionRuntime.CaptureAsync(new SimulatedFrameSource(4,4),2,cancellation.Token);
            }
            catch(OperationCanceledException)
            {
                threw=true;
            }

            Check(threw,"Capture session should honor cancellation.");
        }

        for(var i=0;i<10;i++)
        {
            var source=new SimulatedFrameSource(2,2);
            var session=await CaptureSessionRuntime.CaptureAsync(source,1);
            Check(session.FirstSequence==session.LastSequence,"Single-frame session should have identical first and last sequence.");
        }

        for(var i=0;i<10;i++)
        {
            var source=new SimulatedFrameSource(2,2);
            var session=await CaptureSessionRuntime.CaptureAsync(source,4);
            Check(session.Fingerprints.Distinct(StringComparer.Ordinal).Count()==4,"Session payload fingerprints should remain unique for sequential simulation.");
        }

        for(var i=0;i<10;i++)
        {
            var source=new SimulatedFrameSource(2,2);
            var session=await CaptureSessionRuntime.CaptureAsync(source,2);
            Check(session.Fingerprints.All(value=>value.All(Uri.IsHexDigit)),"Session fingerprints should remain hexadecimal.");
        }

        for(var i=0;i<10;i++)
        {
            var source=new SimulatedFrameSource(2,2);
            var session=await CaptureSessionRuntime.CaptureAsync(source,2);
            Check(session.FirstSequence is not null && session.LastSequence is not null,"Session sequence boundaries should be present.");
        }

        for(var i=0;i<10;i++)
        {
            var source=new SimulatedFrameSource(2,2);
            var session=await CaptureSessionRuntime.CaptureAsync(source,2);
            Check(session.LastSequence!.Value-session.FirstSequence!.Value==1,"Two-frame session sequence span should be one.");
        }

        assert(round==100,$"Capture session smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
