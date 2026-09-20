using Asun.Platform.RenderIntegration;

namespace Asun.Platform.SimulationIntegration;

public sealed record ProductionSimulationRenderReplayFrame(
    long Sequence,
    string SimulationObservationFingerprint,
    string RenderFingerprint);

public static class ProductionSimulationRenderReplayRuntime
{
    public static IReadOnlyList<ProductionSimulationRenderReplayFrame> Create(
        IReadOnlyList<ProductionSimulationFrameLink> simulationFrames,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> renderFrames)
    {
        ArgumentNullException.ThrowIfNull(simulationFrames);
        ArgumentNullException.ThrowIfNull(renderFrames);

        var simulation=simulationFrames.OrderBy(frame=>frame.SimulationSequence).ToArray();
        var render=renderFrames.OrderBy(frame=>frame.Sequence).ToArray();
        ValidateSourceIdentity(simulation,render);
        if(simulation.Length!=render.Length)
            throw new ArgumentException("Simulation and Render replay frame counts must match.",nameof(renderFrames));

        var result=new List<ProductionSimulationRenderReplayFrame>(simulation.Length);
        for(var index=0;index<simulation.Length;index++)
        {
            if(simulation[index].SimulationSequence!=render[index].Sequence)
                throw new ArgumentException($"Simulation/Render sequence mismatch at index {index}.",nameof(renderFrames));

            result.Add(new ProductionSimulationRenderReplayFrame(
                render[index].Sequence,
                simulation[index].SimulationObservationFingerprint,
                render[index].RenderFingerprint));
        }

        return result;
    }

    public static IReadOnlyList<string> Validate(
        IReadOnlyList<ProductionSimulationFrameLink> simulationFrames,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> renderFrames,
        IReadOnlyList<ProductionSimulationRenderReplayFrame> replay)
    {
        ArgumentNullException.ThrowIfNull(simulationFrames);
        ArgumentNullException.ThrowIfNull(renderFrames);
        ArgumentNullException.ThrowIfNull(replay);

        var errors=new List<string>();
        var simulation=simulationFrames.OrderBy(frame=>frame.SimulationSequence).ToArray();
        var render=renderFrames.OrderBy(frame=>frame.Sequence).ToArray();
        errors.AddRange(ValidateSourceIdentity(simulation,render));
        var actual=replay.OrderBy(frame=>frame.Sequence).ToArray();

        if(simulation.Length!=render.Length || actual.Length!=render.Length)
            errors.Add("Simulation/Render replay counts must match.");

        var count=Math.Min(actual.Length,Math.Min(simulation.Length,render.Length));
        for(var index=0;index<count;index++)
        {
            if(actual[index].Sequence!=render[index].Sequence || actual[index].Sequence!=simulation[index].SimulationSequence)
                errors.Add($"Simulation/Render replay sequence {index} is misaligned.");
            if(actual[index].SimulationObservationFingerprint!=simulation[index].SimulationObservationFingerprint)
                errors.Add($"Simulation/Render replay simulation fingerprint {index} is mismatched.");
            if(actual[index].RenderFingerprint!=render[index].RenderFingerprint)
                errors.Add($"Simulation/Render replay render fingerprint {index} is mismatched.");
        }

        if(actual.Select(frame=>frame.Sequence).Distinct().Count()!=actual.Length)
            errors.Add("Simulation/Render replay sequences must be unique.");

        return errors;
    }

    public static bool IsValid(
        IReadOnlyList<ProductionSimulationFrameLink> simulationFrames,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> renderFrames,
        IReadOnlyList<ProductionSimulationRenderReplayFrame> replay)=>
        Validate(simulationFrames,renderFrames,replay).Count==0;

    private static IReadOnlyList<string> ValidateSourceIdentity(
        IReadOnlyList<ProductionSimulationFrameLink> simulation,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> render)
    {
        var errors=new List<string>();

        if(simulation.Select(frame=>frame.SimulationSequence).Distinct().Count()!=simulation.Count)
            errors.Add("Simulation replay sequences must be unique.");
        if(render.Select(frame=>frame.Sequence).Distinct().Count()!=render.Count)
            errors.Add("Render replay sequences must be unique.");

        if(simulation.Any(frame=>string.IsNullOrWhiteSpace(frame.SimulationObservationFingerprint) ||
                                  frame.SimulationObservationFingerprint.Length!=64))
            errors.Add("Simulation observation fingerprints must be 64 characters.");
        if(render.Any(frame=>string.IsNullOrWhiteSpace(frame.RenderFingerprint) ||
                              frame.RenderFingerprint.Length!=64))
            errors.Add("Render fingerprints must be 64 characters.");

        return errors;
    }
}
