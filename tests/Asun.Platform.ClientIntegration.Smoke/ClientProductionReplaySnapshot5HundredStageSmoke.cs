using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientProductionReplaySnapshot5HundredStageSmoke
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
                var left=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var right=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var tampered=left with { ReplayFingerprint=new string('b',64) };
                Check(ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right) &&
                      !ClientProductionReplaySnapshotRuntime.IsEquivalent(left,tampered),
                      "identical client executions must converge while replay tampering remains visible");
        }
        for(var i1=0;i1<10;i1++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var right=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var tampered=left with { ReplayFingerprint=new string('b',64) };
                Check(ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right) &&
                      !ClientProductionReplaySnapshotRuntime.IsEquivalent(left,tampered),
                      "identical client executions must converge while replay tampering remains visible");
        }
        for(var i2=0;i2<10;i2++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var right=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var tampered=left with { ReplayFingerprint=new string('b',64) };
                Check(ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right) &&
                      !ClientProductionReplaySnapshotRuntime.IsEquivalent(left,tampered),
                      "identical client executions must converge while replay tampering remains visible");
        }
        for(var i3=0;i3<10;i3++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var right=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var tampered=left with { ReplayFingerprint=new string('b',64) };
                Check(ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right) &&
                      !ClientProductionReplaySnapshotRuntime.IsEquivalent(left,tampered),
                      "identical client executions must converge while replay tampering remains visible");
        }
        for(var i4=0;i4<10;i4++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var right=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var tampered=left with { ReplayFingerprint=new string('b',64) };
                Check(ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right) &&
                      !ClientProductionReplaySnapshotRuntime.IsEquivalent(left,tampered),
                      "identical client executions must converge while replay tampering remains visible");
        }
        for(var i5=0;i5<10;i5++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var right=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var tampered=left with { ReplayFingerprint=new string('b',64) };
                Check(ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right) &&
                      !ClientProductionReplaySnapshotRuntime.IsEquivalent(left,tampered),
                      "identical client executions must converge while replay tampering remains visible");
        }
        for(var i6=0;i6<10;i6++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var right=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var tampered=left with { ReplayFingerprint=new string('b',64) };
                Check(ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right) &&
                      !ClientProductionReplaySnapshotRuntime.IsEquivalent(left,tampered),
                      "identical client executions must converge while replay tampering remains visible");
        }
        for(var i7=0;i7<10;i7++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var right=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var tampered=left with { ReplayFingerprint=new string('b',64) };
                Check(ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right) &&
                      !ClientProductionReplaySnapshotRuntime.IsEquivalent(left,tampered),
                      "identical client executions must converge while replay tampering remains visible");
        }
        for(var i8=0;i8<10;i8++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var right=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var tampered=left with { ReplayFingerprint=new string('b',64) };
                Check(ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right) &&
                      !ClientProductionReplaySnapshotRuntime.IsEquivalent(left,tampered),
                      "identical client executions must converge while replay tampering remains visible");
        }
        for(var i9=0;i9<10;i9++)
        {
            round++;
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                workspace.Load(definition);
                var report=await workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var right=ClientProductionReplaySnapshotRuntime.Create(workspace.Snapshot,report);
                var tampered=left with { ReplayFingerprint=new string('b',64) };
                Check(ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right) &&
                      !ClientProductionReplaySnapshotRuntime.IsEquivalent(left,tampered),
                      "identical client executions must converge while replay tampering remains visible");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("client replay smoke must execute exactly 100 rounds");
    }
}
