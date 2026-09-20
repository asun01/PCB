namespace Asun.Platform.SimulationIntegration;

public sealed record ProductionSimulationRenderReplayDescriptor(
    long Sequence,
    string SimulationObservationFingerprint,
    string RenderFingerprint);

public static class ProductionSimulationRenderReplayDescriptorRuntime
{
    public static IReadOnlyList<ProductionSimulationRenderReplayDescriptor> Create(
        IReadOnlyList<ProductionSimulationRenderReplayFrame> replay)
    {
        ArgumentNullException.ThrowIfNull(replay);

        var ordered=replay.OrderBy(frame=>frame.Sequence).ToArray();
        if(ordered.Select(frame=>frame.Sequence).Distinct().Count()!=ordered.Length)
            throw new ArgumentException("Simulation/Render replay sequences must be unique.",nameof(replay));

        if(ordered.Any(frame=>frame.Sequence<=0 ||
                              frame.SimulationObservationFingerprint.Length!=64 ||
                              frame.RenderFingerprint.Length!=64))
            throw new ArgumentException("Simulation/Render replay descriptor input is invalid.",nameof(replay));

        return ordered
            .Select(frame=>new ProductionSimulationRenderReplayDescriptor(
                frame.Sequence,
                frame.SimulationObservationFingerprint,
                frame.RenderFingerprint))
            .ToArray();
    }

    public static IReadOnlyList<string> Validate(
        IReadOnlyList<ProductionSimulationRenderReplayFrame> replay,
        IReadOnlyList<ProductionSimulationRenderReplayDescriptor> descriptor)
    {
        ArgumentNullException.ThrowIfNull(replay);
        ArgumentNullException.ThrowIfNull(descriptor);

        var errors=new List<string>();
        var expected=Create(replay);
        var actual=descriptor.OrderBy(item=>item.Sequence).ToArray();

        if(expected.Count!=actual.Length)
            errors.Add("Replay descriptor count must match replay frame count.");

        var count=Math.Min(expected.Count,actual.Length);
        for(var index=0;index<count;index++)
        {
            if(!actual[index].Equals(expected[index]))
                errors.Add($"Replay descriptor {index} does not match canonical replay content.");
        }

        if(actual.Select(item=>item.Sequence).Distinct().Count()!=actual.Length)
            errors.Add("Replay descriptor sequences must be unique.");

        return errors;
    }

    public static bool IsValid(
        IReadOnlyList<ProductionSimulationRenderReplayFrame> replay,
        IReadOnlyList<ProductionSimulationRenderReplayDescriptor> descriptor)=>
        Validate(replay,descriptor).Count==0;
}
