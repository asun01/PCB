using System.Security.Cryptography;
using System.Text;

namespace Asun.Simulation.Core;

public static class StableSimulationSeedRuntime
{
    public static int CreateSeed(
        SimulatedBoardScenario scenario,
        long sequence)
    {
        ArgumentNullException.ThrowIfNull(scenario);

        if(sequence<=0)
            throw new ArgumentOutOfRangeException(nameof(sequence));

        var canonical=$"{scenario.Seed}|{sequence}|{scenario.Assembly.Fingerprint}";
        var hash=SHA256.HashData(
            Encoding.UTF8.GetBytes(canonical));

        var value=BitConverter.ToInt32(hash,0);
        return value;
    }
}
