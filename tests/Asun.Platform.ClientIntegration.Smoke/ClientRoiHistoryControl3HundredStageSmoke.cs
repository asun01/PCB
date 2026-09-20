using System.Numerics;
using Asun.Platform.ClientIntegration;
using Asun.UI.Viewports;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientRoiHistoryControl3HundredStageSmoke
{
    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var i0=0;i0<10;i0++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
                await client.ExecuteAsync(
                    new Asun.Device.Impl.SimulatedFrameSource(8,8),
                    ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                client.SetRoiMode(RoiEditorMode.CreateRectangle);
                client.SubmitRoiInput(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                client.SubmitRoiInput(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                client.UndoRoi();
                var redone=client.RedoRoi();
                var snapshot=client.Capture();
                Check(redone &&
                      snapshot.CanUndoRoi &&
                      !snapshot.CanRedoRoi &&
                      snapshot.Roi!.RoiCount==1,
                      "Redo should restore the ROI and consume the Redo state");
        }
        for(var i1=0;i1<10;i1++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
                await client.ExecuteAsync(
                    new Asun.Device.Impl.SimulatedFrameSource(8,8),
                    ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                client.SetRoiMode(RoiEditorMode.CreateRectangle);
                client.SubmitRoiInput(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                client.SubmitRoiInput(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                client.UndoRoi();
                var redone=client.RedoRoi();
                var snapshot=client.Capture();
                Check(redone &&
                      snapshot.CanUndoRoi &&
                      !snapshot.CanRedoRoi &&
                      snapshot.Roi!.RoiCount==1,
                      "Redo should restore the ROI and consume the Redo state");
        }
        for(var i2=0;i2<10;i2++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
                await client.ExecuteAsync(
                    new Asun.Device.Impl.SimulatedFrameSource(8,8),
                    ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                client.SetRoiMode(RoiEditorMode.CreateRectangle);
                client.SubmitRoiInput(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                client.SubmitRoiInput(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                client.UndoRoi();
                var redone=client.RedoRoi();
                var snapshot=client.Capture();
                Check(redone &&
                      snapshot.CanUndoRoi &&
                      !snapshot.CanRedoRoi &&
                      snapshot.Roi!.RoiCount==1,
                      "Redo should restore the ROI and consume the Redo state");
        }
        for(var i3=0;i3<10;i3++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
                await client.ExecuteAsync(
                    new Asun.Device.Impl.SimulatedFrameSource(8,8),
                    ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                client.SetRoiMode(RoiEditorMode.CreateRectangle);
                client.SubmitRoiInput(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                client.SubmitRoiInput(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                client.UndoRoi();
                var redone=client.RedoRoi();
                var snapshot=client.Capture();
                Check(redone &&
                      snapshot.CanUndoRoi &&
                      !snapshot.CanRedoRoi &&
                      snapshot.Roi!.RoiCount==1,
                      "Redo should restore the ROI and consume the Redo state");
        }
        for(var i4=0;i4<10;i4++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
                await client.ExecuteAsync(
                    new Asun.Device.Impl.SimulatedFrameSource(8,8),
                    ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                client.SetRoiMode(RoiEditorMode.CreateRectangle);
                client.SubmitRoiInput(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                client.SubmitRoiInput(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                client.UndoRoi();
                var redone=client.RedoRoi();
                var snapshot=client.Capture();
                Check(redone &&
                      snapshot.CanUndoRoi &&
                      !snapshot.CanRedoRoi &&
                      snapshot.Roi!.RoiCount==1,
                      "Redo should restore the ROI and consume the Redo state");
        }
        for(var i5=0;i5<10;i5++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
                await client.ExecuteAsync(
                    new Asun.Device.Impl.SimulatedFrameSource(8,8),
                    ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                client.SetRoiMode(RoiEditorMode.CreateRectangle);
                client.SubmitRoiInput(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                client.SubmitRoiInput(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                client.UndoRoi();
                var redone=client.RedoRoi();
                var snapshot=client.Capture();
                Check(redone &&
                      snapshot.CanUndoRoi &&
                      !snapshot.CanRedoRoi &&
                      snapshot.Roi!.RoiCount==1,
                      "Redo should restore the ROI and consume the Redo state");
        }
        for(var i6=0;i6<10;i6++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
                await client.ExecuteAsync(
                    new Asun.Device.Impl.SimulatedFrameSource(8,8),
                    ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                client.SetRoiMode(RoiEditorMode.CreateRectangle);
                client.SubmitRoiInput(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                client.SubmitRoiInput(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                client.UndoRoi();
                var redone=client.RedoRoi();
                var snapshot=client.Capture();
                Check(redone &&
                      snapshot.CanUndoRoi &&
                      !snapshot.CanRedoRoi &&
                      snapshot.Roi!.RoiCount==1,
                      "Redo should restore the ROI and consume the Redo state");
        }
        for(var i7=0;i7<10;i7++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
                await client.ExecuteAsync(
                    new Asun.Device.Impl.SimulatedFrameSource(8,8),
                    ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                client.SetRoiMode(RoiEditorMode.CreateRectangle);
                client.SubmitRoiInput(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                client.SubmitRoiInput(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                client.UndoRoi();
                var redone=client.RedoRoi();
                var snapshot=client.Capture();
                Check(redone &&
                      snapshot.CanUndoRoi &&
                      !snapshot.CanRedoRoi &&
                      snapshot.Roi!.RoiCount==1,
                      "Redo should restore the ROI and consume the Redo state");
        }
        for(var i8=0;i8<10;i8++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
                await client.ExecuteAsync(
                    new Asun.Device.Impl.SimulatedFrameSource(8,8),
                    ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                client.SetRoiMode(RoiEditorMode.CreateRectangle);
                client.SubmitRoiInput(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                client.SubmitRoiInput(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                client.UndoRoi();
                var redone=client.RedoRoi();
                var snapshot=client.Capture();
                Check(redone &&
                      snapshot.CanUndoRoi &&
                      !snapshot.CanRedoRoi &&
                      snapshot.Roi!.RoiCount==1,
                      "Redo should restore the ROI and consume the Redo state");
        }
        for(var i9=0;i9<10;i9++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
                await client.ExecuteAsync(
                    new Asun.Device.Impl.SimulatedFrameSource(8,8),
                    ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                client.SetRoiMode(RoiEditorMode.CreateRectangle);
                client.SubmitRoiInput(ViewportInputEventKind.PointerDown,new Vector2(100,100));
                client.SubmitRoiInput(ViewportInputEventKind.PointerUp,new Vector2(200,200));
                client.UndoRoi();
                var redone=client.RedoRoi();
                var snapshot=client.Capture();
                Check(redone &&
                      snapshot.CanUndoRoi &&
                      !snapshot.CanRedoRoi &&
                      snapshot.Roi!.RoiCount==1,
                      "Redo should restore the ROI and consume the Redo state");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("client ROI history smoke must execute exactly 100 rounds");
    }
}
