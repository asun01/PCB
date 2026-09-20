using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientAcquisitionFaultState5HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(4,4),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            workspace.SetFault("fault");
            var faulted=workspace.Snapshot;
            workspace.Unbind();
            Check(faulted.State==ClientAcquisitionState.Faulted &&
                  faulted.Descriptor?.SourceId=="simulation" &&
                  workspace.Snapshot.State==ClientAcquisitionState.Unbound &&
                  workspace.Snapshot.Preview is null,
                  "Faulted Acquisition must retain source identity until unbound");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(4,4),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            workspace.SetFault("fault");
            var faulted=workspace.Snapshot;
            workspace.Unbind();
            Check(faulted.State==ClientAcquisitionState.Faulted &&
                  faulted.Descriptor?.SourceId=="simulation" &&
                  workspace.Snapshot.State==ClientAcquisitionState.Unbound &&
                  workspace.Snapshot.Preview is null,
                  "Faulted Acquisition must retain source identity until unbound");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(4,4),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            workspace.SetFault("fault");
            var faulted=workspace.Snapshot;
            workspace.Unbind();
            Check(faulted.State==ClientAcquisitionState.Faulted &&
                  faulted.Descriptor?.SourceId=="simulation" &&
                  workspace.Snapshot.State==ClientAcquisitionState.Unbound &&
                  workspace.Snapshot.Preview is null,
                  "Faulted Acquisition must retain source identity until unbound");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(4,4),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            workspace.SetFault("fault");
            var faulted=workspace.Snapshot;
            workspace.Unbind();
            Check(faulted.State==ClientAcquisitionState.Faulted &&
                  faulted.Descriptor?.SourceId=="simulation" &&
                  workspace.Snapshot.State==ClientAcquisitionState.Unbound &&
                  workspace.Snapshot.Preview is null,
                  "Faulted Acquisition must retain source identity until unbound");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(4,4),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            workspace.SetFault("fault");
            var faulted=workspace.Snapshot;
            workspace.Unbind();
            Check(faulted.State==ClientAcquisitionState.Faulted &&
                  faulted.Descriptor?.SourceId=="simulation" &&
                  workspace.Snapshot.State==ClientAcquisitionState.Unbound &&
                  workspace.Snapshot.Preview is null,
                  "Faulted Acquisition must retain source identity until unbound");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(4,4),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            workspace.SetFault("fault");
            var faulted=workspace.Snapshot;
            workspace.Unbind();
            Check(faulted.State==ClientAcquisitionState.Faulted &&
                  faulted.Descriptor?.SourceId=="simulation" &&
                  workspace.Snapshot.State==ClientAcquisitionState.Unbound &&
                  workspace.Snapshot.Preview is null,
                  "Faulted Acquisition must retain source identity until unbound");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(4,4),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            workspace.SetFault("fault");
            var faulted=workspace.Snapshot;
            workspace.Unbind();
            Check(faulted.State==ClientAcquisitionState.Faulted &&
                  faulted.Descriptor?.SourceId=="simulation" &&
                  workspace.Snapshot.State==ClientAcquisitionState.Unbound &&
                  workspace.Snapshot.Preview is null,
                  "Faulted Acquisition must retain source identity until unbound");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(4,4),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            workspace.SetFault("fault");
            var faulted=workspace.Snapshot;
            workspace.Unbind();
            Check(faulted.State==ClientAcquisitionState.Faulted &&
                  faulted.Descriptor?.SourceId=="simulation" &&
                  workspace.Snapshot.State==ClientAcquisitionState.Unbound &&
                  workspace.Snapshot.Preview is null,
                  "Faulted Acquisition must retain source identity until unbound");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(4,4),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            workspace.SetFault("fault");
            var faulted=workspace.Snapshot;
            workspace.Unbind();
            Check(faulted.State==ClientAcquisitionState.Faulted &&
                  faulted.Descriptor?.SourceId=="simulation" &&
                  workspace.Snapshot.State==ClientAcquisitionState.Unbound &&
                  workspace.Snapshot.Preview is null,
                  "Faulted Acquisition must retain source identity until unbound");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var workspace=new ClientAcquisitionWorkspace();
            workspace.Bind(
                new Asun.Device.Impl.SimulatedFrameSource(4,4),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            workspace.SetFault("fault");
            var faulted=workspace.Snapshot;
            workspace.Unbind();
            Check(faulted.State==ClientAcquisitionState.Faulted &&
                  faulted.Descriptor?.SourceId=="simulation" &&
                  workspace.Snapshot.State==ClientAcquisitionState.Unbound &&
                  workspace.Snapshot.Preview is null,
                  "Faulted Acquisition must retain source identity until unbound");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
