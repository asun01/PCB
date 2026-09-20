using System.Numerics;
using Asun.Platform.ClientIntegration;
using Asun.Platform.RoiProductionIntegration;
using Asun.UI.Viewports;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientRoiInteractionWorkspace5HundredStageSmoke
{
    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var i0=0;i0<10;i0++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                first.Load(firstDefinition);
                var firstReport=await first.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var left=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                using var right=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                left.BindProductionReport(firstReport);
                right.BindProductionReport(firstReport);
                var stableId=Guid.Parse("75000000-0000-0000-0000-000000000002");
                left.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                right.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                var l=left.Capture();
                var r=right.Capture();
                Check(l.RoiFingerprint==r.RoiFingerprint &&
                      l.InteractionFingerprint==r.InteractionFingerprint,
                      "identical client ROI documents with stable identity should converge deterministically");
        }
        for(var i1=0;i1<10;i1++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                first.Load(firstDefinition);
                var firstReport=await first.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var left=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                using var right=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                left.BindProductionReport(firstReport);
                right.BindProductionReport(firstReport);
                var stableId=Guid.Parse("75000000-0000-0000-0000-000000000002");
                left.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                right.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                var l=left.Capture();
                var r=right.Capture();
                Check(l.RoiFingerprint==r.RoiFingerprint &&
                      l.InteractionFingerprint==r.InteractionFingerprint,
                      "identical client ROI documents with stable identity should converge deterministically");
        }
        for(var i2=0;i2<10;i2++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                first.Load(firstDefinition);
                var firstReport=await first.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var left=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                using var right=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                left.BindProductionReport(firstReport);
                right.BindProductionReport(firstReport);
                var stableId=Guid.Parse("75000000-0000-0000-0000-000000000002");
                left.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                right.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                var l=left.Capture();
                var r=right.Capture();
                Check(l.RoiFingerprint==r.RoiFingerprint &&
                      l.InteractionFingerprint==r.InteractionFingerprint,
                      "identical client ROI documents with stable identity should converge deterministically");
        }
        for(var i3=0;i3<10;i3++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                first.Load(firstDefinition);
                var firstReport=await first.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var left=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                using var right=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                left.BindProductionReport(firstReport);
                right.BindProductionReport(firstReport);
                var stableId=Guid.Parse("75000000-0000-0000-0000-000000000002");
                left.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                right.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                var l=left.Capture();
                var r=right.Capture();
                Check(l.RoiFingerprint==r.RoiFingerprint &&
                      l.InteractionFingerprint==r.InteractionFingerprint,
                      "identical client ROI documents with stable identity should converge deterministically");
        }
        for(var i4=0;i4<10;i4++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                first.Load(firstDefinition);
                var firstReport=await first.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var left=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                using var right=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                left.BindProductionReport(firstReport);
                right.BindProductionReport(firstReport);
                var stableId=Guid.Parse("75000000-0000-0000-0000-000000000002");
                left.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                right.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                var l=left.Capture();
                var r=right.Capture();
                Check(l.RoiFingerprint==r.RoiFingerprint &&
                      l.InteractionFingerprint==r.InteractionFingerprint,
                      "identical client ROI documents with stable identity should converge deterministically");
        }
        for(var i5=0;i5<10;i5++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                first.Load(firstDefinition);
                var firstReport=await first.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var left=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                using var right=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                left.BindProductionReport(firstReport);
                right.BindProductionReport(firstReport);
                var stableId=Guid.Parse("75000000-0000-0000-0000-000000000002");
                left.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                right.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                var l=left.Capture();
                var r=right.Capture();
                Check(l.RoiFingerprint==r.RoiFingerprint &&
                      l.InteractionFingerprint==r.InteractionFingerprint,
                      "identical client ROI documents with stable identity should converge deterministically");
        }
        for(var i6=0;i6<10;i6++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                first.Load(firstDefinition);
                var firstReport=await first.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var left=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                using var right=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                left.BindProductionReport(firstReport);
                right.BindProductionReport(firstReport);
                var stableId=Guid.Parse("75000000-0000-0000-0000-000000000002");
                left.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                right.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                var l=left.Capture();
                var r=right.Capture();
                Check(l.RoiFingerprint==r.RoiFingerprint &&
                      l.InteractionFingerprint==r.InteractionFingerprint,
                      "identical client ROI documents with stable identity should converge deterministically");
        }
        for(var i7=0;i7<10;i7++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                first.Load(firstDefinition);
                var firstReport=await first.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var left=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                using var right=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                left.BindProductionReport(firstReport);
                right.BindProductionReport(firstReport);
                var stableId=Guid.Parse("75000000-0000-0000-0000-000000000002");
                left.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                right.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                var l=left.Capture();
                var r=right.Capture();
                Check(l.RoiFingerprint==r.RoiFingerprint &&
                      l.InteractionFingerprint==r.InteractionFingerprint,
                      "identical client ROI documents with stable identity should converge deterministically");
        }
        for(var i8=0;i8<10;i8++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                first.Load(firstDefinition);
                var firstReport=await first.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var left=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                using var right=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                left.BindProductionReport(firstReport);
                right.BindProductionReport(firstReport);
                var stableId=Guid.Parse("75000000-0000-0000-0000-000000000002");
                left.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                right.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                var l=left.Capture();
                var r=right.Capture();
                Check(l.RoiFingerprint==r.RoiFingerprint &&
                      l.InteractionFingerprint==r.InteractionFingerprint,
                      "identical client ROI documents with stable identity should converge deterministically");
        }
        for(var i9=0;i9<10;i9++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                first.Load(firstDefinition);
                var firstReport=await first.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                using var left=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                using var right=new ClientRoiInteractionWorkspace(new Vector2(100,100),new Vector2(400,400));
                left.BindProductionReport(firstReport);
                right.BindProductionReport(firstReport);
                var stableId=Guid.Parse("75000000-0000-0000-0000-000000000002");
                left.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                right.AddRectangle(new Vector2(50,50),new Vector2(20,20),stableId);
                var l=left.Capture();
                var r=right.Capture();
                Check(l.RoiFingerprint==r.RoiFingerprint &&
                      l.InteractionFingerprint==r.InteractionFingerprint,
                      "identical client ROI documents with stable identity should converge deterministically");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("client ROI workspace smoke must execute exactly 100 rounds");
    }
}
