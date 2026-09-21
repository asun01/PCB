namespace Asun.Platform.ClientIntegration;

public sealed record ClientResultsSurface(
    ClientInspectionResultDisplay Current,
    IReadOnlyList<ClientRunHistoryDisplayItem> History,
    long? SelectedOrdinal,
    string SelectionText);

public static class ClientResultsSurfaceRuntime
{
    public static ClientResultsSurface Create(
        ClientInspectionWorkspaceSnapshot snapshot,
        int maxHistoryItems=10)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var current=ClientResultsPresentationRuntime.CreateCurrent(snapshot);
        var history=ClientResultsPresentationRuntime.CreateHistory(snapshot,maxHistoryItems);
        var selected=snapshot.SelectedHistoryOrdinal;
        var selectionText=selected is null
            ? "No historical run selected."
            : history.Any(item=>item.Ordinal==selected.Value)
                ? $"Selected Run {selected.Value}"
                : $"Selected Run {selected.Value} is outside the visible history window.";

        return new ClientResultsSurface(
            current,
            history,
            selected,
            selectionText);
    }
}
