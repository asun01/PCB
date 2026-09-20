using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientRunHistorySelection3HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),Guid.NewGuid(),2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,1,selection.SelectionSequence);
            var next=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectionSequence==1 &&
                  next.SelectionSequence==2 &&
                  next.SelectedOrdinal==2,
                  "run-history selection sequence must be monotonic across user selections");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),Guid.NewGuid(),2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,1,selection.SelectionSequence);
            var next=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectionSequence==1 &&
                  next.SelectionSequence==2 &&
                  next.SelectedOrdinal==2,
                  "run-history selection sequence must be monotonic across user selections");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),Guid.NewGuid(),2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,1,selection.SelectionSequence);
            var next=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectionSequence==1 &&
                  next.SelectionSequence==2 &&
                  next.SelectedOrdinal==2,
                  "run-history selection sequence must be monotonic across user selections");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),Guid.NewGuid(),2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,1,selection.SelectionSequence);
            var next=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectionSequence==1 &&
                  next.SelectionSequence==2 &&
                  next.SelectedOrdinal==2,
                  "run-history selection sequence must be monotonic across user selections");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),Guid.NewGuid(),2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,1,selection.SelectionSequence);
            var next=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectionSequence==1 &&
                  next.SelectionSequence==2 &&
                  next.SelectedOrdinal==2,
                  "run-history selection sequence must be monotonic across user selections");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),Guid.NewGuid(),2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,1,selection.SelectionSequence);
            var next=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectionSequence==1 &&
                  next.SelectionSequence==2 &&
                  next.SelectedOrdinal==2,
                  "run-history selection sequence must be monotonic across user selections");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),Guid.NewGuid(),2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,1,selection.SelectionSequence);
            var next=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectionSequence==1 &&
                  next.SelectionSequence==2 &&
                  next.SelectedOrdinal==2,
                  "run-history selection sequence must be monotonic across user selections");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),Guid.NewGuid(),2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,1,selection.SelectionSequence);
            var next=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectionSequence==1 &&
                  next.SelectionSequence==2 &&
                  next.SelectedOrdinal==2,
                  "run-history selection sequence must be monotonic across user selections");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),Guid.NewGuid(),2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,1,selection.SelectionSequence);
            var next=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectionSequence==1 &&
                  next.SelectionSequence==2 &&
                  next.SelectedOrdinal==2,
                  "run-history selection sequence must be monotonic across user selections");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),Guid.NewGuid(),2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),Guid.NewGuid(),3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,1,selection.SelectionSequence);
            var next=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectionSequence==1 &&
                  next.SelectionSequence==2 &&
                  next.SelectedOrdinal==2,
                  "run-history selection sequence must be monotonic across user selections");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
