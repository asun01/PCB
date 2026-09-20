using System.Security.Cryptography;
using System.Text;
using Asun.Platform.RenderIntegration;
using Asun.Platform.SimulationIntegration;

namespace Asun.Platform.ReplayIntegration;

public sealed record ProductionSimulationRenderReplayConvergence(
    Guid ProductionSessionId,
    string SimulationDescriptorFingerprint,
    int FrameCount,
    string RenderDescriptorFingerprint,
    string Fingerprint);

public static class ProductionSimulationRenderReplayConvergenceRuntime
{
    public static ProductionSimulationRenderReplayConvergence Create(
        ProductionSimulationReplayDescriptor simulationDescriptor,
        ProductionSimulationReplayBinding simulationBinding,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> renderFrames,
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> renderDescriptors)
    {
        ArgumentNullException.ThrowIfNull(simulationDescriptor);
        ArgumentNullException.ThrowIfNull(simulationBinding);
        ArgumentNullException.ThrowIfNull(renderFrames);
        ArgumentNullException.ThrowIfNull(renderDescriptors);

        var errors=Validate(
            simulationDescriptor,
            simulationBinding,
            renderFrames,
            renderDescriptors);

        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var renderDescriptorFingerprint=CreateRenderDescriptorFingerprint(renderDescriptors);
        var fingerprint=CreateFingerprint(
            simulationDescriptor.ProductionSessionId,
            simulationDescriptor.DescriptorFingerprint,
            simulationDescriptor.FrameCount,
            renderDescriptorFingerprint);

        return new ProductionSimulationRenderReplayConvergence(
            simulationDescriptor.ProductionSessionId,
            simulationDescriptor.DescriptorFingerprint,
            simulationDescriptor.FrameCount,
            renderDescriptorFingerprint,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionSimulationReplayDescriptor simulationDescriptor,
        ProductionSimulationReplayBinding simulationBinding,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> renderFrames,
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> renderDescriptors)
    {
        ArgumentNullException.ThrowIfNull(simulationDescriptor);
        ArgumentNullException.ThrowIfNull(simulationBinding);
        ArgumentNullException.ThrowIfNull(renderFrames);
        ArgumentNullException.ThrowIfNull(renderDescriptors);

        var errors=new List<string>();

        if(simulationDescriptor.ProductionSessionId==Guid.Empty)
            errors.Add("Simulation descriptor Production session identity cannot be empty.");
        if(simulationDescriptor.FrameCount<=0)
            errors.Add("Simulation descriptor frame count must be positive.");
        if(simulationDescriptor.DescriptorFingerprint.Length!=64 ||
           !simulationDescriptor.DescriptorFingerprint.All(Uri.IsHexDigit))
            errors.Add("Simulation descriptor fingerprint is malformed.");

        if(simulationBinding.ProductionSessionId!=simulationDescriptor.ProductionSessionId)
            errors.Add("Simulation binding session identity must match descriptor.");
        if(simulationBinding.Frames.Count!=simulationDescriptor.FrameCount)
            errors.Add("Simulation binding frame count must match descriptor.");
        if(renderFrames.Count!=simulationDescriptor.FrameCount)
            errors.Add("Render replay frame count must match simulation descriptor.");

        var simFrames=simulationBinding.Frames.OrderBy(frame=>frame.SimulationSequence).ToArray();
        var orderedRenderFrames=renderFrames.OrderBy(frame=>frame.Sequence).ToArray();
        var orderedDescriptors=renderDescriptors.OrderBy(frame=>frame.Sequence).ToArray();

        if(renderDescriptors.Count!=renderFrames.Count)
            errors.Add("Render evidence descriptor count must match render replay frame count.");

        var count=Math.Min(simFrames.Length,orderedRenderFrames.Length);
        for(var index=0;index<count;index++)
        {
            if(simFrames[index].ProductionSequence!=orderedRenderFrames[index].Sequence)
                errors.Add($"Simulation/render sequence mismatch at index {index}.");

            if(simFrames[index].ProductionInputFingerprint!=orderedRenderFrames[index].ProductionInputFingerprint)
                errors.Add($"Simulation/render production input fingerprint mismatch at index {index}.");
        }

        var descriptorCount=Math.Min(orderedRenderFrames.Length,orderedDescriptors.Length);
        for(var index=0;index<descriptorCount;index++)
        {
            if(orderedDescriptors[index].Sequence!=orderedRenderFrames[index].Sequence)
                errors.Add($"Render evidence descriptor sequence mismatch at index {index}.");

            if(orderedDescriptors[index].RenderFingerprint!=orderedRenderFrames[index].RenderFingerprint)
                errors.Add($"Render evidence descriptor fingerprint mismatch at index {index}.");
        }

        if(orderedDescriptors.Select(item=>item.Sequence).Distinct().Count()!=orderedDescriptors.Length)
            errors.Add("Render evidence descriptor sequences must be unique.");

        if(orderedRenderFrames.Select(item=>item.Sequence).Distinct().Count()!=orderedRenderFrames.Length)
            errors.Add("Render replay frame sequences must be unique.");

        return errors;
    }

    public static bool IsValid(
        ProductionSimulationReplayDescriptor simulationDescriptor,
        ProductionSimulationReplayBinding simulationBinding,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> renderFrames,
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> renderDescriptors)=>
        Validate(simulationDescriptor,simulationBinding,renderFrames,renderDescriptors).Count==0;

    private static string CreateRenderDescriptorFingerprint(
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> descriptors)=>
        Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(
                    string.Join(
                        "|",
                        descriptors
                            .OrderBy(item=>item.Sequence)
                            .Select(item=>$"{item.Sequence}:{item.DescriptorFingerprint}")))))
            .ToLowerInvariant();

    private static string CreateFingerprint(
        Guid productionSessionId,
        string simulationDescriptorFingerprint,
        int frameCount,
        string renderDescriptorFingerprint)=>
        Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(
                    string.Join(
                        "|",
                        productionSessionId,
                        simulationDescriptorFingerprint,
                        frameCount,
                        renderDescriptorFingerprint))))
            .ToLowerInvariant();
}
