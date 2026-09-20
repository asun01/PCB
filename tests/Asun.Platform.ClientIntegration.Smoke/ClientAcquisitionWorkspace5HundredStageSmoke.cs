using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientAcquisitionWorkspace5HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation Source",true));
            var sourceAvailable=workspace.TryGetSource(out var source);
            Check(sourceAvailable &&
                  source is Asun.Device.Impl.SimulatedFrameSource &&
                  workspace.Snapshot.CanCapture,
                  "bound Acquisition workspace must expose the existing frame-source port");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation Source",true));
            var sourceAvailable=workspace.TryGetSource(out var source);
            Check(sourceAvailable &&
                  source is Asun.Device.Impl.SimulatedFrameSource &&
                  workspace.Snapshot.CanCapture,
                  "bound Acquisition workspace must expose the existing frame-source port");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation Source",true));
            var sourceAvailable=workspace.TryGetSource(out var source);
            Check(sourceAvailable &&
                  source is Asun.Device.Impl.SimulatedFrameSource &&
                  workspace.Snapshot.CanCapture,
                  "bound Acquisition workspace must expose the existing frame-source port");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation Source",true));
            var sourceAvailable=workspace.TryGetSource(out var source);
            Check(sourceAvailable &&
                  source is Asun.Device.Impl.SimulatedFrameSource &&
                  workspace.Snapshot.CanCapture,
                  "bound Acquisition workspace must expose the existing frame-source port");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation Source",true));
            var sourceAvailable=workspace.TryGetSource(out var source);
            Check(sourceAvailable &&
                  source is Asun.Device.Impl.SimulatedFrameSource &&
                  workspace.Snapshot.CanCapture,
                  "bound Acquisition workspace must expose the existing frame-source port");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation Source",true));
            var sourceAvailable=workspace.TryGetSource(out var source);
            Check(sourceAvailable &&
                  source is Asun.Device.Impl.SimulatedFrameSource &&
                  workspace.Snapshot.CanCapture,
                  "bound Acquisition workspace must expose the existing frame-source port");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation Source",true));
            var sourceAvailable=workspace.TryGetSource(out var source);
            Check(sourceAvailable &&
                  source is Asun.Device.Impl.SimulatedFrameSource &&
                  workspace.Snapshot.CanCapture,
                  "bound Acquisition workspace must expose the existing frame-source port");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation Source",true));
            var sourceAvailable=workspace.TryGetSource(out var source);
            Check(sourceAvailable &&
                  source is Asun.Device.Impl.SimulatedFrameSource &&
                  workspace.Snapshot.CanCapture,
                  "bound Acquisition workspace must expose the existing frame-source port");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation Source",true));
            var sourceAvailable=workspace.TryGetSource(out var source);
            Check(sourceAvailable &&
                  source is Asun.Device.Impl.SimulatedFrameSource &&
                  workspace.Snapshot.CanCapture,
                  "bound Acquisition workspace must expose the existing frame-source port");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(8,8),
                new ClientAcquisitionDescriptor("simulation","Simulation Source",true));
            var sourceAvailable=workspace.TryGetSource(out var source);
            Check(sourceAvailable &&
                  source is Asun.Device.Impl.SimulatedFrameSource &&
                  workspace.Snapshot.CanCapture,
                  "bound Acquisition workspace must expose the existing frame-source port");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
