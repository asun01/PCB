using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.SimulationIntegration;

public sealed record ProductionSimulationRenderProvenance(
    long ProductionSequence,
    string ProductionInputFingerprint,
    long SimulationSequence,
    string SimulationObservationFingerprint,
    string RenderFingerprint);

public static class ProductionSimulationRenderProvenanceRuntime
{
    public static IReadOnlyList<ProductionSimulationRenderProvenance> Create(
        IReadOnlyList<ProductionSimulationFrameLink> simulationFrames,
        IReadOnlyList<Asun.Platform.RenderIntegration.ProductionRenderReplayFrameIntegrity> renderFrames)
    {
        ArgumentNullException.ThrowIfNull(simulationFrames);
        ArgumentNullException.ThrowIfNull(renderFrames);

        var simulation=simulationFrames.OrderBy(frame=>frame.SimulationSequence).ToArray();
        var render=renderFrames.OrderBy(frame=>frame.Sequence).ToArray();
        var errors=ValidateSources(simulation,render);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors),nameof(renderFrames));

        return simulation.Select((frame,index)=>new ProductionSimulationRenderProvenance(
            frame.ProductionSequence,
            frame.ProductionInputFingerprint,
            frame.SimulationSequence,
            frame.SimulationObservationFingerprint,
            render[index].RenderFingerprint)).ToArray();
    }

    public static IReadOnlyList<string> Validate(
        IReadOnlyList<ProductionSimulationFrameLink> simulationFrames,
        IReadOnlyList<Asun.Platform.RenderIntegration.ProductionRenderReplayFrameIntegrity> renderFrames,
        IReadOnlyList<ProductionSimulationRenderProvenance> provenance)
    {
        ArgumentNullException.ThrowIfNull(simulationFrames);
        ArgumentNullException.ThrowIfNull(renderFrames);
        ArgumentNullException.ThrowIfNull(provenance);

        var simulation=simulationFrames.OrderBy(frame=>frame.SimulationSequence).ToArray();
        var render=renderFrames.OrderBy(frame=>frame.Sequence).ToArray();
        var actual=provenance.OrderBy(item=>item.SimulationSequence).ToArray();
        var errors=ValidateSources(simulation,render);

        if(actual.Length!=simulation.Length || actual.Length!=render.Length)
            errors.Add("Simulation/Render provenance counts must match.");

        var count=Math.Min(actual.Length,Math.Min(simulation.Length,render.Length));
        for(var index=0;index<count;index++)
        {
            var expectedSimulation=simulation[index];
            var expectedRender=render[index];
            var item=actual[index];
            if(item.ProductionSequence!=expectedSimulation.ProductionSequence ||
               item.ProductionSequence!=expectedRender.Sequence ||
               item.SimulationSequence!=expectedSimulation.SimulationSequence)
                errors.Add($"Provenance sequence alignment failed at index {index}.");
            if(!string.Equals(item.ProductionInputFingerprint,expectedSimulation.ProductionInputFingerprint,StringComparison.Ordinal) ||
               !string.Equals(expectedRender.ProductionInputFingerprint,expectedSimulation.ProductionInputFingerprint,StringComparison.Ordinal))
                errors.Add($"Production input fingerprint alignment failed at index {index}.");
            if(!string.Equals(item.SimulationObservationFingerprint,expectedSimulation.SimulationObservationFingerprint,StringComparison.Ordinal))
                errors.Add($"Simulation observation fingerprint mismatch at index {index}.");
            if(!string.Equals(item.RenderFingerprint,expectedRender.RenderFingerprint,StringComparison.Ordinal))
                errors.Add($"Render fingerprint mismatch at index {index}.");
        }

        if(actual.Select(item=>item.ProductionSequence).Distinct().Count()!=actual.Length)
            errors.Add("Production provenance sequences must be unique.");
        if(actual.Select(item=>item.SimulationSequence).Distinct().Count()!=actual.Length)
            errors.Add("Simulation provenance sequences must be unique.");
        return errors;
    }

    public static string CreateFingerprint(IReadOnlyList<ProductionSimulationRenderProvenance> provenance)
    {
        ArgumentNullException.ThrowIfNull(provenance);
        var ordered=provenance.OrderBy(item=>item.SimulationSequence).ToArray();
        var canonical=string.Join("\n",ordered.Select(item=>
            $"{item.ProductionSequence}|{item.ProductionInputFingerprint}|{item.SimulationSequence}|{item.SimulationObservationFingerprint}|{item.RenderFingerprint}"));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }

    public static bool IsEquivalent(
        IReadOnlyList<ProductionSimulationRenderProvenance> left,
        IReadOnlyList<ProductionSimulationRenderProvenance> right)=>
        CreateFingerprint(left)==CreateFingerprint(right);

    public static IReadOnlyList<string> CreateReplayDescriptor(
        IReadOnlyList<ProductionSimulationRenderProvenance> provenance)
    {
        ArgumentNullException.ThrowIfNull(provenance);
        return provenance.OrderBy(item=>item.SimulationSequence)
            .Select(item=>$"{item.ProductionSequence}:{item.ProductionInputFingerprint}:{item.SimulationSequence}:{item.SimulationObservationFingerprint}:{item.RenderFingerprint}")
            .ToArray();
    }

    private static List<string> ValidateSources(
        IReadOnlyList<ProductionSimulationFrameLink> simulation,
        IReadOnlyList<Asun.Platform.RenderIntegration.ProductionRenderReplayFrameIntegrity> render)
    {
        var errors=new List<string>();
        if(simulation.Length!=render.Length)
            errors.Add("Simulation/Render source counts must match.");
        if(simulation.Select(frame=>frame.SimulationSequence).Distinct().Count()!=simulation.Length)
            errors.Add("Simulation source sequences must be unique.");
        if(render.Select(frame=>frame.Sequence).Distinct().Count()!=render.Length)
            errors.Add("Render source sequences must be unique.");
        var count=Math.Min(simulation.Length,render.Length);
        for(var index=0;index<count;index++)
        {
            if(simulation[index].SimulationSequence!=render[index].Sequence)
                errors.Add($"Simulation/Render sequence mismatch at index {index}.");
            if(!string.Equals(simulation[index].ProductionInputFingerprint,render[index].ProductionInputFingerprint,StringComparison.Ordinal))
                errors.Add($"Production input fingerprint mismatch at index {index}.");
            if(simulation[index].ProductionSequence!=render[index].Sequence)
                errors.Add($"Production/Render sequence mismatch at index {index}.");
        }
        return errors;
    }
}
