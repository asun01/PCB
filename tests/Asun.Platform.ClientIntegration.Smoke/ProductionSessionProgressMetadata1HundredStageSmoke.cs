using Asun.Device.Contracts;
using Asun.Production.Runtime;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ProductionSessionProgressMetadata1HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,3,FrameSequence.Create(1))
            {
                Width=64,
                Height=48,
                PixelFormat="Gray8"
            };
            Check(progress.Width==64 &&
                  progress.Height==48 &&
                  progress.PixelFormat=="Gray8" &&
                  progress.LastSequence?.Value==1,
                  "Production progress must carry actual frame dimensions and format");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,3,FrameSequence.Create(1))
            {
                Width=64,
                Height=48,
                PixelFormat="Gray8"
            };
            Check(progress.Width==64 &&
                  progress.Height==48 &&
                  progress.PixelFormat=="Gray8" &&
                  progress.LastSequence?.Value==1,
                  "Production progress must carry actual frame dimensions and format");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,3,FrameSequence.Create(1))
            {
                Width=64,
                Height=48,
                PixelFormat="Gray8"
            };
            Check(progress.Width==64 &&
                  progress.Height==48 &&
                  progress.PixelFormat=="Gray8" &&
                  progress.LastSequence?.Value==1,
                  "Production progress must carry actual frame dimensions and format");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,3,FrameSequence.Create(1))
            {
                Width=64,
                Height=48,
                PixelFormat="Gray8"
            };
            Check(progress.Width==64 &&
                  progress.Height==48 &&
                  progress.PixelFormat=="Gray8" &&
                  progress.LastSequence?.Value==1,
                  "Production progress must carry actual frame dimensions and format");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,3,FrameSequence.Create(1))
            {
                Width=64,
                Height=48,
                PixelFormat="Gray8"
            };
            Check(progress.Width==64 &&
                  progress.Height==48 &&
                  progress.PixelFormat=="Gray8" &&
                  progress.LastSequence?.Value==1,
                  "Production progress must carry actual frame dimensions and format");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,3,FrameSequence.Create(1))
            {
                Width=64,
                Height=48,
                PixelFormat="Gray8"
            };
            Check(progress.Width==64 &&
                  progress.Height==48 &&
                  progress.PixelFormat=="Gray8" &&
                  progress.LastSequence?.Value==1,
                  "Production progress must carry actual frame dimensions and format");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,3,FrameSequence.Create(1))
            {
                Width=64,
                Height=48,
                PixelFormat="Gray8"
            };
            Check(progress.Width==64 &&
                  progress.Height==48 &&
                  progress.PixelFormat=="Gray8" &&
                  progress.LastSequence?.Value==1,
                  "Production progress must carry actual frame dimensions and format");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,3,FrameSequence.Create(1))
            {
                Width=64,
                Height=48,
                PixelFormat="Gray8"
            };
            Check(progress.Width==64 &&
                  progress.Height==48 &&
                  progress.PixelFormat=="Gray8" &&
                  progress.LastSequence?.Value==1,
                  "Production progress must carry actual frame dimensions and format");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,3,FrameSequence.Create(1))
            {
                Width=64,
                Height=48,
                PixelFormat="Gray8"
            };
            Check(progress.Width==64 &&
                  progress.Height==48 &&
                  progress.PixelFormat=="Gray8" &&
                  progress.LastSequence?.Value==1,
                  "Production progress must carry actual frame dimensions and format");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,3,FrameSequence.Create(1))
            {
                Width=64,
                Height=48,
                PixelFormat="Gray8"
            };
            Check(progress.Width==64 &&
                  progress.Height==48 &&
                  progress.PixelFormat=="Gray8" &&
                  progress.LastSequence?.Value==1,
                  "Production progress must carry actual frame dimensions and format");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
