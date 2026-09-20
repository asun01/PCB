using System.Numerics;
using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientRoiImageSizeBinding5HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var workspace=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(200,200));
            workspace.SetImageSize(new Vector2(64,32));
            workspace.ResizeViewport(new Vector2(320,160));
            var snapshot=workspace.CaptureViewportSnapshot();
            Check(snapshot.Transform.ImageSize==new Vector2(64,32) &&
                  snapshot.Transform.ViewportSize==new Vector2(320,160) &&
                  snapshot.Transform.IsImageFullyVisible,
                  "ROI image size and viewport resize must compose deterministically");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var workspace=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(200,200));
            workspace.SetImageSize(new Vector2(64,32));
            workspace.ResizeViewport(new Vector2(320,160));
            var snapshot=workspace.CaptureViewportSnapshot();
            Check(snapshot.Transform.ImageSize==new Vector2(64,32) &&
                  snapshot.Transform.ViewportSize==new Vector2(320,160) &&
                  snapshot.Transform.IsImageFullyVisible,
                  "ROI image size and viewport resize must compose deterministically");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var workspace=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(200,200));
            workspace.SetImageSize(new Vector2(64,32));
            workspace.ResizeViewport(new Vector2(320,160));
            var snapshot=workspace.CaptureViewportSnapshot();
            Check(snapshot.Transform.ImageSize==new Vector2(64,32) &&
                  snapshot.Transform.ViewportSize==new Vector2(320,160) &&
                  snapshot.Transform.IsImageFullyVisible,
                  "ROI image size and viewport resize must compose deterministically");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var workspace=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(200,200));
            workspace.SetImageSize(new Vector2(64,32));
            workspace.ResizeViewport(new Vector2(320,160));
            var snapshot=workspace.CaptureViewportSnapshot();
            Check(snapshot.Transform.ImageSize==new Vector2(64,32) &&
                  snapshot.Transform.ViewportSize==new Vector2(320,160) &&
                  snapshot.Transform.IsImageFullyVisible,
                  "ROI image size and viewport resize must compose deterministically");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var workspace=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(200,200));
            workspace.SetImageSize(new Vector2(64,32));
            workspace.ResizeViewport(new Vector2(320,160));
            var snapshot=workspace.CaptureViewportSnapshot();
            Check(snapshot.Transform.ImageSize==new Vector2(64,32) &&
                  snapshot.Transform.ViewportSize==new Vector2(320,160) &&
                  snapshot.Transform.IsImageFullyVisible,
                  "ROI image size and viewport resize must compose deterministically");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var workspace=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(200,200));
            workspace.SetImageSize(new Vector2(64,32));
            workspace.ResizeViewport(new Vector2(320,160));
            var snapshot=workspace.CaptureViewportSnapshot();
            Check(snapshot.Transform.ImageSize==new Vector2(64,32) &&
                  snapshot.Transform.ViewportSize==new Vector2(320,160) &&
                  snapshot.Transform.IsImageFullyVisible,
                  "ROI image size and viewport resize must compose deterministically");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var workspace=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(200,200));
            workspace.SetImageSize(new Vector2(64,32));
            workspace.ResizeViewport(new Vector2(320,160));
            var snapshot=workspace.CaptureViewportSnapshot();
            Check(snapshot.Transform.ImageSize==new Vector2(64,32) &&
                  snapshot.Transform.ViewportSize==new Vector2(320,160) &&
                  snapshot.Transform.IsImageFullyVisible,
                  "ROI image size and viewport resize must compose deterministically");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var workspace=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(200,200));
            workspace.SetImageSize(new Vector2(64,32));
            workspace.ResizeViewport(new Vector2(320,160));
            var snapshot=workspace.CaptureViewportSnapshot();
            Check(snapshot.Transform.ImageSize==new Vector2(64,32) &&
                  snapshot.Transform.ViewportSize==new Vector2(320,160) &&
                  snapshot.Transform.IsImageFullyVisible,
                  "ROI image size and viewport resize must compose deterministically");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var workspace=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(200,200));
            workspace.SetImageSize(new Vector2(64,32));
            workspace.ResizeViewport(new Vector2(320,160));
            var snapshot=workspace.CaptureViewportSnapshot();
            Check(snapshot.Transform.ImageSize==new Vector2(64,32) &&
                  snapshot.Transform.ViewportSize==new Vector2(320,160) &&
                  snapshot.Transform.IsImageFullyVisible,
                  "ROI image size and viewport resize must compose deterministically");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var workspace=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(200,200));
            workspace.SetImageSize(new Vector2(64,32));
            workspace.ResizeViewport(new Vector2(320,160));
            var snapshot=workspace.CaptureViewportSnapshot();
            Check(snapshot.Transform.ImageSize==new Vector2(64,32) &&
                  snapshot.Transform.ViewportSize==new Vector2(320,160) &&
                  snapshot.Transform.IsImageFullyVisible,
                  "ROI image size and viewport resize must compose deterministically");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
