using System.Numerics;
using Asun.Platform.ClientIntegration;
using Asun.Platform.RoiProductionIntegration;
using Asun.UI.Viewports;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientRoiInteractionWorkspace2HundredStageSmoke
{
    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var i0=0;i0<10;i0++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var roi=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                roi.BindProductionReport(report);
                roi.Mode=RoiEditorMode.CreateRectangle;
                var down=roi.Submit(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                var move=roi.Submit(ViewportInputEventKind.PointerMove,new Vector2(200,200));
                var up=roi.Submit(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                var snapshot=roi.Capture();
                Check(down && move && up &&
                      snapshot.RoiCount==1 &&
                      snapshot.SelectedRoiId is Guid selected &&
                      selected!=Guid.Empty &&
                      snapshot.ProcessedEvents>=3,
                      "ROI pointer interaction should create and process one ROI inside the client workspace");
        }
        for(var i1=0;i1<10;i1++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var roi=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                roi.BindProductionReport(report);
                roi.Mode=RoiEditorMode.CreateRectangle;
                var down=roi.Submit(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                var move=roi.Submit(ViewportInputEventKind.PointerMove,new Vector2(200,200));
                var up=roi.Submit(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                var snapshot=roi.Capture();
                Check(down && move && up &&
                      snapshot.RoiCount==1 &&
                      snapshot.SelectedRoiId is Guid selected &&
                      selected!=Guid.Empty &&
                      snapshot.ProcessedEvents>=3,
                      "ROI pointer interaction should create and process one ROI inside the client workspace");
        }
        for(var i2=0;i2<10;i2++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var roi=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                roi.BindProductionReport(report);
                roi.Mode=RoiEditorMode.CreateRectangle;
                var down=roi.Submit(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                var move=roi.Submit(ViewportInputEventKind.PointerMove,new Vector2(200,200));
                var up=roi.Submit(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                var snapshot=roi.Capture();
                Check(down && move && up &&
                      snapshot.RoiCount==1 &&
                      snapshot.SelectedRoiId is Guid selected &&
                      selected!=Guid.Empty &&
                      snapshot.ProcessedEvents>=3,
                      "ROI pointer interaction should create and process one ROI inside the client workspace");
        }
        for(var i3=0;i3<10;i3++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var roi=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                roi.BindProductionReport(report);
                roi.Mode=RoiEditorMode.CreateRectangle;
                var down=roi.Submit(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                var move=roi.Submit(ViewportInputEventKind.PointerMove,new Vector2(200,200));
                var up=roi.Submit(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                var snapshot=roi.Capture();
                Check(down && move && up &&
                      snapshot.RoiCount==1 &&
                      snapshot.SelectedRoiId is Guid selected &&
                      selected!=Guid.Empty &&
                      snapshot.ProcessedEvents>=3,
                      "ROI pointer interaction should create and process one ROI inside the client workspace");
        }
        for(var i4=0;i4<10;i4++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var roi=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                roi.BindProductionReport(report);
                roi.Mode=RoiEditorMode.CreateRectangle;
                var down=roi.Submit(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                var move=roi.Submit(ViewportInputEventKind.PointerMove,new Vector2(200,200));
                var up=roi.Submit(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                var snapshot=roi.Capture();
                Check(down && move && up &&
                      snapshot.RoiCount==1 &&
                      snapshot.SelectedRoiId is Guid selected &&
                      selected!=Guid.Empty &&
                      snapshot.ProcessedEvents>=3,
                      "ROI pointer interaction should create and process one ROI inside the client workspace");
        }
        for(var i5=0;i5<10;i5++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var roi=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                roi.BindProductionReport(report);
                roi.Mode=RoiEditorMode.CreateRectangle;
                var down=roi.Submit(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                var move=roi.Submit(ViewportInputEventKind.PointerMove,new Vector2(200,200));
                var up=roi.Submit(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                var snapshot=roi.Capture();
                Check(down && move && up &&
                      snapshot.RoiCount==1 &&
                      snapshot.SelectedRoiId is Guid selected &&
                      selected!=Guid.Empty &&
                      snapshot.ProcessedEvents>=3,
                      "ROI pointer interaction should create and process one ROI inside the client workspace");
        }
        for(var i6=0;i6<10;i6++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var roi=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                roi.BindProductionReport(report);
                roi.Mode=RoiEditorMode.CreateRectangle;
                var down=roi.Submit(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                var move=roi.Submit(ViewportInputEventKind.PointerMove,new Vector2(200,200));
                var up=roi.Submit(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                var snapshot=roi.Capture();
                Check(down && move && up &&
                      snapshot.RoiCount==1 &&
                      snapshot.SelectedRoiId is Guid selected &&
                      selected!=Guid.Empty &&
                      snapshot.ProcessedEvents>=3,
                      "ROI pointer interaction should create and process one ROI inside the client workspace");
        }
        for(var i7=0;i7<10;i7++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var roi=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                roi.BindProductionReport(report);
                roi.Mode=RoiEditorMode.CreateRectangle;
                var down=roi.Submit(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                var move=roi.Submit(ViewportInputEventKind.PointerMove,new Vector2(200,200));
                var up=roi.Submit(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                var snapshot=roi.Capture();
                Check(down && move && up &&
                      snapshot.RoiCount==1 &&
                      snapshot.SelectedRoiId is Guid selected &&
                      selected!=Guid.Empty &&
                      snapshot.ProcessedEvents>=3,
                      "ROI pointer interaction should create and process one ROI inside the client workspace");
        }
        for(var i8=0;i8<10;i8++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var roi=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                roi.BindProductionReport(report);
                roi.Mode=RoiEditorMode.CreateRectangle;
                var down=roi.Submit(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                var move=roi.Submit(ViewportInputEventKind.PointerMove,new Vector2(200,200));
                var up=roi.Submit(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                var snapshot=roi.Capture();
                Check(down && move && up &&
                      snapshot.RoiCount==1 &&
                      snapshot.SelectedRoiId is Guid selected &&
                      selected!=Guid.Empty &&
                      snapshot.ProcessedEvents>=3,
                      "ROI pointer interaction should create and process one ROI inside the client workspace");
        }
        for(var i9=0;i9<10;i9++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var roi=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                roi.BindProductionReport(report);
                roi.Mode=RoiEditorMode.CreateRectangle;
                var down=roi.Submit(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                var move=roi.Submit(ViewportInputEventKind.PointerMove,new Vector2(200,200));
                var up=roi.Submit(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                var snapshot=roi.Capture();
                Check(down && move && up &&
                      snapshot.RoiCount==1 &&
                      snapshot.SelectedRoiId is Guid selected &&
                      selected!=Guid.Empty &&
                      snapshot.ProcessedEvents>=3,
                      "ROI pointer interaction should create and process one ROI inside the client workspace");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("client ROI workspace smoke must execute exactly 100 rounds");
    }
}
