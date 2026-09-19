namespace Asun.UI.Viewports;

public static class TileViewportDiagnosticsValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        TileViewportDiagnostics diagnostics)
    {
        var errors = new List<string>();

        if (diagnostics.Planned < 0 ||
            diagnostics.Visible < 0 ||
            diagnostics.Prefetch < 0 ||
            diagnostics.Loaded < 0 ||
            diagnostics.Failed < 0 ||
            diagnostics.CacheCount < 0 ||
            diagnostics.InFlight < 0 ||
            diagnostics.CacheHits < 0 ||
            diagnostics.CacheMisses < 0 ||
            diagnostics.Evictions < 0)
        {
            errors.Add("Tile viewport diagnostics counters cannot be negative.");
        }

        if (diagnostics.Visible + diagnostics.Prefetch > diagnostics.Planned)
            errors.Add("Visible plus prefetch requests cannot exceed planned requests.");

        if (diagnostics.Loaded > diagnostics.Planned)
            errors.Add("Loaded requests cannot exceed planned requests.");

        return errors;
    }

    public static bool IsValid(TileViewportDiagnostics diagnostics) =>
        Validate(diagnostics).Count == 0;
}
