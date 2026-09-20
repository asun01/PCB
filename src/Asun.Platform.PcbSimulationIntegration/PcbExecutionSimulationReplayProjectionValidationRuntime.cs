using Asun.Simulation.Core;
using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.SimulationIntegration;

namespace Asun.Platform.PcbSimulationIntegration;

public static class PcbExecutionSimulationReplayProjectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbExecutionSnapshot executionSnapshot,
        IReadOnlyList<SimulationObservation> observations,
        ProductionSimulationReplayBinding simulationBinding,
        PcbExecutionSimulationReplayProjection projection)
    {
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(observations);
        ArgumentNullException.ThrowIfNull(simulationBinding);
        ArgumentNullException.ThrowIfNull(projection);

        var errors=new List<string>();
        if(simulationBinding.ProductionFingerprint!=executionSnapshot.ProductionFingerprint)
            errors.Add("Simulation binding production fingerprint must match execution snapshot.");
        if(simulationBinding.Frames.Count!=executionSnapshot.FrameCount)
            errors.Add("Simulation binding frame count must match execution snapshot.");
        if(observations.Count!=executionSnapshot.FrameCount)
            errors.Add("Simulation observation count must match execution snapshot.");
        if(projection.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            errors.Add("Projection execution snapshot fingerprint must match.");
        if(projection.ProductionSessionId!=executionSnapshot.ProductionSessionId.ToString())
            errors.Add("Projection production session identity must match.");
        if(projection.SimulationBindingFingerprint!=simulationBinding.Fingerprint)
            errors.Add("Projection simulation binding fingerprint must match.");
        if(projection.FrameCount!=executionSnapshot.FrameCount)
            errors.Add("Projection frame count must match.");
        if(projection.ObservationCount!=observations.Count)
            errors.Add("Projection observation count must match.");

        var ordered=observations.OrderBy(observation=>observation.Sequence.Value).ToArray();
        if(ordered.Select(observation=>observation.Sequence.Value).Distinct().Count()!=ordered.Length)
            errors.Add("Simulation observation sequences must be unique.");

        for(var index=0;index<ordered.Length;index++)
        {
            if(ordered[index].Sequence.Value!=index+1)
                errors.Add($"Simulation replay observation {index} sequence is not contiguous.");
            if(ordered[index].Fingerprint.Length!=64 ||
               !ordered[index].Fingerprint.All(character=>
                   Uri.IsHexDigit(character) &&
                   char.ToLowerInvariant(character)==character))
                errors.Add($"Simulation replay observation {index} fingerprint is invalid.");
        }

        if(projection.Fingerprint.Length!=64 ||
           !projection.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Simulation replay projection fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count>0)
            return errors;

        var expected=PcbExecutionSimulationReplayProjectionRuntime.CreateFingerprint(
            executionSnapshot,
            ordered,
            simulationBinding);
        if(expected!=projection.Fingerprint)
            errors.Add("Simulation replay projection fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        PcbExecutionSnapshot executionSnapshot,
        IReadOnlyList<SimulationObservation> observations,
        ProductionSimulationReplayBinding simulationBinding,
        PcbExecutionSimulationReplayProjection projection)=>
        Validate(executionSnapshot,observations,simulationBinding,projection).Count==0;
}
