using System.Security.Cryptography;
using System.Text;
using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.SimulationIntegration;
using Asun.Simulation.Core;

namespace Asun.Platform.PcbSimulationIntegration;

public static class PcbExecutionSimulationReplayProjectionRuntime
{
    public static PcbExecutionSimulationReplayProjection Create(
        PcbExecutionSnapshot executionSnapshot,
        IReadOnlyList<SimulationObservation> observations,
        ProductionSimulationReplayBinding simulationBinding)
    {
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(observations);
        ArgumentNullException.ThrowIfNull(simulationBinding);

        if(simulationBinding.ProductionFingerprint!=executionSnapshot.ProductionFingerprint)
            throw new ArgumentException("Simulation binding production fingerprint must match execution snapshot.",nameof(simulationBinding));

        if(simulationBinding.Frames.Count!=executionSnapshot.FrameCount ||
           observations.Count!=executionSnapshot.FrameCount)
            throw new ArgumentException("Simulation replay frame counts must match the execution snapshot.");

        var fingerprint=CreateFingerprint(
            executionSnapshot,
            observations,
            simulationBinding);

        return new PcbExecutionSimulationReplayProjection(
            executionSnapshot.Fingerprint,
            executionSnapshot.ProductionSessionId.ToString(),
            simulationBinding.Fingerprint,
            executionSnapshot.FrameCount,
            observations.Count,
            fingerprint);
    }

    internal static string CreateFingerprint(
        PcbExecutionSnapshot executionSnapshot,
        IReadOnlyList<SimulationObservation> observations,
        ProductionSimulationReplayBinding simulationBinding)
    {
        var canonical=new StringBuilder();
        canonical.Append(executionSnapshot.Fingerprint).Append('|')
            .Append(executionSnapshot.ProductionSessionId).Append('|')
            .Append(simulationBinding.Fingerprint).Append('|');
        foreach(var observation in observations.OrderBy(item=>item.Sequence.Value))
            canonical.Append(observation.Sequence.Value).Append('|')
                .Append(observation.Fingerprint).Append('|');

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical.ToString())))
            .ToLowerInvariant();
    }
}
