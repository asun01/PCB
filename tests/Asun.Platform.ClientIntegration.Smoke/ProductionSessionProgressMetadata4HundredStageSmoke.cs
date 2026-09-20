using Asun.Device.Contracts;
using Asun.Production.Runtime;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ProductionSessionProgressMetadata4HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),3,3,FrameSequence.Create(3))
            {
                Width=100,
                Height=80,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==progress.TotalFrames &&
                  progress.LastSequence?.Value==3 &&
                  progress.Width==100 &&
                  progress.Height==80,
                  "terminal Production progress must retain terminal frame metadata");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),3,3,FrameSequence.Create(3))
            {
                Width=100,
                Height=80,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==progress.TotalFrames &&
                  progress.LastSequence?.Value==3 &&
                  progress.Width==100 &&
                  progress.Height==80,
                  "terminal Production progress must retain terminal frame metadata");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),3,3,FrameSequence.Create(3))
            {
                Width=100,
                Height=80,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==progress.TotalFrames &&
                  progress.LastSequence?.Value==3 &&
                  progress.Width==100 &&
                  progress.Height==80,
                  "terminal Production progress must retain terminal frame metadata");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),3,3,FrameSequence.Create(3))
            {
                Width=100,
                Height=80,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==progress.TotalFrames &&
                  progress.LastSequence?.Value==3 &&
                  progress.Width==100 &&
                  progress.Height==80,
                  "terminal Production progress must retain terminal frame metadata");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),3,3,FrameSequence.Create(3))
            {
                Width=100,
                Height=80,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==progress.TotalFrames &&
                  progress.LastSequence?.Value==3 &&
                  progress.Width==100 &&
                  progress.Height==80,
                  "terminal Production progress must retain terminal frame metadata");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),3,3,FrameSequence.Create(3))
            {
                Width=100,
                Height=80,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==progress.TotalFrames &&
                  progress.LastSequence?.Value==3 &&
                  progress.Width==100 &&
                  progress.Height==80,
                  "terminal Production progress must retain terminal frame metadata");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),3,3,FrameSequence.Create(3))
            {
                Width=100,
                Height=80,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==progress.TotalFrames &&
                  progress.LastSequence?.Value==3 &&
                  progress.Width==100 &&
                  progress.Height==80,
                  "terminal Production progress must retain terminal frame metadata");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),3,3,FrameSequence.Create(3))
            {
                Width=100,
                Height=80,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==progress.TotalFrames &&
                  progress.LastSequence?.Value==3 &&
                  progress.Width==100 &&
                  progress.Height==80,
                  "terminal Production progress must retain terminal frame metadata");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),3,3,FrameSequence.Create(3))
            {
                Width=100,
                Height=80,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==progress.TotalFrames &&
                  progress.LastSequence?.Value==3 &&
                  progress.Width==100 &&
                  progress.Height==80,
                  "terminal Production progress must retain terminal frame metadata");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),3,3,FrameSequence.Create(3))
            {
                Width=100,
                Height=80,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==progress.TotalFrames &&
                  progress.LastSequence?.Value==3 &&
                  progress.Width==100 &&
                  progress.Height==80,
                  "terminal Production progress must retain terminal frame metadata");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
