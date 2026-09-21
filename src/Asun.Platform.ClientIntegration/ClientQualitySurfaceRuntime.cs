namespace Asun.Platform.ClientIntegration;

public sealed record ClientQualitySurface(
    ClientQualityWorkspaceSnapshot Snapshot,
    bool HasFindings,
    string SelectionText,
    ClientQualityFindingDisplayItem? SelectedFinding);

public static class ClientQualitySurfaceRuntime
{
    public static ClientQualitySurface Create(
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var quality=snapshot.Quality;
        var selected=quality.SelectedFindingId;
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
            selectedFinding);
    }
}
