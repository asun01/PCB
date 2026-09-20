namespace Asun.Platform.ClientIntegration;

public sealed record ClientQualityFilter(
    string Outcome,
    string Severity)
{
    public static ClientQualityFilter All { get; }=new("All","All");
}

public static class ClientQualityFilterRuntime
{
    public static IReadOnlyList<ClientQualityFindingDisplayItem> Apply(
        ClientQualityWorkspaceSnapshot snapshot,
        ClientQualityFilter filter)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(filter);

        return snapshot.Findings
            .Where(item=>
                (filter.Outcome=="All" || item.Outcome==filter.Outcome) &&
                (filter.Severity=="All" || item.Severity==filter.Severity))
            .ToArray();
    }
}
