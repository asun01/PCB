namespace Asun.UI.Viewports;

public static class SimulatedTileSourceValidationRuntime
{
    public static IReadOnlyList<string> Validate<TTile>(
        SimulatedTileSource<TTile> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var errors = new List<string>();

        if (source.LoadCount < 0 ||
            source.ActiveLoadCount < 0 ||
            source.MaxConcurrentLoads < 0)
        {
            errors.Add("Simulated tile source counters cannot be negative.");
        }

        if (source.ActiveLoadCount > source.MaxConcurrentLoads)
            errors.Add("Active simulated loads cannot exceed recorded maximum concurrency.");

        if (source.MaxConcurrentLoads > source.LoadCount)
            errors.Add("Maximum concurrency cannot exceed total started loads.");

        return errors;
    }

    public static bool IsValid<TTile>(
        SimulatedTileSource<TTile> source) =>
        Validate(source).Count == 0;
}
