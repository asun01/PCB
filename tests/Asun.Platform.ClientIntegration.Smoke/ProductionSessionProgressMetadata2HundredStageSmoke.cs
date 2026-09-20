using Asun.Device.Contracts;
using Asun.Production.Runtime;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ProductionSessionProgressMetadata2HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),2,3,FrameSequence.Create(2))
            {
                Width=32,
                Height=24,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==2 &&
                  progress.TotalFrames==3 &&
                  progress.LastSequence?.Value==2,
                  "Production progress must preserve frame completion semantics with metadata");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),2,3,FrameSequence.Create(2))
            {
                Width=32,
                Height=24,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==2 &&
                  progress.TotalFrames==3 &&
                  progress.LastSequence?.Value==2,
                  "Production progress must preserve frame completion semantics with metadata");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),2,3,FrameSequence.Create(2))
            {
                Width=32,
                Height=24,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==2 &&
                  progress.TotalFrames==3 &&
                  progress.LastSequence?.Value==2,
                  "Production progress must preserve frame completion semantics with metadata");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),2,3,FrameSequence.Create(2))
            {
                Width=32,
                Height=24,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==2 &&
                  progress.TotalFrames==3 &&
                  progress.LastSequence?.Value==2,
                  "Production progress must preserve frame completion semantics with metadata");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),2,3,FrameSequence.Create(2))
            {
                Width=32,
                Height=24,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==2 &&
                  progress.TotalFrames==3 &&
                  progress.LastSequence?.Value==2,
                  "Production progress must preserve frame completion semantics with metadata");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),2,3,FrameSequence.Create(2))
            {
                Width=32,
                Height=24,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==2 &&
                  progress.TotalFrames==3 &&
                  progress.LastSequence?.Value==2,
                  "Production progress must preserve frame completion semantics with metadata");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),2,3,FrameSequence.Create(2))
            {
                Width=32,
                Height=24,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==2 &&
                  progress.TotalFrames==3 &&
                  progress.LastSequence?.Value==2,
                  "Production progress must preserve frame completion semantics with metadata");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),2,3,FrameSequence.Create(2))
            {
                Width=32,
                Height=24,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==2 &&
                  progress.TotalFrames==3 &&
                  progress.LastSequence?.Value==2,
                  "Production progress must preserve frame completion semantics with metadata");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),2,3,FrameSequence.Create(2))
            {
                Width=32,
                Height=24,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==2 &&
                  progress.TotalFrames==3 &&
                  progress.LastSequence?.Value==2,
                  "Production progress must preserve frame completion semantics with metadata");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),2,3,FrameSequence.Create(2))
            {
                Width=32,
                Height=24,
                PixelFormat="Gray8"
            };
            Check(progress.CompletedFrames==2 &&
                  progress.TotalFrames==3 &&
                  progress.LastSequence?.Value==2,
                  "Production progress must preserve frame completion semantics with metadata");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
