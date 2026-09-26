namespace Asun.Platform.ClientIntegration;

public sealed record ClientQualitySurface(
    ClientQualityWorkspaceSnapshot Snapshot,
    bool HasFindings,
    string SelectionText,
    ClientQualityFindingDisplayItem? SelectedFinding)
{
    public ClientQualityFilter Filter { get; init; }=ClientQualityFilter.All;
    public bool SelectedFindingVisible { get; init; }
    public IReadOnlyList<ClientQualityFindingDisplayItem> VisibleFindings { get; init; }=
        Array.Empty<ClientQualityFindingDisplayItem>();
    public string AuthorityText { get; init; }="Quality authority: not bound.";
    public string SummaryText { get; init; }="Quality: no results."; 
    public string ProviderText { get; init; }="Provider: none."; 
};

public static class ClientQualitySurfaceRuntime
{
    public static ClientQualitySurface Create(
        ClientInspectionWorkspaceSnapshot snapshot,
        ClientQualityFilter? filter=null)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        return Create(snapshot.Quality,filter);
    }

    public static ClientQualitySurface Create(
        ClientQualityWorkspaceSnapshot quality,
        ClientQualityFilter? filter=null)
    {
        ArgumentNullException.ThrowIfNull(quality);

        var activeFilter=filter ?? ClientQualityFilter.All;
        var selected=quality.SelectedFindingId;
        var visibleFindings=ClientQualityFilterRuntime.Apply(quality,activeFilter);
        var selectedFinding=selected is null
            ? null
            : quality.Findings.FirstOrDefault(item=>item.FindingId==selected);

        var selectedFindingVisible=selectedFinding is not null &&
                                   visibleFindings.Any(item=>item.FindingId==selectedFinding.FindingId);

        var selectionText=selectedFinding is null
            ? "No finding selected."
            : selectedFindingVisible
                ? $"Selected finding {selectedFinding.FindingId}"
                : $"Selected finding {selectedFinding.FindingId} is hidden by the current filter.";

        var authorityText=quality.IsBound &&
                          quality.Fingerprint is { Length:64 }
            ? $"Quality authority: Bound · {quality.Fingerprint[..12]}..."
            : "Quality authority: not bound.";
        var summaryText=quality.IsBound
            ? $"Quality: {quality.ResultCount} result(s) · {quality.FindingCount} finding(s) · Pass {quality.PassCount} · Fail {quality.FailCount} · Review {quality.ReviewCount} · Evidence {quality.EvidenceLinkCount}."
            : "Quality: no authoritative run attached.";
        var providerText=quality.Provider is { } provider
            ? $"Provider: {provider.DisplayName}" + (provider.IsSimulation ? " · Simulation." : ".")
            : "Provider: none.";

        return new ClientQualitySurface(
            quality,
            quality.Findings.Count>0,
            selectionText,
            selectedFinding)
        {
            Filter=activeFilter,
            VisibleFindings=visibleFindings,
            SelectedFindingVisible=selectedFindingVisible,
            AuthorityText=authorityText,
            SummaryText=summaryText,
            ProviderText=providerText
        };
    }
}
