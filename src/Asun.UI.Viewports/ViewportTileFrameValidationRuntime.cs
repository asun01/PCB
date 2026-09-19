namespace Asun.UI.Viewports;

public static class ViewportTileFrameValidationRuntime
{
    public static IReadOnlyList<string> Validate<TTile>(
        ViewportTileFrame<TTile> frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var errors = new List<string>();

        if (frame.Requests.Select(request => request.Index).Distinct().Count() !=
            frame.Requests.Count)
        {
            errors.Add("Tile frame request indices must be unique.");
        }

        if (frame.LoadedTiles.Keys.Any(index =>
            !frame.Requests.Any(request => request.Index == index)))
        {
            errors.Add("Loaded tiles must be backed by a requested tile index.");
        }

        if (frame.IsCompleteForAllRequests &&
            frame.Failures.Count != 0)
        {
            errors.Add("A complete-for-all tile frame cannot contain failures.");
        }

        if (frame.LoadedCount > frame.RequestedCount)
            errors.Add("Loaded tile count cannot exceed requested tile count.");

        var missingVisible = frame.MissingVisibleRequests();

        if (frame.IsCompleteForVisible && missingVisible.Count != 0)
            errors.Add("A visible-complete tile frame cannot report missing visible requests.");

        return errors;
    }

    public static bool IsValid<TTile>(
        ViewportTileFrame<TTile> frame) =>
        Validate(frame).Count == 0;
}
