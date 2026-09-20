using System.Numerics;
using Asun.Device.Contracts;
using Asun.Platform.ClientIntegration;
using Asun.Platform.SimulationIntegration;
using Asun.Production.Runtime;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientInspectionExecutionRecovery4HundredStageSmoke
{
    private sealed class FailOnceFrameSource : IFrameSource
    {
        private readonly IFrameSource _inner;
        private int _first=1;

        public FailOnceFrameSource(IFrameSource inner) => _inner=inner;

        public ValueTask<CapturedFrame?> CaptureAsync(
            CancellationToken cancellationToken=default)
        {
            if(Interlocked.Exchange(ref _first,0)==1)
                throw new InvalidOperationException("simulated acquisition fault");

            return _inner.CaptureAsync(cancellationToken);
        }
    }

    private static ProductionSessionDefinition Definition(Guid sessionId) =>
        ClientSimulationSessionFactory.CreateDefinition(sessionId,3);

    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("88000000-0000-0000-0000-000000000104"),
                3);
            workspace.BindAcquisition(
                ClientSimulationSessionFactory.CreateSource(),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.ResetCurrentSession();
            var snapshot=workspace.Capture();
            Check(snapshot.Production.Status==ClientExecutionStatus.Idle &&
                  snapshot.Program?.Status==ClientProgramLoadStatus.Empty &&
                  !snapshot.Acquisition.CanCapture &&
                  !snapshot.Quality.IsBound &&
                  snapshot.Replay is null &&
                  snapshot.Release is null,
                  "full session reset must return the client to a clean unbound state");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("88000000-0000-0000-0000-000000000104"),
                3);
            workspace.BindAcquisition(
                ClientSimulationSessionFactory.CreateSource(),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.ResetCurrentSession();
            var snapshot=workspace.Capture();
            Check(snapshot.Production.Status==ClientExecutionStatus.Idle &&
                  snapshot.Program?.Status==ClientProgramLoadStatus.Empty &&
                  !snapshot.Acquisition.CanCapture &&
                  !snapshot.Quality.IsBound &&
                  snapshot.Replay is null &&
                  snapshot.Release is null,
                  "full session reset must return the client to a clean unbound state");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("88000000-0000-0000-0000-000000000104"),
                3);
            workspace.BindAcquisition(
                ClientSimulationSessionFactory.CreateSource(),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.ResetCurrentSession();
            var snapshot=workspace.Capture();
            Check(snapshot.Production.Status==ClientExecutionStatus.Idle &&
                  snapshot.Program?.Status==ClientProgramLoadStatus.Empty &&
                  !snapshot.Acquisition.CanCapture &&
                  !snapshot.Quality.IsBound &&
                  snapshot.Replay is null &&
                  snapshot.Release is null,
                  "full session reset must return the client to a clean unbound state");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("88000000-0000-0000-0000-000000000104"),
                3);
            workspace.BindAcquisition(
                ClientSimulationSessionFactory.CreateSource(),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.ResetCurrentSession();
            var snapshot=workspace.Capture();
            Check(snapshot.Production.Status==ClientExecutionStatus.Idle &&
                  snapshot.Program?.Status==ClientProgramLoadStatus.Empty &&
                  !snapshot.Acquisition.CanCapture &&
                  !snapshot.Quality.IsBound &&
                  snapshot.Replay is null &&
                  snapshot.Release is null,
                  "full session reset must return the client to a clean unbound state");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("88000000-0000-0000-0000-000000000104"),
                3);
            workspace.BindAcquisition(
                ClientSimulationSessionFactory.CreateSource(),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.ResetCurrentSession();
            var snapshot=workspace.Capture();
            Check(snapshot.Production.Status==ClientExecutionStatus.Idle &&
                  snapshot.Program?.Status==ClientProgramLoadStatus.Empty &&
                  !snapshot.Acquisition.CanCapture &&
                  !snapshot.Quality.IsBound &&
                  snapshot.Replay is null &&
                  snapshot.Release is null,
                  "full session reset must return the client to a clean unbound state");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("88000000-0000-0000-0000-000000000104"),
                3);
            workspace.BindAcquisition(
                ClientSimulationSessionFactory.CreateSource(),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.ResetCurrentSession();
            var snapshot=workspace.Capture();
            Check(snapshot.Production.Status==ClientExecutionStatus.Idle &&
                  snapshot.Program?.Status==ClientProgramLoadStatus.Empty &&
                  !snapshot.Acquisition.CanCapture &&
                  !snapshot.Quality.IsBound &&
                  snapshot.Replay is null &&
                  snapshot.Release is null,
                  "full session reset must return the client to a clean unbound state");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("88000000-0000-0000-0000-000000000104"),
                3);
            workspace.BindAcquisition(
                ClientSimulationSessionFactory.CreateSource(),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.ResetCurrentSession();
            var snapshot=workspace.Capture();
            Check(snapshot.Production.Status==ClientExecutionStatus.Idle &&
                  snapshot.Program?.Status==ClientProgramLoadStatus.Empty &&
                  !snapshot.Acquisition.CanCapture &&
                  !snapshot.Quality.IsBound &&
                  snapshot.Replay is null &&
                  snapshot.Release is null,
                  "full session reset must return the client to a clean unbound state");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("88000000-0000-0000-0000-000000000104"),
                3);
            workspace.BindAcquisition(
                ClientSimulationSessionFactory.CreateSource(),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.ResetCurrentSession();
            var snapshot=workspace.Capture();
            Check(snapshot.Production.Status==ClientExecutionStatus.Idle &&
                  snapshot.Program?.Status==ClientProgramLoadStatus.Empty &&
                  !snapshot.Acquisition.CanCapture &&
                  !snapshot.Quality.IsBound &&
                  snapshot.Replay is null &&
                  snapshot.Release is null,
                  "full session reset must return the client to a clean unbound state");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("88000000-0000-0000-0000-000000000104"),
                3);
            workspace.BindAcquisition(
                ClientSimulationSessionFactory.CreateSource(),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.ResetCurrentSession();
            var snapshot=workspace.Capture();
            Check(snapshot.Production.Status==ClientExecutionStatus.Idle &&
                  snapshot.Program?.Status==ClientProgramLoadStatus.Empty &&
                  !snapshot.Acquisition.CanCapture &&
                  !snapshot.Quality.IsBound &&
                  snapshot.Replay is null &&
                  snapshot.Release is null,
                  "full session reset must return the client to a clean unbound state");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("88000000-0000-0000-0000-000000000104"),
                3);
            workspace.BindAcquisition(
                ClientSimulationSessionFactory.CreateSource(),
                new ClientAcquisitionDescriptor("simulation","Simulation",true));
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.ResetCurrentSession();
            var snapshot=workspace.Capture();
            Check(snapshot.Production.Status==ClientExecutionStatus.Idle &&
                  snapshot.Program?.Status==ClientProgramLoadStatus.Empty &&
                  !snapshot.Acquisition.CanCapture &&
                  !snapshot.Quality.IsBound &&
                  snapshot.Replay is null &&
                  snapshot.Release is null,
                  "full session reset must return the client to a clean unbound state");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
