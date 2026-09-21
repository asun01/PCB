namespace Asun.Platform.ClientIntegration;

public sealed record ClientQualitySurface(
    ClientQualityWorkspaceSnapshot Snapshot,
    bool HasFindings,
    string SelectionText,
    ClientQualityFindingDisplayItem? SelectedFinding)
{
    public ClientQualityFilter Filter { get; init; }=ClientQualityFilter.All;
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

        var selectionText=selectedFinding is not null
            ? $"Selected finding {selectedFinding.FindingId}"
            : "No finding selected.";

        return new ClientQualitySurface(
            quality,
            quality.Findings.Count>0,
            selectionText,
            selectedFinding)
        {
            Filter=activeFilter,
            VisibleFindings=visibleFindings
        };
    }
}
