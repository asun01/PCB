namespace Asun.Platform.ClientIntegration;

public sealed record ClientQualitySurface(
    ClientQualityWorkspaceSnapshot Snapshot,
    bool HasFindings,
    string SelectionText);

public static class ClientQualitySurfaceRuntime
{
    public static ClientQualitySurface Create(
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var quality=snapshot.Quality;
        var selected=quality.SelectedFindingId;
        var hasSelection=selected is not null &&
                         quality.Findings.Any(item=>item.FindingId==selected);

        var selectionText=hasSelection
            ? $"Selected finding {selected}"
            : "No finding selected.";

        return new ClientQualitySurface(
            quality,
            quality.Findings.Count>0,
            selectionText);
    }
}
