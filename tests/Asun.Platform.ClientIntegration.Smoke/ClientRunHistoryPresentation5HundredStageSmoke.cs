using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientRunHistoryPresentation5HundredStageSmoke
{
    private static ClientProductionRunHistorySnapshot CreateHistorySnapshot() =>
        new(
            5,
            4,
            0,
            new[]
            {
                new ClientProductionRunHistoryEntry(
                    1,
                    Guid.Parse("78000000-0000-0000-0000-000000000001"),
                    Guid.Parse("78000000-0000-0000-0000-000000000101"),
                    3,
                    new string('a',64),
                    true,
                    "artifact-1.pcb"),
                new ClientProductionRunHistoryEntry(
                    2,
                    Guid.Parse("78000000-0000-0000-0000-000000000002"),
                    Guid.Parse("78000000-0000-0000-0000-000000000102"),
                    2,
                    new string('b',64),
                    false,
                    "artifact-2.pcb"),
                new ClientProductionRunHistoryEntry(
                    3,
                    Guid.Parse("78000000-0000-0000-0000-000000000003"),
                    Guid.Parse("78000000-0000-0000-0000-000000000103"),
                    4,
                    new string('c',64),
                    true,
                    "artifact-3.pcb")
            });


    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var i0=0;i0<10;i0++)
        {
            round++;
                var snapshot=CreateHistorySnapshot();
                var items=ClientRunHistoryPresentationRuntime.CreateItems(snapshot,5);
                var reversed=items.Reverse().ToArray();
                Check(!ClientRunHistoryPresentationRuntime.IsValid(snapshot,reversed),
                      "presentation projection should detect reordered display items");
        }
        for(var i1=0;i1<10;i1++)
        {
            round++;
                var snapshot=CreateHistorySnapshot();
                var items=ClientRunHistoryPresentationRuntime.CreateItems(snapshot,5);
                var reversed=items.Reverse().ToArray();
                Check(!ClientRunHistoryPresentationRuntime.IsValid(snapshot,reversed),
                      "presentation projection should detect reordered display items");
        }
        for(var i2=0;i2<10;i2++)
        {
            round++;
                var snapshot=CreateHistorySnapshot();
                var items=ClientRunHistoryPresentationRuntime.CreateItems(snapshot,5);
                var reversed=items.Reverse().ToArray();
                Check(!ClientRunHistoryPresentationRuntime.IsValid(snapshot,reversed),
                      "presentation projection should detect reordered display items");
        }
        for(var i3=0;i3<10;i3++)
        {
            round++;
                var snapshot=CreateHistorySnapshot();
                var items=ClientRunHistoryPresentationRuntime.CreateItems(snapshot,5);
                var reversed=items.Reverse().ToArray();
                Check(!ClientRunHistoryPresentationRuntime.IsValid(snapshot,reversed),
                      "presentation projection should detect reordered display items");
        }
        for(var i4=0;i4<10;i4++)
        {
            round++;
                var snapshot=CreateHistorySnapshot();
                var items=ClientRunHistoryPresentationRuntime.CreateItems(snapshot,5);
                var reversed=items.Reverse().ToArray();
                Check(!ClientRunHistoryPresentationRuntime.IsValid(snapshot,reversed),
                      "presentation projection should detect reordered display items");
        }
        for(var i5=0;i5<10;i5++)
        {
            round++;
                var snapshot=CreateHistorySnapshot();
                var items=ClientRunHistoryPresentationRuntime.CreateItems(snapshot,5);
                var reversed=items.Reverse().ToArray();
                Check(!ClientRunHistoryPresentationRuntime.IsValid(snapshot,reversed),
                      "presentation projection should detect reordered display items");
        }
        for(var i6=0;i6<10;i6++)
        {
            round++;
                var snapshot=CreateHistorySnapshot();
                var items=ClientRunHistoryPresentationRuntime.CreateItems(snapshot,5);
                var reversed=items.Reverse().ToArray();
                Check(!ClientRunHistoryPresentationRuntime.IsValid(snapshot,reversed),
                      "presentation projection should detect reordered display items");
        }
        for(var i7=0;i7<10;i7++)
        {
            round++;
                var snapshot=CreateHistorySnapshot();
                var items=ClientRunHistoryPresentationRuntime.CreateItems(snapshot,5);
                var reversed=items.Reverse().ToArray();
                Check(!ClientRunHistoryPresentationRuntime.IsValid(snapshot,reversed),
                      "presentation projection should detect reordered display items");
        }
        for(var i8=0;i8<10;i8++)
        {
            round++;
                var snapshot=CreateHistorySnapshot();
                var items=ClientRunHistoryPresentationRuntime.CreateItems(snapshot,5);
                var reversed=items.Reverse().ToArray();
                Check(!ClientRunHistoryPresentationRuntime.IsValid(snapshot,reversed),
                      "presentation projection should detect reordered display items");
        }
        for(var i9=0;i9<10;i9++)
        {
            round++;
                var snapshot=CreateHistorySnapshot();
                var items=ClientRunHistoryPresentationRuntime.CreateItems(snapshot,5);
                var reversed=items.Reverse().ToArray();
                Check(!ClientRunHistoryPresentationRuntime.IsValid(snapshot,reversed),
                      "presentation projection should detect reordered display items");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("history presentation smoke must execute exactly 100 rounds");
    }
}
