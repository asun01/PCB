namespace Asun.Platform.ClientIntegration;

public sealed record ClientRunHistoryDisplayItem(
    long Ordinal,
    string SessionText,
    string FrameText,
    string ReleaseText,
    string ReplayText,
    bool ReleaseReady)
{
    public string QualityFingerprint { get; init; }="";
    public Guid ProductionSessionId { get; init; }
};

public static class ClientRunHistoryPresentationRuntime
{
    public static ClientRunHistoryDisplayItem CreateItem(
        ClientProductionRunHistoryEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        return new ClientRunHistoryDisplayItem(
            entry.Ordinal,
            $"Session {entry.ProductionSessionId}",
            $"{entry.FrameCount} frame(s)",
            entry.ReleaseReady
                ? $"Release Ready · {entry.ArtifactPath}"
                : "Release Not Ready",
            $"Replay {entry.ReplayFingerprint[..12]}...",
            entry.ReleaseReady)
        {
            ProductionSessionId=entry.ProductionSessionId,
            QualityFingerprint=entry.QualityFingerprint,
            QualityText=$"Quality {entry.QualityFingerprint[..12]}..."
        };
    }

    public static IReadOnlyList<ClientRunHistoryDisplayItem> CreateItems(
        ClientProductionRunHistorySnapshot snapshot,
        int maxItems=5)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        if(maxItems<=0)
            throw new ArgumentOutOfRangeException(nameof(maxItems));

        return snapshot.Entries
            .OrderByDescending(entry=>entry.Ordinal)
            .Take(maxItems)
            .Select(CreateItem)
            .ToArray();
    }

    public static bool IsValid(
        ClientProductionRunHistorySnapshot snapshot,
        IReadOnlyList<ClientRunHistoryDisplayItem> items)=>
        items.SequenceEqual(
            CreateItems(snapshot,Math.Max(1,items.Count)));
}
