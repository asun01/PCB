using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientAcquisitionCommandGate5HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('a',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var routing=ClientCommandAvailabilityRuntime.Create(snapshot);
            Check(routing.CanRun &&
                  routing.CanCreateRoi &&
                  routing.CanSelectRoi,
                  "Completed Production with a ready external source must expose execution/ROI commands");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('a',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var routing=ClientCommandAvailabilityRuntime.Create(snapshot);
            Check(routing.CanRun &&
                  routing.CanCreateRoi &&
                  routing.CanSelectRoi,
                  "Completed Production with a ready external source must expose execution/ROI commands");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('a',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var routing=ClientCommandAvailabilityRuntime.Create(snapshot);
            Check(routing.CanRun &&
                  routing.CanCreateRoi &&
                  routing.CanSelectRoi,
                  "Completed Production with a ready external source must expose execution/ROI commands");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('a',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var routing=ClientCommandAvailabilityRuntime.Create(snapshot);
            Check(routing.CanRun &&
                  routing.CanCreateRoi &&
                  routing.CanSelectRoi,
                  "Completed Production with a ready external source must expose execution/ROI commands");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('a',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var routing=ClientCommandAvailabilityRuntime.Create(snapshot);
            Check(routing.CanRun &&
                  routing.CanCreateRoi &&
                  routing.CanSelectRoi,
                  "Completed Production with a ready external source must expose execution/ROI commands");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('a',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var routing=ClientCommandAvailabilityRuntime.Create(snapshot);
            Check(routing.CanRun &&
                  routing.CanCreateRoi &&
                  routing.CanSelectRoi,
                  "Completed Production with a ready external source must expose execution/ROI commands");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('a',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var routing=ClientCommandAvailabilityRuntime.Create(snapshot);
            Check(routing.CanRun &&
                  routing.CanCreateRoi &&
                  routing.CanSelectRoi,
                  "Completed Production with a ready external source must expose execution/ROI commands");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('a',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var routing=ClientCommandAvailabilityRuntime.Create(snapshot);
            Check(routing.CanRun &&
                  routing.CanCreateRoi &&
                  routing.CanSelectRoi,
                  "Completed Production with a ready external source must expose execution/ROI commands");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('a',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var routing=ClientCommandAvailabilityRuntime.Create(snapshot);
            Check(routing.CanRun &&
                  routing.CanCreateRoi &&
                  routing.CanSelectRoi,
                  "Completed Production with a ready external source must expose execution/ROI commands");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    Guid.NewGuid(),new Version(1,0,0),Guid.NewGuid(),
                    ClientExecutionStatus.Completed,3,new string('a',64),null),
                null,null,null,
                new ClientProductionRunHistorySnapshot(5,1,0,Array.Empty<ClientProductionRunHistoryEntry>()))
            {
                Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                    ClientAcquisitionState.Ready,
                    new ClientAcquisitionDescriptor("camera-01","Camera 01",false),
                    null,
                    true)
            };
            var routing=ClientCommandAvailabilityRuntime.Create(snapshot);
            Check(routing.CanRun &&
                  routing.CanCreateRoi &&
                  routing.CanSelectRoi,
                  "Completed Production with a ready external source must expose execution/ROI commands");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
