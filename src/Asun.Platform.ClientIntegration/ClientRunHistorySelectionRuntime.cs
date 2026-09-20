namespace Asun.Platform.ClientIntegration;

public sealed record ClientRunHistorySelection(
    long? SelectedOrdinal,
    long SelectionSequence,
    string StatusText);

public static class ClientRunHistorySelectionRuntime
{
    public static ClientRunHistorySelection CreateInitial() =>
        new(null,0,"No historical run selected.");

    public static ClientRunHistorySelection Select(
        ClientProductionRunHistorySnapshot snapshot,
        long ordinal,
        long previousSelectionSequence)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if(previousSelectionSequence<0)
            throw new ArgumentOutOfRangeException(nameof(previousSelectionSequence));

        var entry=snapshot.Entries.FirstOrDefault(item=>item.Ordinal==ordinal);
        if(entry is null)
            return new(
                null,
                previousSelectionSequence,
                $"Run {ordinal} is not available in the bounded history.");

        return new(
            entry.Ordinal,
            checked(previousSelectionSequence+1),
            $"Selected Run {entry.Ordinal} · Session {entry.ProductionSessionId} · {entry.FrameCount} frame(s) · {(entry.ReleaseReady ? "Release Ready" : "Release Not Ready")}.");
    }
}
