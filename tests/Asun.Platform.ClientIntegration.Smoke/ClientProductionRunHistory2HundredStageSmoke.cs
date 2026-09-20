using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientProductionRunHistory2HundredStageSmoke
{
    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var i0=0;i0<10;i0++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                for(var run=0;run<3;run++)
                {
                    var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                    var definition=ClientWorkspaceSmokeFixture.CreateDefinition(
                        Guid.Parse($"74000000-0000-0000-0000-{run+1:000000000000}"));
                    workspace.Load(definition);
                    var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                    var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                    var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                    history.Append(replay,release);
                }
                var snapshot=history.Capture();
                Check(snapshot.Entries.Count==2 &&
                      snapshot.DroppedCount==1 &&
                      snapshot.NextOrdinal==4 &&
                      snapshot.Entries[0].Ordinal==2,
                      "history should evict oldest entries at the bounded capacity");
        }
        for(var i1=0;i1<10;i1++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                for(var run=0;run<3;run++)
                {
                    var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                    var definition=ClientWorkspaceSmokeFixture.CreateDefinition(
                        Guid.Parse($"74000000-0000-0000-0000-{run+1:000000000000}"));
                    workspace.Load(definition);
                    var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                    var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                    var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                    history.Append(replay,release);
                }
                var snapshot=history.Capture();
                Check(snapshot.Entries.Count==2 &&
                      snapshot.DroppedCount==1 &&
                      snapshot.NextOrdinal==4 &&
                      snapshot.Entries[0].Ordinal==2,
                      "history should evict oldest entries at the bounded capacity");
        }
        for(var i2=0;i2<10;i2++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                for(var run=0;run<3;run++)
                {
                    var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                    var definition=ClientWorkspaceSmokeFixture.CreateDefinition(
                        Guid.Parse($"74000000-0000-0000-0000-{run+1:000000000000}"));
                    workspace.Load(definition);
                    var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                    var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                    var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                    history.Append(replay,release);
                }
                var snapshot=history.Capture();
                Check(snapshot.Entries.Count==2 &&
                      snapshot.DroppedCount==1 &&
                      snapshot.NextOrdinal==4 &&
                      snapshot.Entries[0].Ordinal==2,
                      "history should evict oldest entries at the bounded capacity");
        }
        for(var i3=0;i3<10;i3++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                for(var run=0;run<3;run++)
                {
                    var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                    var definition=ClientWorkspaceSmokeFixture.CreateDefinition(
                        Guid.Parse($"74000000-0000-0000-0000-{run+1:000000000000}"));
                    workspace.Load(definition);
                    var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                    var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                    var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                    history.Append(replay,release);
                }
                var snapshot=history.Capture();
                Check(snapshot.Entries.Count==2 &&
                      snapshot.DroppedCount==1 &&
                      snapshot.NextOrdinal==4 &&
                      snapshot.Entries[0].Ordinal==2,
                      "history should evict oldest entries at the bounded capacity");
        }
        for(var i4=0;i4<10;i4++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                for(var run=0;run<3;run++)
                {
                    var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                    var definition=ClientWorkspaceSmokeFixture.CreateDefinition(
                        Guid.Parse($"74000000-0000-0000-0000-{run+1:000000000000}"));
                    workspace.Load(definition);
                    var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                    var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                    var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                    history.Append(replay,release);
                }
                var snapshot=history.Capture();
                Check(snapshot.Entries.Count==2 &&
                      snapshot.DroppedCount==1 &&
                      snapshot.NextOrdinal==4 &&
                      snapshot.Entries[0].Ordinal==2,
                      "history should evict oldest entries at the bounded capacity");
        }
        for(var i5=0;i5<10;i5++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                for(var run=0;run<3;run++)
                {
                    var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                    var definition=ClientWorkspaceSmokeFixture.CreateDefinition(
                        Guid.Parse($"74000000-0000-0000-0000-{run+1:000000000000}"));
                    workspace.Load(definition);
                    var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                    var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                    var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                    history.Append(replay,release);
                }
                var snapshot=history.Capture();
                Check(snapshot.Entries.Count==2 &&
                      snapshot.DroppedCount==1 &&
                      snapshot.NextOrdinal==4 &&
                      snapshot.Entries[0].Ordinal==2,
                      "history should evict oldest entries at the bounded capacity");
        }
        for(var i6=0;i6<10;i6++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                for(var run=0;run<3;run++)
                {
                    var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                    var definition=ClientWorkspaceSmokeFixture.CreateDefinition(
                        Guid.Parse($"74000000-0000-0000-0000-{run+1:000000000000}"));
                    workspace.Load(definition);
                    var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                    var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                    var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                    history.Append(replay,release);
                }
                var snapshot=history.Capture();
                Check(snapshot.Entries.Count==2 &&
                      snapshot.DroppedCount==1 &&
                      snapshot.NextOrdinal==4 &&
                      snapshot.Entries[0].Ordinal==2,
                      "history should evict oldest entries at the bounded capacity");
        }
        for(var i7=0;i7<10;i7++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                for(var run=0;run<3;run++)
                {
                    var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                    var definition=ClientWorkspaceSmokeFixture.CreateDefinition(
                        Guid.Parse($"74000000-0000-0000-0000-{run+1:000000000000}"));
                    workspace.Load(definition);
                    var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                    var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                    var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                    history.Append(replay,release);
                }
                var snapshot=history.Capture();
                Check(snapshot.Entries.Count==2 &&
                      snapshot.DroppedCount==1 &&
                      snapshot.NextOrdinal==4 &&
                      snapshot.Entries[0].Ordinal==2,
                      "history should evict oldest entries at the bounded capacity");
        }
        for(var i8=0;i8<10;i8++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                for(var run=0;run<3;run++)
                {
                    var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                    var definition=ClientWorkspaceSmokeFixture.CreateDefinition(
                        Guid.Parse($"74000000-0000-0000-0000-{run+1:000000000000}"));
                    workspace.Load(definition);
                    var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                    var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                    var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                    history.Append(replay,release);
                }
                var snapshot=history.Capture();
                Check(snapshot.Entries.Count==2 &&
                      snapshot.DroppedCount==1 &&
                      snapshot.NextOrdinal==4 &&
                      snapshot.Entries[0].Ordinal==2,
                      "history should evict oldest entries at the bounded capacity");
        }
        for(var i9=0;i9<10;i9++)
        {
            round++;
                var history=new ClientProductionRunHistory(2);
                for(var run=0;run<3;run++)
                {
                    var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                    var definition=ClientWorkspaceSmokeFixture.CreateDefinition(
                        Guid.Parse($"74000000-0000-0000-0000-{run+1:000000000000}"));
                    workspace.Load(definition);
                    var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                    var replay=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                    var release=ClientReleaseProjectionRuntime.Create(replay,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
                    history.Append(replay,release);
                }
                var snapshot=history.Capture();
                Check(snapshot.Entries.Count==2 &&
                      snapshot.DroppedCount==1 &&
                      snapshot.NextOrdinal==4 &&
                      snapshot.Entries[0].Ordinal==2,
                      "history should evict oldest entries at the bounded capacity");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("client run history smoke must execute exactly 100 rounds");
    }
}
