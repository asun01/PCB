using System.Numerics;
using Asun.Platform.ClientIntegration;
using Asun.Platform.Pipeline;
using Asun.Platform.Evidence;
using Asun.Production.Runtime;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientInspectionWorkspaceControl2HundredStageSmoke
{
    private sealed class CountingRunner : IProductionSessionRunner
    {
        private readonly IProductionSessionRunner _inner=new ProductionSessionRuntimeAdapter();
        public int CallCount { get; private set; }

        public async ValueTask<ProductionSessionReport> RunAsync(
            ProductionSessionDefinition definition,
            Asun.Device.Contracts.IFrameSource source,
            CancellationToken cancellationToken=default)
        {
            CallCount++;
            return await _inner.RunAsync(definition,source,cancellationToken);
        }
    }

    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var i0=0;i0<10;i0++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                client.Load(definition);
                using var cancellation=new CancellationTokenSource();
                cancellation.Cancel();
                try
                {
                    await client.ExecuteAsync(
                        new Asun.Device.Impl.SimulatedFrameSource(8,8),
                        ClientWorkspaceSmokeFixture.CreateReleaseManifest(),
                        cancellation.Token);
                }
                catch(OperationCanceledException)
                {
                }
                var snapshot=client.Capture();
                Check(snapshot.Production.Status==ClientExecutionStatus.Cancelled &&
                      snapshot.Replay is null &&
                      snapshot.Release is null &&
                      snapshot.History.Entries.Count==0,
                      "cancelled client execution must not publish Replay/Release/History results");
        }
        for(var i1=0;i1<10;i1++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                client.Load(definition);
                using var cancellation=new CancellationTokenSource();
                cancellation.Cancel();
                try
                {
                    await client.ExecuteAsync(
                        new Asun.Device.Impl.SimulatedFrameSource(8,8),
                        ClientWorkspaceSmokeFixture.CreateReleaseManifest(),
                        cancellation.Token);
                }
                catch(OperationCanceledException)
                {
                }
                var snapshot=client.Capture();
                Check(snapshot.Production.Status==ClientExecutionStatus.Cancelled &&
                      snapshot.Replay is null &&
                      snapshot.Release is null &&
                      snapshot.History.Entries.Count==0,
                      "cancelled client execution must not publish Replay/Release/History results");
        }
        for(var i2=0;i2<10;i2++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                client.Load(definition);
                using var cancellation=new CancellationTokenSource();
                cancellation.Cancel();
                try
                {
                    await client.ExecuteAsync(
                        new Asun.Device.Impl.SimulatedFrameSource(8,8),
                        ClientWorkspaceSmokeFixture.CreateReleaseManifest(),
                        cancellation.Token);
                }
                catch(OperationCanceledException)
                {
                }
                var snapshot=client.Capture();
                Check(snapshot.Production.Status==ClientExecutionStatus.Cancelled &&
                      snapshot.Replay is null &&
                      snapshot.Release is null &&
                      snapshot.History.Entries.Count==0,
                      "cancelled client execution must not publish Replay/Release/History results");
        }
        for(var i3=0;i3<10;i3++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                client.Load(definition);
                using var cancellation=new CancellationTokenSource();
                cancellation.Cancel();
                try
                {
                    await client.ExecuteAsync(
                        new Asun.Device.Impl.SimulatedFrameSource(8,8),
                        ClientWorkspaceSmokeFixture.CreateReleaseManifest(),
                        cancellation.Token);
                }
                catch(OperationCanceledException)
                {
                }
                var snapshot=client.Capture();
                Check(snapshot.Production.Status==ClientExecutionStatus.Cancelled &&
                      snapshot.Replay is null &&
                      snapshot.Release is null &&
                      snapshot.History.Entries.Count==0,
                      "cancelled client execution must not publish Replay/Release/History results");
        }
        for(var i4=0;i4<10;i4++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                client.Load(definition);
                using var cancellation=new CancellationTokenSource();
                cancellation.Cancel();
                try
                {
                    await client.ExecuteAsync(
                        new Asun.Device.Impl.SimulatedFrameSource(8,8),
                        ClientWorkspaceSmokeFixture.CreateReleaseManifest(),
                        cancellation.Token);
                }
                catch(OperationCanceledException)
                {
                }
                var snapshot=client.Capture();
                Check(snapshot.Production.Status==ClientExecutionStatus.Cancelled &&
                      snapshot.Replay is null &&
                      snapshot.Release is null &&
                      snapshot.History.Entries.Count==0,
                      "cancelled client execution must not publish Replay/Release/History results");
        }
        for(var i5=0;i5<10;i5++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                client.Load(definition);
                using var cancellation=new CancellationTokenSource();
                cancellation.Cancel();
                try
                {
                    await client.ExecuteAsync(
                        new Asun.Device.Impl.SimulatedFrameSource(8,8),
                        ClientWorkspaceSmokeFixture.CreateReleaseManifest(),
                        cancellation.Token);
                }
                catch(OperationCanceledException)
                {
                }
                var snapshot=client.Capture();
                Check(snapshot.Production.Status==ClientExecutionStatus.Cancelled &&
                      snapshot.Replay is null &&
                      snapshot.Release is null &&
                      snapshot.History.Entries.Count==0,
                      "cancelled client execution must not publish Replay/Release/History results");
        }
        for(var i6=0;i6<10;i6++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                client.Load(definition);
                using var cancellation=new CancellationTokenSource();
                cancellation.Cancel();
                try
                {
                    await client.ExecuteAsync(
                        new Asun.Device.Impl.SimulatedFrameSource(8,8),
                        ClientWorkspaceSmokeFixture.CreateReleaseManifest(),
                        cancellation.Token);
                }
                catch(OperationCanceledException)
                {
                }
                var snapshot=client.Capture();
                Check(snapshot.Production.Status==ClientExecutionStatus.Cancelled &&
                      snapshot.Replay is null &&
                      snapshot.Release is null &&
                      snapshot.History.Entries.Count==0,
                      "cancelled client execution must not publish Replay/Release/History results");
        }
        for(var i7=0;i7<10;i7++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                client.Load(definition);
                using var cancellation=new CancellationTokenSource();
                cancellation.Cancel();
                try
                {
                    await client.ExecuteAsync(
                        new Asun.Device.Impl.SimulatedFrameSource(8,8),
                        ClientWorkspaceSmokeFixture.CreateReleaseManifest(),
                        cancellation.Token);
                }
                catch(OperationCanceledException)
                {
                }
                var snapshot=client.Capture();
                Check(snapshot.Production.Status==ClientExecutionStatus.Cancelled &&
                      snapshot.Replay is null &&
                      snapshot.Release is null &&
                      snapshot.History.Entries.Count==0,
                      "cancelled client execution must not publish Replay/Release/History results");
        }
        for(var i8=0;i8<10;i8++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                client.Load(definition);
                using var cancellation=new CancellationTokenSource();
                cancellation.Cancel();
                try
                {
                    await client.ExecuteAsync(
                        new Asun.Device.Impl.SimulatedFrameSource(8,8),
                        ClientWorkspaceSmokeFixture.CreateReleaseManifest(),
                        cancellation.Token);
                }
                catch(OperationCanceledException)
                {
                }
                var snapshot=client.Capture();
                Check(snapshot.Production.Status==ClientExecutionStatus.Cancelled &&
                      snapshot.Replay is null &&
                      snapshot.Release is null &&
                      snapshot.History.Entries.Count==0,
                      "cancelled client execution must not publish Replay/Release/History results");
        }
        for(var i9=0;i9<10;i9++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                client.Load(definition);
                using var cancellation=new CancellationTokenSource();
                cancellation.Cancel();
                try
                {
                    await client.ExecuteAsync(
                        new Asun.Device.Impl.SimulatedFrameSource(8,8),
                        ClientWorkspaceSmokeFixture.CreateReleaseManifest(),
                        cancellation.Token);
                }
                catch(OperationCanceledException)
                {
                }
                var snapshot=client.Capture();
                Check(snapshot.Production.Status==ClientExecutionStatus.Cancelled &&
                      snapshot.Replay is null &&
                      snapshot.Release is null &&
                      snapshot.History.Entries.Count==0,
                      "cancelled client execution must not publish Replay/Release/History results");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("client inspection control smoke must execute exactly 100 rounds");
    }
}
