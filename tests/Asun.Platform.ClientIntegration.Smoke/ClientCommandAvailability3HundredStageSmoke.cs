using System.Numerics;
using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientCommandAvailability3HundredStageSmoke
{
    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var i0=0;i0<10;i0++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
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
                var availability=ClientCommandAvailabilityRuntime.Create(client.Capture());
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset,
                      "Cancelled client command availability must expose recovery actions");
        }
        for(var i1=0;i1<10;i1++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
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
                var availability=ClientCommandAvailabilityRuntime.Create(client.Capture());
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset,
                      "Cancelled client command availability must expose recovery actions");
        }
        for(var i2=0;i2<10;i2++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
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
                var availability=ClientCommandAvailabilityRuntime.Create(client.Capture());
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset,
                      "Cancelled client command availability must expose recovery actions");
        }
        for(var i3=0;i3<10;i3++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
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
                var availability=ClientCommandAvailabilityRuntime.Create(client.Capture());
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset,
                      "Cancelled client command availability must expose recovery actions");
        }
        for(var i4=0;i4<10;i4++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
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
                var availability=ClientCommandAvailabilityRuntime.Create(client.Capture());
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset,
                      "Cancelled client command availability must expose recovery actions");
        }
        for(var i5=0;i5<10;i5++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
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
                var availability=ClientCommandAvailabilityRuntime.Create(client.Capture());
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset,
                      "Cancelled client command availability must expose recovery actions");
        }
        for(var i6=0;i6<10;i6++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
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
                var availability=ClientCommandAvailabilityRuntime.Create(client.Capture());
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset,
                      "Cancelled client command availability must expose recovery actions");
        }
        for(var i7=0;i7<10;i7++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
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
                var availability=ClientCommandAvailabilityRuntime.Create(client.Capture());
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset,
                      "Cancelled client command availability must expose recovery actions");
        }
        for(var i8=0;i8<10;i8++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
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
                var availability=ClientCommandAvailabilityRuntime.Create(client.Capture());
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset,
                      "Cancelled client command availability must expose recovery actions");
        }
        for(var i9=0;i9<10;i9++)
        {
            round++;
                using var client=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(400,400));
                client.Load(ClientWorkspaceSmokeFixture.CreateDefinition());
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
                var availability=ClientCommandAvailabilityRuntime.Create(client.Capture());
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset,
                      "Cancelled client command availability must expose recovery actions");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("command availability smoke must execute exactly 100 rounds");
    }
}
