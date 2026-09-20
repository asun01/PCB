using System.Security.Cryptography;
using System.Text;
using Asun.Simulation.Core;

namespace Asun.Platform.SimulationIntegration;

public sealed record ProductionSimulationReplayDescriptor(
    Guid ProductionSessionId,
    string ProductionFingerprint,
    int FrameCount,
    string BindingFingerprint,
    string DescriptorFingerprint);

public static class ProductionSimulationReplayDescriptorRuntime
{
    public static ProductionSimulationReplayDescriptor Create(
        Asun.Production.Runtime.ProductionSessionReport productionReport,
        IReadOnlyList<SimulationObservation> observations,
        ProductionSimulationReplayBinding binding)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(observations);
        ArgumentNullException.ThrowIfNull(binding);

        if(!ProductionSimulationReplayBindingValidationRuntime.IsValid(productionReport,observations,binding))
            throw new ArgumentException("Production simulation replay binding is invalid.",nameof(binding));

        var canonical=string.Join(
            "|",
            binding.ProductionSessionId,
            binding.ProductionFingerprint,
            binding.Frames.Count,
            binding.Fingerprint);
        var fingerprint=Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();

        return new ProductionSimulationReplayDescriptor(
            binding.ProductionSessionId,
            binding.ProductionFingerprint,
            binding.Frames.Count,
            binding.Fingerprint,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        Asun.Production.Runtime.ProductionSessionReport productionReport,
        IReadOnlyList<SimulationObservation> observations,
        ProductionSimulationReplayBinding binding,
        ProductionSimulationReplayDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(observations);
        ArgumentNullException.ThrowIfNull(binding);
        ArgumentNullException.ThrowIfNull(descriptor);

        var errors=new List<string>();
        if(!ProductionSimulationReplayBindingValidationRuntime.IsValid(productionReport,observations,binding))
            errors.Add("Production simulation replay binding is invalid.");
        if(descriptor.ProductionSessionId!=binding.ProductionSessionId)
            errors.Add("Replay descriptor production session identity must match.");
        if(descriptor.ProductionFingerprint!=binding.ProductionFingerprint)
            errors.Add("Replay descriptor production fingerprint must match.");
        if(descriptor.FrameCount!=binding.Frames.Count)
            errors.Add("Replay descriptor frame count must match.");
        if(descriptor.BindingFingerprint!=binding.Fingerprint)
            errors.Add("Replay descriptor binding fingerprint must match.");
        if(descriptor.DescriptorFingerprint.Length!=64 || !descriptor.DescriptorFingerprint.All(Uri.IsHexDigit))
            errors.Add("Replay descriptor fingerprint must be 64 hexadecimal characters.");

        if(errors.Count>0)
            return errors;

        var expected=Create(productionReport,observations,binding);
        if(expected.DescriptorFingerprint!=descriptor.DescriptorFingerprint)
            errors.Add("Replay descriptor fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        Asun.Production.Runtime.ProductionSessionReport productionReport,
        IReadOnlyList<SimulationObservation> observations,
        ProductionSimulationReplayBinding binding,
        ProductionSimulationReplayDescriptor descriptor)=>
        Validate(productionReport,observations,binding,descriptor).Count==0;

    public static bool IsEquivalent(
        ProductionSimulationReplayDescriptor left,
        ProductionSimulationReplayDescriptor right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.DescriptorFingerprint==right.DescriptorFingerprint;
    }
}
