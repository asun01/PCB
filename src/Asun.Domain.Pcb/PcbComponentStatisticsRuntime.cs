namespace Asun.Domain.Pcb;

public static class PcbComponentStatisticsRuntime
{
    public static PcbComponentStatistics Create(
        IReadOnlyList<PcbComponentReference> components)
    {
        ArgumentNullException.ThrowIfNull(components);

        if(components.Any(component=>!component.IsValid()))
            throw new ArgumentException("Component statistics require valid components.",nameof(components));

        return new PcbComponentStatistics(
            components.Count,
            components.Count(component=>component.Side==PcbLayerSide.Top),
            components.Count(component=>component.Side==PcbLayerSide.Bottom),
            components.Count(component=>component.Side==PcbLayerSide.Internal));
    }
}
