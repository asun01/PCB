namespace Asun.UI.Viewports;

public static class TileLoadHealthValidationRuntime
{
    public static IReadOnlyList<string> Validate(TileLoadHealth health)
    {
        var errors = new List<string>();

        if (health.CacheCount < 0 ||
            health.InFlight < 0 ||
            health.Hits < 0 ||
            health.Misses < 0 ||
            health.Evictions < 0)
        {
            errors.Add("Tile load health counters cannot be negative.");
        }

        return errors;
    }

    public static bool IsValid(TileLoadHealth health) =>
        Validate(health).Count == 0;
}
