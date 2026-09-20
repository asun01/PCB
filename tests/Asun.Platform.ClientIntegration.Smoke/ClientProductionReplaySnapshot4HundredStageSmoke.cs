using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientProductionReplaySnapshot4HundredStageSmoke
{
    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var i0=0;i0<10;i0++)
        {
            round++;
                var firstWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                firstWorkspace.Load(firstDefinition);
                var firstReport=await firstWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var secondWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var secondDefinition=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("73000000-0000-0000-0000-000000000202"));
                secondWorkspace.Load(secondDefinition);
                var secondReport=await secondWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(firstWorkspace.Snapshot,firstReport);
                var right=ClientProductionReplaySnapshotRuntime.Create(secondWorkspace.Snapshot,secondReport);
                Check(!ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right),
                      "different client session identities must remain replay-distinguishable");
        }
        for(var i1=0;i1<10;i1++)
        {
            round++;
                var firstWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                firstWorkspace.Load(firstDefinition);
                var firstReport=await firstWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var secondWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var secondDefinition=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("73000000-0000-0000-0000-000000000202"));
                secondWorkspace.Load(secondDefinition);
                var secondReport=await secondWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(firstWorkspace.Snapshot,firstReport);
                var right=ClientProductionReplaySnapshotRuntime.Create(secondWorkspace.Snapshot,secondReport);
                Check(!ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right),
                      "different client session identities must remain replay-distinguishable");
        }
        for(var i2=0;i2<10;i2++)
        {
            round++;
                var firstWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                firstWorkspace.Load(firstDefinition);
                var firstReport=await firstWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var secondWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var secondDefinition=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("73000000-0000-0000-0000-000000000202"));
                secondWorkspace.Load(secondDefinition);
                var secondReport=await secondWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(firstWorkspace.Snapshot,firstReport);
                var right=ClientProductionReplaySnapshotRuntime.Create(secondWorkspace.Snapshot,secondReport);
                Check(!ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right),
                      "different client session identities must remain replay-distinguishable");
        }
        for(var i3=0;i3<10;i3++)
        {
            round++;
                var firstWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                firstWorkspace.Load(firstDefinition);
                var firstReport=await firstWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var secondWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var secondDefinition=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("73000000-0000-0000-0000-000000000202"));
                secondWorkspace.Load(secondDefinition);
                var secondReport=await secondWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(firstWorkspace.Snapshot,firstReport);
                var right=ClientProductionReplaySnapshotRuntime.Create(secondWorkspace.Snapshot,secondReport);
                Check(!ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right),
                      "different client session identities must remain replay-distinguishable");
        }
        for(var i4=0;i4<10;i4++)
        {
            round++;
                var firstWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                firstWorkspace.Load(firstDefinition);
                var firstReport=await firstWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var secondWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var secondDefinition=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("73000000-0000-0000-0000-000000000202"));
                secondWorkspace.Load(secondDefinition);
                var secondReport=await secondWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(firstWorkspace.Snapshot,firstReport);
                var right=ClientProductionReplaySnapshotRuntime.Create(secondWorkspace.Snapshot,secondReport);
                Check(!ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right),
                      "different client session identities must remain replay-distinguishable");
        }
        for(var i5=0;i5<10;i5++)
        {
            round++;
                var firstWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                firstWorkspace.Load(firstDefinition);
                var firstReport=await firstWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var secondWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var secondDefinition=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("73000000-0000-0000-0000-000000000202"));
                secondWorkspace.Load(secondDefinition);
                var secondReport=await secondWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(firstWorkspace.Snapshot,firstReport);
                var right=ClientProductionReplaySnapshotRuntime.Create(secondWorkspace.Snapshot,secondReport);
                Check(!ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right),
                      "different client session identities must remain replay-distinguishable");
        }
        for(var i6=0;i6<10;i6++)
        {
            round++;
                var firstWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                firstWorkspace.Load(firstDefinition);
                var firstReport=await firstWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var secondWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var secondDefinition=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("73000000-0000-0000-0000-000000000202"));
                secondWorkspace.Load(secondDefinition);
                var secondReport=await secondWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(firstWorkspace.Snapshot,firstReport);
                var right=ClientProductionReplaySnapshotRuntime.Create(secondWorkspace.Snapshot,secondReport);
                Check(!ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right),
                      "different client session identities must remain replay-distinguishable");
        }
        for(var i7=0;i7<10;i7++)
        {
            round++;
                var firstWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                firstWorkspace.Load(firstDefinition);
                var firstReport=await firstWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var secondWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var secondDefinition=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("73000000-0000-0000-0000-000000000202"));
                secondWorkspace.Load(secondDefinition);
                var secondReport=await secondWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(firstWorkspace.Snapshot,firstReport);
                var right=ClientProductionReplaySnapshotRuntime.Create(secondWorkspace.Snapshot,secondReport);
                Check(!ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right),
                      "different client session identities must remain replay-distinguishable");
        }
        for(var i8=0;i8<10;i8++)
        {
            round++;
                var firstWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                firstWorkspace.Load(firstDefinition);
                var firstReport=await firstWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var secondWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var secondDefinition=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("73000000-0000-0000-0000-000000000202"));
                secondWorkspace.Load(secondDefinition);
                var secondReport=await secondWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(firstWorkspace.Snapshot,firstReport);
                var right=ClientProductionReplaySnapshotRuntime.Create(secondWorkspace.Snapshot,secondReport);
                Check(!ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right),
                      "different client session identities must remain replay-distinguishable");
        }
        for(var i9=0;i9<10;i9++)
        {
            round++;
                var firstWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var firstDefinition=ClientWorkspaceSmokeFixture.CreateDefinition();
                firstWorkspace.Load(firstDefinition);
                var firstReport=await firstWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var secondWorkspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                var secondDefinition=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("73000000-0000-0000-0000-000000000202"));
                secondWorkspace.Load(secondDefinition);
                var secondReport=await secondWorkspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
                var left=ClientProductionReplaySnapshotRuntime.Create(firstWorkspace.Snapshot,firstReport);
                var right=ClientProductionReplaySnapshotRuntime.Create(secondWorkspace.Snapshot,secondReport);
                Check(!ClientProductionReplaySnapshotRuntime.IsEquivalent(left,right),
                      "different client session identities must remain replay-distinguishable");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("client replay smoke must execute exactly 100 rounds");
    }
}
