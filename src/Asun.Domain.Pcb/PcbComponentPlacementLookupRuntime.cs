namespace Asun.Domain.Pcb;

public static class PcbComponentPlacementLookupRuntime
{
    public static PcbComponentReference? FindByDesignator(
        IReadOnlyList<PcbComponentReference> components,
        string designator)
    {
        ArgumentNullException.ThrowIfNull(components);

        if(string.IsNullOrWhiteSpace(designator))
            throw new ArgumentException("Designator cannot be blank.",nameof(designator));

        return components.FirstOrDefault(
            component=>string.Equals(
                component.Designator,
                designator.Trim(),
                StringComparison.Ordinal));
    }

    public static IReadOnlyList<PcbComponentReference> FindByPackage(
        IReadOnlyList<PcbComponentReference> components,
        string packageName)
    {
        ArgumentNullException.ThrowIfNull(components);

        if(string.IsNullOrWhiteSpace(packageName))
            throw new ArgumentException("Package name cannot be blank.",nameof(packageName));

        return components
            .Where(component=>string.Equals(
                component.PackageName,
                packageName.Trim(),
                StringComparison.Ordinal))
            .OrderBy(component=>component.Designator,StringComparer.Ordinal)
            .ToArray();
    }
}
