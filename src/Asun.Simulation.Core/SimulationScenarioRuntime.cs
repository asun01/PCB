using System.Security.Cryptography;
using System.Text;
using Asun.Metrology.Core;

namespace Asun.Simulation.Core;

public static class SimulationScenarioRuntime
{
    public static SimulatedBoardScenario Create(
        Asun.Domain.Pcb.PcbAssemblySnapshot assembly,
        int seed)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        if(!Asun.Domain.Pcb.PcbAssemblySnapshotValidationRuntime.IsValid(assembly))
            throw new ArgumentException("Assembly snapshot is invalid.",nameof(assembly));

        return new SimulatedBoardScenario(assembly,seed);
    }

    public static SimulationObservation Observe(
        SimulatedBoardScenario scenario,
        long sequence)
    {
        ArgumentNullException.ThrowIfNull(scenario);

        if(sequence<=0)
            throw new ArgumentOutOfRangeException(nameof(sequence));

        var random=new Random(HashCode.Combine(scenario.Seed,sequence));
        var defects=new List<SimulatedDefect>();

        if(scenario.Assembly.Components.Count>0 &&
           random.Next(0,2)==1)
        {
            var component=scenario.Assembly.Components[
                random.Next(scenario.Assembly.Components.Count)];

            var kind=(SimulationDefectKind)(
                1+random.Next(0,4));

            defects.Add(new SimulatedDefect(
                kind,
                component.Designator,
                new MetrologyPoint2D(
                    component.Position.Xmm+(random.NextDouble()-0.5),
                    component.Position.Ymm+(random.NextDouble()-0.5)),
                Math.Round(random.NextDouble(),6),
                $"SIM-{(int)kind}-{component.Designator}"));
        }

        var canonical=new StringBuilder();
        canonical.Append(sequence).Append('|')
            .Append(scenario.Assembly.Fingerprint).Append('|')
            .Append(scenario.Seed).Append('|');

        foreach(var defect in defects)
        {
            canonical.Append((int)defect.Kind).Append('|')
                .Append(defect.TargetDesignator).Append('|')
                .Append(defect.Position.X.ToString("R")).Append('|')
                .Append(defect.Position.Y.ToString("R")).Append('|')
                .Append(defect.Magnitude.ToString("R")).Append('|')
                .Append(defect.Code).Append('|');
        }

        var fingerprint=Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(canonical.ToString())))
            .ToLowerInvariant();

        return new SimulationObservation(
            FrameSequence.Create(sequence),
            MetrologyPoint2D.Zero,
            defects,
            fingerprint);
    }
}
