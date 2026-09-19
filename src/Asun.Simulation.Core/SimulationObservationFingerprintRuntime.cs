using System.Security.Cryptography;
using System.Text;

namespace Asun.Simulation.Core;

public static class SimulationObservationFingerprintRuntime
{
    public static string CreateFingerprint(
        SimulatedBoardScenario scenario,
        SimulationObservation observation)
    {
        ArgumentNullException.ThrowIfNull(scenario);
        ArgumentNullException.ThrowIfNull(observation);

        var canonical=new StringBuilder();
        canonical.Append(observation.Sequence.Value).Append('|')
            .Append(scenario.Assembly.Fingerprint).Append('|')
            .Append(scenario.Seed).Append('|');

        foreach(var defect in observation.Defects)
        {
            canonical.Append((int)defect.Kind).Append('|')
                .Append(defect.TargetDesignator.Length).Append(':')
                .Append(defect.TargetDesignator).Append('|')
                .Append(defect.Position.X.ToString("R")).Append('|')
                .Append(defect.Position.Y.ToString("R")).Append('|')
                .Append(defect.Magnitude.ToString("R")).Append('|')
                .Append(defect.Code.Length).Append(':')
                .Append(defect.Code).Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(canonical.ToString())))
            .ToLowerInvariant();
    }
}
