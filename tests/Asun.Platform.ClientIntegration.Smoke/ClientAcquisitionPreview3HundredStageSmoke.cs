using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientAcquisitionPreview3HundredStageSmoke
{
    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            var preview=await workspace.PreviewAsync();
            Check(workspace.Snapshot.State==ClientAcquisitionState.Ready &&
                  workspace.Snapshot.CanCapture &&
                  workspace.Snapshot.Preview?.PayloadFingerprint==preview.PayloadFingerprint,
                  "successful preview must preserve Ready Acquisition state and latest payload identity");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            var preview=await workspace.PreviewAsync();
            Check(workspace.Snapshot.State==ClientAcquisitionState.Ready &&
                  workspace.Snapshot.CanCapture &&
                  workspace.Snapshot.Preview?.PayloadFingerprint==preview.PayloadFingerprint,
                  "successful preview must preserve Ready Acquisition state and latest payload identity");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            var preview=await workspace.PreviewAsync();
            Check(workspace.Snapshot.State==ClientAcquisitionState.Ready &&
                  workspace.Snapshot.CanCapture &&
                  workspace.Snapshot.Preview?.PayloadFingerprint==preview.PayloadFingerprint,
                  "successful preview must preserve Ready Acquisition state and latest payload identity");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            var preview=await workspace.PreviewAsync();
            Check(workspace.Snapshot.State==ClientAcquisitionState.Ready &&
                  workspace.Snapshot.CanCapture &&
                  workspace.Snapshot.Preview?.PayloadFingerprint==preview.PayloadFingerprint,
                  "successful preview must preserve Ready Acquisition state and latest payload identity");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            var preview=await workspace.PreviewAsync();
            Check(workspace.Snapshot.State==ClientAcquisitionState.Ready &&
                  workspace.Snapshot.CanCapture &&
                  workspace.Snapshot.Preview?.PayloadFingerprint==preview.PayloadFingerprint,
                  "successful preview must preserve Ready Acquisition state and latest payload identity");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            var preview=await workspace.PreviewAsync();
            Check(workspace.Snapshot.State==ClientAcquisitionState.Ready &&
                  workspace.Snapshot.CanCapture &&
                  workspace.Snapshot.Preview?.PayloadFingerprint==preview.PayloadFingerprint,
                  "successful preview must preserve Ready Acquisition state and latest payload identity");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            var preview=await workspace.PreviewAsync();
            Check(workspace.Snapshot.State==ClientAcquisitionState.Ready &&
                  workspace.Snapshot.CanCapture &&
                  workspace.Snapshot.Preview?.PayloadFingerprint==preview.PayloadFingerprint,
                  "successful preview must preserve Ready Acquisition state and latest payload identity");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            var preview=await workspace.PreviewAsync();
            Check(workspace.Snapshot.State==ClientAcquisitionState.Ready &&
                  workspace.Snapshot.CanCapture &&
                  workspace.Snapshot.Preview?.PayloadFingerprint==preview.PayloadFingerprint,
                  "successful preview must preserve Ready Acquisition state and latest payload identity");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            var preview=await workspace.PreviewAsync();
            Check(workspace.Snapshot.State==ClientAcquisitionState.Ready &&
                  workspace.Snapshot.CanCapture &&
                  workspace.Snapshot.Preview?.PayloadFingerprint==preview.PayloadFingerprint,
                  "successful preview must preserve Ready Acquisition state and latest payload identity");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            var preview=await workspace.PreviewAsync();
            Check(workspace.Snapshot.State==ClientAcquisitionState.Ready &&
                  workspace.Snapshot.CanCapture &&
                  workspace.Snapshot.Preview?.PayloadFingerprint==preview.PayloadFingerprint,
                  "successful preview must preserve Ready Acquisition state and latest payload identity");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
