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

        var random=new Random(
            StableSimulationSeedRuntime.CreateSeed(
                scenario,
                sequence));
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

        var observation=new SimulationObservation(
            FrameSequence.Create(sequence),
            MetrologyPoint2D.Zero,
            defects,
            string.Empty);

        return observation with
        {
            Fingerprint=SimulationObservationFingerprintRuntime.CreateFingerprint(
                scenario,
                observation)
        };
        return new SimulationObservation(
            FrameSequence.Create(sequence),
            MetrologyPoint2D.Zero,
            defects,
            fingerprint);
    }
}
