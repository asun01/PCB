using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientRunHistorySelection1HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var first=Guid.NewGuid();
            var second=Guid.NewGuid();
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),first,2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),second,3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectedOrdinal==2 &&
                  selection.SelectionSequence==1 &&
                  selection.StatusText.Contains(second.ToString()),
                  "available history entry must become the selected run");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var first=Guid.NewGuid();
            var second=Guid.NewGuid();
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),first,2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),second,3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectedOrdinal==2 &&
                  selection.SelectionSequence==1 &&
                  selection.StatusText.Contains(second.ToString()),
                  "available history entry must become the selected run");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var first=Guid.NewGuid();
            var second=Guid.NewGuid();
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),first,2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),second,3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectedOrdinal==2 &&
                  selection.SelectionSequence==1 &&
                  selection.StatusText.Contains(second.ToString()),
                  "available history entry must become the selected run");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var first=Guid.NewGuid();
            var second=Guid.NewGuid();
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),first,2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),second,3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectedOrdinal==2 &&
                  selection.SelectionSequence==1 &&
                  selection.StatusText.Contains(second.ToString()),
                  "available history entry must become the selected run");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var first=Guid.NewGuid();
            var second=Guid.NewGuid();
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),first,2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),second,3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectedOrdinal==2 &&
                  selection.SelectionSequence==1 &&
                  selection.StatusText.Contains(second.ToString()),
                  "available history entry must become the selected run");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var first=Guid.NewGuid();
            var second=Guid.NewGuid();
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),first,2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),second,3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectedOrdinal==2 &&
                  selection.SelectionSequence==1 &&
                  selection.StatusText.Contains(second.ToString()),
                  "available history entry must become the selected run");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var first=Guid.NewGuid();
            var second=Guid.NewGuid();
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),first,2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),second,3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectedOrdinal==2 &&
                  selection.SelectionSequence==1 &&
                  selection.StatusText.Contains(second.ToString()),
                  "available history entry must become the selected run");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var first=Guid.NewGuid();
            var second=Guid.NewGuid();
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),first,2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),second,3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectedOrdinal==2 &&
                  selection.SelectionSequence==1 &&
                  selection.StatusText.Contains(second.ToString()),
                  "available history entry must become the selected run");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var first=Guid.NewGuid();
            var second=Guid.NewGuid();
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),first,2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),second,3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectedOrdinal==2 &&
                  selection.SelectionSequence==1 &&
                  selection.StatusText.Contains(second.ToString()),
                  "available history entry must become the selected run");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var first=Guid.NewGuid();
            var second=Guid.NewGuid();
            var history=new ClientProductionRunHistorySnapshot(
                5,
                3,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(1,Guid.NewGuid(),first,2,new string('a',64),true,"a"),
                    new ClientProductionRunHistoryEntry(2,Guid.NewGuid(),second,3,new string('b',64),false,"b")
                });
            var selection=ClientRunHistorySelectionRuntime.CreateInitial();
            selection=ClientRunHistorySelectionRuntime.Select(history,2,selection.SelectionSequence);
            Check(selection.SelectedOrdinal==2 &&
                  selection.SelectionSequence==1 &&
                  selection.StatusText.Contains(second.ToString()),
                  "available history entry must become the selected run");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
