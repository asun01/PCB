using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientResultsPresentation4HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                2,
                4,
                1,
                new[]
                {
                    new ClientProductionRunHistoryEntry(
                        2,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        3,
                        new string('c',64),
                        true,
                        "artifact-2"),
                    new ClientProductionRunHistoryEntry(
                        3,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        4,
                        new string('d',64),
                        false,
                        "artifact-3")
                });
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(null,null,null,ClientExecutionStatus.Idle,0,null,null),
                null,null,null,history);
            var items=ClientResultsPresentationRuntime.CreateHistory(snapshot,10);
            Check(items.Count==2 &&
                  items[0].Ordinal==3 &&
                  items[0].ReleaseText=="Release Not Ready" &&
                  items[1].Ordinal==2 &&
                  items[1].ReleaseReady,
                  "result history presentation must preserve descending ordinal and release state");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                2,
                4,
                1,
                new[]
                {
                    new ClientProductionRunHistoryEntry(
                        2,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        3,
                        new string('c',64),
                        true,
                        "artifact-2"),
                    new ClientProductionRunHistoryEntry(
                        3,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        4,
                        new string('d',64),
                        false,
                        "artifact-3")
                });
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(null,null,null,ClientExecutionStatus.Idle,0,null,null),
                null,null,null,history);
            var items=ClientResultsPresentationRuntime.CreateHistory(snapshot,10);
            Check(items.Count==2 &&
                  items[0].Ordinal==3 &&
                  items[0].ReleaseText=="Release Not Ready" &&
                  items[1].Ordinal==2 &&
                  items[1].ReleaseReady,
                  "result history presentation must preserve descending ordinal and release state");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                2,
                4,
                1,
                new[]
                {
                    new ClientProductionRunHistoryEntry(
                        2,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        3,
                        new string('c',64),
                        true,
                        "artifact-2"),
                    new ClientProductionRunHistoryEntry(
                        3,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        4,
                        new string('d',64),
                        false,
                        "artifact-3")
                });
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(null,null,null,ClientExecutionStatus.Idle,0,null,null),
                null,null,null,history);
            var items=ClientResultsPresentationRuntime.CreateHistory(snapshot,10);
            Check(items.Count==2 &&
                  items[0].Ordinal==3 &&
                  items[0].ReleaseText=="Release Not Ready" &&
                  items[1].Ordinal==2 &&
                  items[1].ReleaseReady,
                  "result history presentation must preserve descending ordinal and release state");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                2,
                4,
                1,
                new[]
                {
                    new ClientProductionRunHistoryEntry(
                        2,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        3,
                        new string('c',64),
                        true,
                        "artifact-2"),
                    new ClientProductionRunHistoryEntry(
                        3,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        4,
                        new string('d',64),
                        false,
                        "artifact-3")
                });
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(null,null,null,ClientExecutionStatus.Idle,0,null,null),
                null,null,null,history);
            var items=ClientResultsPresentationRuntime.CreateHistory(snapshot,10);
            Check(items.Count==2 &&
                  items[0].Ordinal==3 &&
                  items[0].ReleaseText=="Release Not Ready" &&
                  items[1].Ordinal==2 &&
                  items[1].ReleaseReady,
                  "result history presentation must preserve descending ordinal and release state");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                2,
                4,
                1,
                new[]
                {
                    new ClientProductionRunHistoryEntry(
                        2,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        3,
                        new string('c',64),
                        true,
                        "artifact-2"),
                    new ClientProductionRunHistoryEntry(
                        3,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        4,
                        new string('d',64),
                        false,
                        "artifact-3")
                });
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(null,null,null,ClientExecutionStatus.Idle,0,null,null),
                null,null,null,history);
            var items=ClientResultsPresentationRuntime.CreateHistory(snapshot,10);
            Check(items.Count==2 &&
                  items[0].Ordinal==3 &&
                  items[0].ReleaseText=="Release Not Ready" &&
                  items[1].Ordinal==2 &&
                  items[1].ReleaseReady,
                  "result history presentation must preserve descending ordinal and release state");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                2,
                4,
                1,
                new[]
                {
                    new ClientProductionRunHistoryEntry(
                        2,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        3,
                        new string('c',64),
                        true,
                        "artifact-2"),
                    new ClientProductionRunHistoryEntry(
                        3,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        4,
                        new string('d',64),
                        false,
                        "artifact-3")
                });
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(null,null,null,ClientExecutionStatus.Idle,0,null,null),
                null,null,null,history);
            var items=ClientResultsPresentationRuntime.CreateHistory(snapshot,10);
            Check(items.Count==2 &&
                  items[0].Ordinal==3 &&
                  items[0].ReleaseText=="Release Not Ready" &&
                  items[1].Ordinal==2 &&
                  items[1].ReleaseReady,
                  "result history presentation must preserve descending ordinal and release state");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                2,
                4,
                1,
                new[]
                {
                    new ClientProductionRunHistoryEntry(
                        2,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        3,
                        new string('c',64),
                        true,
                        "artifact-2"),
                    new ClientProductionRunHistoryEntry(
                        3,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        4,
                        new string('d',64),
                        false,
                        "artifact-3")
                });
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(null,null,null,ClientExecutionStatus.Idle,0,null,null),
                null,null,null,history);
            var items=ClientResultsPresentationRuntime.CreateHistory(snapshot,10);
            Check(items.Count==2 &&
                  items[0].Ordinal==3 &&
                  items[0].ReleaseText=="Release Not Ready" &&
                  items[1].Ordinal==2 &&
                  items[1].ReleaseReady,
                  "result history presentation must preserve descending ordinal and release state");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                2,
                4,
                1,
                new[]
                {
                    new ClientProductionRunHistoryEntry(
                        2,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        3,
                        new string('c',64),
                        true,
                        "artifact-2"),
                    new ClientProductionRunHistoryEntry(
                        3,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        4,
                        new string('d',64),
                        false,
                        "artifact-3")
                });
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(null,null,null,ClientExecutionStatus.Idle,0,null,null),
                null,null,null,history);
            var items=ClientResultsPresentationRuntime.CreateHistory(snapshot,10);
            Check(items.Count==2 &&
                  items[0].Ordinal==3 &&
                  items[0].ReleaseText=="Release Not Ready" &&
                  items[1].Ordinal==2 &&
                  items[1].ReleaseReady,
                  "result history presentation must preserve descending ordinal and release state");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                2,
                4,
                1,
                new[]
                {
                    new ClientProductionRunHistoryEntry(
                        2,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        3,
                        new string('c',64),
                        true,
                        "artifact-2"),
                    new ClientProductionRunHistoryEntry(
                        3,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        4,
                        new string('d',64),
                        false,
                        "artifact-3")
                });
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(null,null,null,ClientExecutionStatus.Idle,0,null,null),
                null,null,null,history);
            var items=ClientResultsPresentationRuntime.CreateHistory(snapshot,10);
            Check(items.Count==2 &&
                  items[0].Ordinal==3 &&
                  items[0].ReleaseText=="Release Not Ready" &&
                  items[1].Ordinal==2 &&
                  items[1].ReleaseReady,
                  "result history presentation must preserve descending ordinal and release state");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                2,
                4,
                1,
                new[]
                {
                    new ClientProductionRunHistoryEntry(
                        2,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        3,
                        new string('c',64),
                        true,
                        "artifact-2"),
                    new ClientProductionRunHistoryEntry(
                        3,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        4,
                        new string('d',64),
                        false,
                        "artifact-3")
                });
            var snapshot=new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(null,null,null,ClientExecutionStatus.Idle,0,null,null),
                null,null,null,history);
            var items=ClientResultsPresentationRuntime.CreateHistory(snapshot,10);
            Check(items.Count==2 &&
                  items[0].Ordinal==3 &&
                  items[0].ReleaseText=="Release Not Ready" &&
                  items[1].Ordinal==2 &&
                  items[1].ReleaseReady,
                  "result history presentation must preserve descending ordinal and release state");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
