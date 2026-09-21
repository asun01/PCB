namespace Asun.Platform.ClientIntegration;

public sealed record ClientResultsSurface(
    ClientInspectionResultDisplay Current,
    IReadOnlyList<ClientRunHistoryDisplayItem> History,
    long? SelectedOrdinal,
    string SelectionText,
    ClientRunHistoryDisplayItem? SelectedHistoryItem);

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
        var selectedEntry=selected is null
            ? null
            : snapshot.History.Entries.FirstOrDefault(entry=>entry.Ordinal==selected.Value);
        var selectedHistoryItem=selectedEntry is null
            ? null
            : ClientRunHistoryPresentationRuntime.CreateItem(selectedEntry);

        var selectionText=selectedHistoryItem is not null
            ? $"Selected Run {selectedHistoryItem.Ordinal}"
            : selected is null
                ? "No historical run selected."
                : $"Selected Run {selected.Value} is outside the available history window.";

        return new ClientResultsSurface(
            current,
            history,
            selected,
            selectionText,
            selectedHistoryItem);
    }
}
