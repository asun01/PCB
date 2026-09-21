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
};

public static class ClientQualitySurfaceRuntime
{
    public static ClientQualitySurface Create(
        ClientInspectionWorkspaceSnapshot snapshot,
        ClientQualityFilter? filter=null)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var quality=snapshot.Quality;
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

        return new ClientQualitySurface(
            quality,
            quality.Findings.Count>0,
            selectionText,
            selectedFinding)
        {
            Filter=activeFilter,
            VisibleFindings=visibleFindings,
            SelectedFindingVisible=selectedFindingVisible
        };
    }
}
