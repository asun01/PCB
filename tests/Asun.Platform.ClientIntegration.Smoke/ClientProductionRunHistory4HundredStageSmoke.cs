using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientProductionRunHistory4HundredStageSmoke
{
    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var i0=0;i0<10;i0++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                history.Append(replay,release);
                var tampered=replay with { ProgramId=Guid.NewGuid() };
                var rejected=false;
                try
                {
                    history.Append(tampered,release);
                }
                catch(ArgumentException)
                {
                    rejected=true;
                }
                Check(rejected && history.Capture().Entries.Count==1,
                      "cross-projection identity drift must be rejected before entering history");
        }
        for(var i1=0;i1<10;i1++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                history.Append(replay,release);
                var tampered=replay with { ProgramId=Guid.NewGuid() };
                var rejected=false;
                try
                {
                    history.Append(tampered,release);
                }
                catch(ArgumentException)
                {
                    rejected=true;
                }
                Check(rejected && history.Capture().Entries.Count==1,
                      "cross-projection identity drift must be rejected before entering history");
        }
        for(var i2=0;i2<10;i2++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                history.Append(replay,release);
                var tampered=replay with { ProgramId=Guid.NewGuid() };
                var rejected=false;
                try
                {
                    history.Append(tampered,release);
                }
                catch(ArgumentException)
                {
                    rejected=true;
                }
                Check(rejected && history.Capture().Entries.Count==1,
                      "cross-projection identity drift must be rejected before entering history");
        }
        for(var i3=0;i3<10;i3++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                history.Append(replay,release);
                var tampered=replay with { ProgramId=Guid.NewGuid() };
                var rejected=false;
                try
                {
                    history.Append(tampered,release);
                }
                catch(ArgumentException)
                {
                    rejected=true;
                }
                Check(rejected && history.Capture().Entries.Count==1,
                      "cross-projection identity drift must be rejected before entering history");
        }
        for(var i4=0;i4<10;i4++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                history.Append(replay,release);
                var tampered=replay with { ProgramId=Guid.NewGuid() };
                var rejected=false;
                try
                {
                    history.Append(tampered,release);
                }
                catch(ArgumentException)
                {
                    rejected=true;
                }
                Check(rejected && history.Capture().Entries.Count==1,
                      "cross-projection identity drift must be rejected before entering history");
        }
        for(var i5=0;i5<10;i5++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                history.Append(replay,release);
                var tampered=replay with { ProgramId=Guid.NewGuid() };
                var rejected=false;
                try
                {
                    history.Append(tampered,release);
                }
                catch(ArgumentException)
                {
                    rejected=true;
                }
                Check(rejected && history.Capture().Entries.Count==1,
                      "cross-projection identity drift must be rejected before entering history");
        }
        for(var i6=0;i6<10;i6++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                history.Append(replay,release);
                var tampered=replay with { ProgramId=Guid.NewGuid() };
                var rejected=false;
                try
                {
                    history.Append(tampered,release);
                }
                catch(ArgumentException)
                {
                    rejected=true;
                }
                Check(rejected && history.Capture().Entries.Count==1,
                      "cross-projection identity drift must be rejected before entering history");
        }
        for(var i7=0;i7<10;i7++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                history.Append(replay,release);
                var tampered=replay with { ProgramId=Guid.NewGuid() };
                var rejected=false;
                try
                {
                    history.Append(tampered,release);
                }
                catch(ArgumentException)
                {
                    rejected=true;
                }
                Check(rejected && history.Capture().Entries.Count==1,
                      "cross-projection identity drift must be rejected before entering history");
        }
        for(var i8=0;i8<10;i8++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                history.Append(replay,release);
                var tampered=replay with { ProgramId=Guid.NewGuid() };
                var rejected=false;
                try
                {
                    history.Append(tampered,release);
                }
                catch(ArgumentException)
                {
                    rejected=true;
                }
                Check(rejected && history.Capture().Entries.Count==1,
                      "cross-projection identity drift must be rejected before entering history");
        }
        for(var i9=0;i9<10;i9++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                history.Append(replay,release);
                var tampered=replay with { ProgramId=Guid.NewGuid() };
                var rejected=false;
                try
                {
                    history.Append(tampered,release);
                }
                catch(ArgumentException)
                {
                    rejected=true;
                }
                Check(rejected && history.Capture().Entries.Count==1,
                      "cross-projection identity drift must be rejected before entering history");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("client run history smoke must execute exactly 100 rounds");
    }
}
