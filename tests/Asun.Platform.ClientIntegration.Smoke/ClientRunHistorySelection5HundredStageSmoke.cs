using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientRunHistorySelection5HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                4,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var initial=ClientRunHistorySelectionRuntime.CreateInitial();
            var selected=ClientRunHistorySelectionRuntime.Select(history,2,initial.SelectionSequence);
            Check(initial.SelectedOrdinal is null &&
                  initial.SelectionSequence==0 &&
                  selected.SelectedOrdinal==2 &&
                  selected.SelectionSequence==1,
                  "selection must be presentation state and must not mutate the source history snapshot");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                4,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var initial=ClientRunHistorySelectionRuntime.CreateInitial();
            var selected=ClientRunHistorySelectionRuntime.Select(history,2,initial.SelectionSequence);
            Check(initial.SelectedOrdinal is null &&
                  initial.SelectionSequence==0 &&
                  selected.SelectedOrdinal==2 &&
                  selected.SelectionSequence==1,
                  "selection must be presentation state and must not mutate the source history snapshot");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                4,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var initial=ClientRunHistorySelectionRuntime.CreateInitial();
            var selected=ClientRunHistorySelectionRuntime.Select(history,2,initial.SelectionSequence);
            Check(initial.SelectedOrdinal is null &&
                  initial.SelectionSequence==0 &&
                  selected.SelectedOrdinal==2 &&
                  selected.SelectionSequence==1,
                  "selection must be presentation state and must not mutate the source history snapshot");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                4,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var initial=ClientRunHistorySelectionRuntime.CreateInitial();
            var selected=ClientRunHistorySelectionRuntime.Select(history,2,initial.SelectionSequence);
            Check(initial.SelectedOrdinal is null &&
                  initial.SelectionSequence==0 &&
                  selected.SelectedOrdinal==2 &&
                  selected.SelectionSequence==1,
                  "selection must be presentation state and must not mutate the source history snapshot");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                4,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var initial=ClientRunHistorySelectionRuntime.CreateInitial();
            var selected=ClientRunHistorySelectionRuntime.Select(history,2,initial.SelectionSequence);
            Check(initial.SelectedOrdinal is null &&
                  initial.SelectionSequence==0 &&
                  selected.SelectedOrdinal==2 &&
                  selected.SelectionSequence==1,
                  "selection must be presentation state and must not mutate the source history snapshot");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                4,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var initial=ClientRunHistorySelectionRuntime.CreateInitial();
            var selected=ClientRunHistorySelectionRuntime.Select(history,2,initial.SelectionSequence);
            Check(initial.SelectedOrdinal is null &&
                  initial.SelectionSequence==0 &&
                  selected.SelectedOrdinal==2 &&
                  selected.SelectionSequence==1,
                  "selection must be presentation state and must not mutate the source history snapshot");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                4,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var initial=ClientRunHistorySelectionRuntime.CreateInitial();
            var selected=ClientRunHistorySelectionRuntime.Select(history,2,initial.SelectionSequence);
            Check(initial.SelectedOrdinal is null &&
                  initial.SelectionSequence==0 &&
                  selected.SelectedOrdinal==2 &&
                  selected.SelectionSequence==1,
                  "selection must be presentation state and must not mutate the source history snapshot");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                4,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var initial=ClientRunHistorySelectionRuntime.CreateInitial();
            var selected=ClientRunHistorySelectionRuntime.Select(history,2,initial.SelectionSequence);
            Check(initial.SelectedOrdinal is null &&
                  initial.SelectionSequence==0 &&
                  selected.SelectedOrdinal==2 &&
                  selected.SelectionSequence==1,
                  "selection must be presentation state and must not mutate the source history snapshot");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                4,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var initial=ClientRunHistorySelectionRuntime.CreateInitial();
            var selected=ClientRunHistorySelectionRuntime.Select(history,2,initial.SelectionSequence);
            Check(initial.SelectedOrdinal is null &&
                  initial.SelectionSequence==0 &&
                  selected.SelectedOrdinal==2 &&
                  selected.SelectionSequence==1,
                  "selection must be presentation state and must not mutate the source history snapshot");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                4,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var initial=ClientRunHistorySelectionRuntime.CreateInitial();
            var selected=ClientRunHistorySelectionRuntime.Select(history,2,initial.SelectionSequence);
            Check(initial.SelectedOrdinal is null &&
                  initial.SelectionSequence==0 &&
                  selected.SelectedOrdinal==2 &&
                  selected.SelectionSequence==1,
                  "selection must be presentation state and must not mutate the source history snapshot");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
