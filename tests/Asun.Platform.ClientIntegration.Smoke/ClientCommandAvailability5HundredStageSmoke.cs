using System.Numerics;
using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientCommandAvailability5HundredStageSmoke
{
    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var i0=0;i0<10;i0++)
        {
            round++;
                var failedProduction=new ClientWorkspaceSnapshot(
                    Guid.Parse("79000000-0000-0000-0000-000000000001"),
                    new Version(1,0,0),
                    Guid.Parse("79000000-0000-0000-0000-000000000002"),
                    ClientExecutionStatus.Failed,
                    0,
                    null,
                    "simulated failure");
                var snapshot=new ClientInspectionWorkspaceSnapshot(
                    failedProduction,
                    null,
                    null,
                    null,
                    new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()));
                var availability=ClientCommandAvailabilityRuntime.Create(snapshot);
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset &&
                      !availability.CanSelectRoi &&
                      !availability.CanCreateRoi,
                      "Failed client command availability must expose recovery without ROI editing");
        }
        for(var i1=0;i1<10;i1++)
        {
            round++;
                var failedProduction=new ClientWorkspaceSnapshot(
                    Guid.Parse("79000000-0000-0000-0000-000000000001"),
                    new Version(1,0,0),
                    Guid.Parse("79000000-0000-0000-0000-000000000002"),
                    ClientExecutionStatus.Failed,
                    0,
                    null,
                    "simulated failure");
                var snapshot=new ClientInspectionWorkspaceSnapshot(
                    failedProduction,
                    null,
                    null,
                    null,
                    new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()));
                var availability=ClientCommandAvailabilityRuntime.Create(snapshot);
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset &&
                      !availability.CanSelectRoi &&
                      !availability.CanCreateRoi,
                      "Failed client command availability must expose recovery without ROI editing");
        }
        for(var i2=0;i2<10;i2++)
        {
            round++;
                var failedProduction=new ClientWorkspaceSnapshot(
                    Guid.Parse("79000000-0000-0000-0000-000000000001"),
                    new Version(1,0,0),
                    Guid.Parse("79000000-0000-0000-0000-000000000002"),
                    ClientExecutionStatus.Failed,
                    0,
                    null,
                    "simulated failure");
                var snapshot=new ClientInspectionWorkspaceSnapshot(
                    failedProduction,
                    null,
                    null,
                    null,
                    new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()));
                var availability=ClientCommandAvailabilityRuntime.Create(snapshot);
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset &&
                      !availability.CanSelectRoi &&
                      !availability.CanCreateRoi,
                      "Failed client command availability must expose recovery without ROI editing");
        }
        for(var i3=0;i3<10;i3++)
        {
            round++;
                var failedProduction=new ClientWorkspaceSnapshot(
                    Guid.Parse("79000000-0000-0000-0000-000000000001"),
                    new Version(1,0,0),
                    Guid.Parse("79000000-0000-0000-0000-000000000002"),
                    ClientExecutionStatus.Failed,
                    0,
                    null,
                    "simulated failure");
                var snapshot=new ClientInspectionWorkspaceSnapshot(
                    failedProduction,
                    null,
                    null,
                    null,
                    new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()));
                var availability=ClientCommandAvailabilityRuntime.Create(snapshot);
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset &&
                      !availability.CanSelectRoi &&
                      !availability.CanCreateRoi,
                      "Failed client command availability must expose recovery without ROI editing");
        }
        for(var i4=0;i4<10;i4++)
        {
            round++;
                var failedProduction=new ClientWorkspaceSnapshot(
                    Guid.Parse("79000000-0000-0000-0000-000000000001"),
                    new Version(1,0,0),
                    Guid.Parse("79000000-0000-0000-0000-000000000002"),
                    ClientExecutionStatus.Failed,
                    0,
                    null,
                    "simulated failure");
                var snapshot=new ClientInspectionWorkspaceSnapshot(
                    failedProduction,
                    null,
                    null,
                    null,
                    new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()));
                var availability=ClientCommandAvailabilityRuntime.Create(snapshot);
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset &&
                      !availability.CanSelectRoi &&
                      !availability.CanCreateRoi,
                      "Failed client command availability must expose recovery without ROI editing");
        }
        for(var i5=0;i5<10;i5++)
        {
            round++;
                var failedProduction=new ClientWorkspaceSnapshot(
                    Guid.Parse("79000000-0000-0000-0000-000000000001"),
                    new Version(1,0,0),
                    Guid.Parse("79000000-0000-0000-0000-000000000002"),
                    ClientExecutionStatus.Failed,
                    0,
                    null,
                    "simulated failure");
                var snapshot=new ClientInspectionWorkspaceSnapshot(
                    failedProduction,
                    null,
                    null,
                    null,
                    new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()));
                var availability=ClientCommandAvailabilityRuntime.Create(snapshot);
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset &&
                      !availability.CanSelectRoi &&
                      !availability.CanCreateRoi,
                      "Failed client command availability must expose recovery without ROI editing");
        }
        for(var i6=0;i6<10;i6++)
        {
            round++;
                var failedProduction=new ClientWorkspaceSnapshot(
                    Guid.Parse("79000000-0000-0000-0000-000000000001"),
                    new Version(1,0,0),
                    Guid.Parse("79000000-0000-0000-0000-000000000002"),
                    ClientExecutionStatus.Failed,
                    0,
                    null,
                    "simulated failure");
                var snapshot=new ClientInspectionWorkspaceSnapshot(
                    failedProduction,
                    null,
                    null,
                    null,
                    new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()));
                var availability=ClientCommandAvailabilityRuntime.Create(snapshot);
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset &&
                      !availability.CanSelectRoi &&
                      !availability.CanCreateRoi,
                      "Failed client command availability must expose recovery without ROI editing");
        }
        for(var i7=0;i7<10;i7++)
        {
            round++;
                var failedProduction=new ClientWorkspaceSnapshot(
                    Guid.Parse("79000000-0000-0000-0000-000000000001"),
                    new Version(1,0,0),
                    Guid.Parse("79000000-0000-0000-0000-000000000002"),
                    ClientExecutionStatus.Failed,
                    0,
                    null,
                    "simulated failure");
                var snapshot=new ClientInspectionWorkspaceSnapshot(
                    failedProduction,
                    null,
                    null,
                    null,
                    new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()));
                var availability=ClientCommandAvailabilityRuntime.Create(snapshot);
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset &&
                      !availability.CanSelectRoi &&
                      !availability.CanCreateRoi,
                      "Failed client command availability must expose recovery without ROI editing");
        }
        for(var i8=0;i8<10;i8++)
        {
            round++;
                var failedProduction=new ClientWorkspaceSnapshot(
                    Guid.Parse("79000000-0000-0000-0000-000000000001"),
                    new Version(1,0,0),
                    Guid.Parse("79000000-0000-0000-0000-000000000002"),
                    ClientExecutionStatus.Failed,
                    0,
                    null,
                    "simulated failure");
                var snapshot=new ClientInspectionWorkspaceSnapshot(
                    failedProduction,
                    null,
                    null,
                    null,
                    new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()));
                var availability=ClientCommandAvailabilityRuntime.Create(snapshot);
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset &&
                      !availability.CanSelectRoi &&
                      !availability.CanCreateRoi,
                      "Failed client command availability must expose recovery without ROI editing");
        }
        for(var i9=0;i9<10;i9++)
        {
            round++;
                var failedProduction=new ClientWorkspaceSnapshot(
                    Guid.Parse("79000000-0000-0000-0000-000000000001"),
                    new Version(1,0,0),
                    Guid.Parse("79000000-0000-0000-0000-000000000002"),
                    ClientExecutionStatus.Failed,
                    0,
                    null,
                    "simulated failure");
                var snapshot=new ClientInspectionWorkspaceSnapshot(
                    failedProduction,
                    null,
                    null,
                    null,
                    new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()));
                var availability=ClientCommandAvailabilityRuntime.Create(snapshot);
                Check(availability.CanLoad &&
                      availability.CanRun &&
                      !availability.CanCancel &&
                      availability.CanReset &&
                      !availability.CanSelectRoi &&
                      !availability.CanCreateRoi,
                      "Failed client command availability must expose recovery without ROI editing");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("command availability smoke must execute exactly 100 rounds");
    }
}
