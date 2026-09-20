using Asun.Device.Contracts;
using Asun.Production.Runtime;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ProductionSessionProgressMetadata3HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,1,FrameSequence.Create(1))
            {
                Width=10,
                Height=20,
                PixelFormat="Mono16"
            };
            Check(progress.Width==10 &&
                  progress.Height==20 &&
                  progress.PixelFormat=="Mono16",
                  "non-Gray8 frame format must remain opaque in the progress contract");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,1,FrameSequence.Create(1))
            {
                Width=10,
                Height=20,
                PixelFormat="Mono16"
            };
            Check(progress.Width==10 &&
                  progress.Height==20 &&
                  progress.PixelFormat=="Mono16",
                  "non-Gray8 frame format must remain opaque in the progress contract");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,1,FrameSequence.Create(1))
            {
                Width=10,
                Height=20,
                PixelFormat="Mono16"
            };
            Check(progress.Width==10 &&
                  progress.Height==20 &&
                  progress.PixelFormat=="Mono16",
                  "non-Gray8 frame format must remain opaque in the progress contract");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,1,FrameSequence.Create(1))
            {
                Width=10,
                Height=20,
                PixelFormat="Mono16"
            };
            Check(progress.Width==10 &&
                  progress.Height==20 &&
                  progress.PixelFormat=="Mono16",
                  "non-Gray8 frame format must remain opaque in the progress contract");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,1,FrameSequence.Create(1))
            {
                Width=10,
                Height=20,
                PixelFormat="Mono16"
            };
            Check(progress.Width==10 &&
                  progress.Height==20 &&
                  progress.PixelFormat=="Mono16",
                  "non-Gray8 frame format must remain opaque in the progress contract");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,1,FrameSequence.Create(1))
            {
                Width=10,
                Height=20,
                PixelFormat="Mono16"
            };
            Check(progress.Width==10 &&
                  progress.Height==20 &&
                  progress.PixelFormat=="Mono16",
                  "non-Gray8 frame format must remain opaque in the progress contract");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,1,FrameSequence.Create(1))
            {
                Width=10,
                Height=20,
                PixelFormat="Mono16"
            };
            Check(progress.Width==10 &&
                  progress.Height==20 &&
                  progress.PixelFormat=="Mono16",
                  "non-Gray8 frame format must remain opaque in the progress contract");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,1,FrameSequence.Create(1))
            {
                Width=10,
                Height=20,
                PixelFormat="Mono16"
            };
            Check(progress.Width==10 &&
                  progress.Height==20 &&
                  progress.PixelFormat=="Mono16",
                  "non-Gray8 frame format must remain opaque in the progress contract");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,1,FrameSequence.Create(1))
            {
                Width=10,
                Height=20,
                PixelFormat="Mono16"
            };
            Check(progress.Width==10 &&
                  progress.Height==20 &&
                  progress.PixelFormat=="Mono16",
                  "non-Gray8 frame format must remain opaque in the progress contract");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var progress=new ProductionSessionProgress(
                Guid.NewGuid(),1,1,FrameSequence.Create(1))
            {
                Width=10,
                Height=20,
                PixelFormat="Mono16"
            };
            Check(progress.Width==10 &&
                  progress.Height==20 &&
                  progress.PixelFormat=="Mono16",
                  "non-Gray8 frame format must remain opaque in the progress contract");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
