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

            var workspace1=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition1=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000001"));
            workspace1.Load(definition1);
            var report1=await workspace1.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay1=ClientProductionReplaySnapshotRuntime.Create(workspace1.Snapshot,report1);
            var release1=ClientReleaseProjectionRuntime.Create(replay1,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay1,release1);

            var workspace2=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition2=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000002"));
            workspace2.Load(definition2);
            var report2=await workspace2.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay2=ClientProductionReplaySnapshotRuntime.Create(workspace2.Snapshot,report2);
            var release2=ClientReleaseProjectionRuntime.Create(replay2,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay2,release2);

            var workspace3=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition3=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000003"));
            workspace3.Load(definition3);
            var report3=await workspace3.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay3=ClientProductionReplaySnapshotRuntime.Create(workspace3.Snapshot,report3);
            var release3=ClientReleaseProjectionRuntime.Create(replay3,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay3,release3);

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

            var workspace1=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition1=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000001"));
            workspace1.Load(definition1);
            var report1=await workspace1.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay1=ClientProductionReplaySnapshotRuntime.Create(workspace1.Snapshot,report1);
            var release1=ClientReleaseProjectionRuntime.Create(replay1,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay1,release1);

            var workspace2=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition2=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000002"));
            workspace2.Load(definition2);
            var report2=await workspace2.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay2=ClientProductionReplaySnapshotRuntime.Create(workspace2.Snapshot,report2);
            var release2=ClientReleaseProjectionRuntime.Create(replay2,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay2,release2);

            var workspace3=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition3=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000003"));
            workspace3.Load(definition3);
            var report3=await workspace3.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay3=ClientProductionReplaySnapshotRuntime.Create(workspace3.Snapshot,report3);
            var release3=ClientReleaseProjectionRuntime.Create(replay3,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay3,release3);

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

            var workspace1=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition1=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000001"));
            workspace1.Load(definition1);
            var report1=await workspace1.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay1=ClientProductionReplaySnapshotRuntime.Create(workspace1.Snapshot,report1);
            var release1=ClientReleaseProjectionRuntime.Create(replay1,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay1,release1);

            var workspace2=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition2=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000002"));
            workspace2.Load(definition2);
            var report2=await workspace2.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay2=ClientProductionReplaySnapshotRuntime.Create(workspace2.Snapshot,report2);
            var release2=ClientReleaseProjectionRuntime.Create(replay2,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay2,release2);

            var workspace3=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition3=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000003"));
            workspace3.Load(definition3);
            var report3=await workspace3.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay3=ClientProductionReplaySnapshotRuntime.Create(workspace3.Snapshot,report3);
            var release3=ClientReleaseProjectionRuntime.Create(replay3,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay3,release3);

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

            var workspace1=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition1=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000001"));
            workspace1.Load(definition1);
            var report1=await workspace1.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay1=ClientProductionReplaySnapshotRuntime.Create(workspace1.Snapshot,report1);
            var release1=ClientReleaseProjectionRuntime.Create(replay1,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay1,release1);

            var workspace2=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition2=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000002"));
            workspace2.Load(definition2);
            var report2=await workspace2.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay2=ClientProductionReplaySnapshotRuntime.Create(workspace2.Snapshot,report2);
            var release2=ClientReleaseProjectionRuntime.Create(replay2,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay2,release2);

            var workspace3=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition3=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000003"));
            workspace3.Load(definition3);
            var report3=await workspace3.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay3=ClientProductionReplaySnapshotRuntime.Create(workspace3.Snapshot,report3);
            var release3=ClientReleaseProjectionRuntime.Create(replay3,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay3,release3);

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

            var workspace1=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition1=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000001"));
            workspace1.Load(definition1);
            var report1=await workspace1.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay1=ClientProductionReplaySnapshotRuntime.Create(workspace1.Snapshot,report1);
            var release1=ClientReleaseProjectionRuntime.Create(replay1,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay1,release1);

            var workspace2=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition2=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000002"));
            workspace2.Load(definition2);
            var report2=await workspace2.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay2=ClientProductionReplaySnapshotRuntime.Create(workspace2.Snapshot,report2);
            var release2=ClientReleaseProjectionRuntime.Create(replay2,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay2,release2);

            var workspace3=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition3=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000003"));
            workspace3.Load(definition3);
            var report3=await workspace3.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay3=ClientProductionReplaySnapshotRuntime.Create(workspace3.Snapshot,report3);
            var release3=ClientReleaseProjectionRuntime.Create(replay3,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay3,release3);

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

            var workspace1=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition1=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000001"));
            workspace1.Load(definition1);
            var report1=await workspace1.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay1=ClientProductionReplaySnapshotRuntime.Create(workspace1.Snapshot,report1);
            var release1=ClientReleaseProjectionRuntime.Create(replay1,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay1,release1);

            var workspace2=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition2=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000002"));
            workspace2.Load(definition2);
            var report2=await workspace2.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay2=ClientProductionReplaySnapshotRuntime.Create(workspace2.Snapshot,report2);
            var release2=ClientReleaseProjectionRuntime.Create(replay2,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay2,release2);

            var workspace3=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition3=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000003"));
            workspace3.Load(definition3);
            var report3=await workspace3.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay3=ClientProductionReplaySnapshotRuntime.Create(workspace3.Snapshot,report3);
            var release3=ClientReleaseProjectionRuntime.Create(replay3,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay3,release3);

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

            var workspace1=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition1=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000001"));
            workspace1.Load(definition1);
            var report1=await workspace1.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay1=ClientProductionReplaySnapshotRuntime.Create(workspace1.Snapshot,report1);
            var release1=ClientReleaseProjectionRuntime.Create(replay1,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay1,release1);

            var workspace2=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition2=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000002"));
            workspace2.Load(definition2);
            var report2=await workspace2.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay2=ClientProductionReplaySnapshotRuntime.Create(workspace2.Snapshot,report2);
            var release2=ClientReleaseProjectionRuntime.Create(replay2,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay2,release2);

            var workspace3=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition3=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000003"));
            workspace3.Load(definition3);
            var report3=await workspace3.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay3=ClientProductionReplaySnapshotRuntime.Create(workspace3.Snapshot,report3);
            var release3=ClientReleaseProjectionRuntime.Create(replay3,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay3,release3);

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

            var workspace1=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition1=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000001"));
            workspace1.Load(definition1);
            var report1=await workspace1.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay1=ClientProductionReplaySnapshotRuntime.Create(workspace1.Snapshot,report1);
            var release1=ClientReleaseProjectionRuntime.Create(replay1,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay1,release1);

            var workspace2=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition2=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000002"));
            workspace2.Load(definition2);
            var report2=await workspace2.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay2=ClientProductionReplaySnapshotRuntime.Create(workspace2.Snapshot,report2);
            var release2=ClientReleaseProjectionRuntime.Create(replay2,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay2,release2);

            var workspace3=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition3=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000003"));
            workspace3.Load(definition3);
            var report3=await workspace3.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay3=ClientProductionReplaySnapshotRuntime.Create(workspace3.Snapshot,report3);
            var release3=ClientReleaseProjectionRuntime.Create(replay3,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay3,release3);

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

            var workspace1=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition1=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000001"));
            workspace1.Load(definition1);
            var report1=await workspace1.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay1=ClientProductionReplaySnapshotRuntime.Create(workspace1.Snapshot,report1);
            var release1=ClientReleaseProjectionRuntime.Create(replay1,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay1,release1);

            var workspace2=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition2=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000002"));
            workspace2.Load(definition2);
            var report2=await workspace2.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay2=ClientProductionReplaySnapshotRuntime.Create(workspace2.Snapshot,report2);
            var release2=ClientReleaseProjectionRuntime.Create(replay2,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay2,release2);

            var workspace3=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition3=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000003"));
            workspace3.Load(definition3);
            var report3=await workspace3.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay3=ClientProductionReplaySnapshotRuntime.Create(workspace3.Snapshot,report3);
            var release3=ClientReleaseProjectionRuntime.Create(replay3,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay3,release3);

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

            var workspace1=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition1=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000001"));
            workspace1.Load(definition1);
            var report1=await workspace1.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay1=ClientProductionReplaySnapshotRuntime.Create(workspace1.Snapshot,report1);
            var release1=ClientReleaseProjectionRuntime.Create(replay1,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay1,release1);

            var workspace2=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition2=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000002"));
            workspace2.Load(definition2);
            var report2=await workspace2.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay2=ClientProductionReplaySnapshotRuntime.Create(workspace2.Snapshot,report2);
            var release2=ClientReleaseProjectionRuntime.Create(replay2,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay2,release2);

            var workspace3=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var definition3=ClientWorkspaceSmokeFixture.CreateDefinition(Guid.Parse("74000000-0000-0000-0000-000000000003"));
            workspace3.Load(definition3);
            var report3=await workspace3.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            var replay3=ClientProductionReplaySnapshotRuntime.Create(workspace3.Snapshot,report3);
            var release3=ClientReleaseProjectionRuntime.Create(replay3,ClientWorkspaceSmokeFixture.CreateReleaseManifest());
            history.Append(replay3,release3);

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
