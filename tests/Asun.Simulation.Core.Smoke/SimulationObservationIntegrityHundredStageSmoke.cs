using Asun.Domain.Pcb;
using Asun.Simulation.Core;

public static class SimulationObservationIntegrityHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var board=new PcbBoardDefinition(
            Guid.Parse("30000000-0000-0000-0000-000000000001"),
            "SimBoard",
            100,
            80,
            4);
        var component=new PcbComponentReference(
            PcbFeatureId.Create("R-001"),
            "R1",
            "10k",
            "0402",
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(20,30),
            0);
        var assembly=PcbAssemblySnapshotRuntime.Create(board,new[]{component});
        var scenario=SimulationScenarioRuntime.Create(assembly,42);
        var observation=SimulationScenarioRuntime.Observe(scenario,1);
        var tampered=observation with
        {
            Fingerprint=new string('a',64)
        };

        for(var i=0;i<10;i++) Check(StableSimulationSeedRuntime.CreateSeed(scenario,1)==StableSimulationSeedRuntime.CreateSeed(scenario,1),"Stable simulation seed should be repeatable.");
        for(var i=0;i<10;i++) Check(StableSimulationSeedRuntime.CreateSeed(scenario,1)!=StableSimulationSeedRuntime.CreateSeed(scenario,2),"Different sequences should produce different stable seeds.");
        for(var i=0;i<10;i++) Check(SimulationObservationFingerprintRuntime.CreateFingerprint(scenario,observation)==observation.Fingerprint,"Observation fingerprint should be independently recomputable.");
        for(var i=0;i<10;i++) Check(SimulationObservationIntegrityRuntime.IsValid(scenario,observation),"Valid observation should pass integrity validation.");
        for(var i=0;i<10;i++) Check(!SimulationObservationIntegrityRuntime.IsValid(scenario,tampered),"Fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(SimulationObservationValidationRuntime.IsValid(observation),"Base observation validation should remain valid.");
        for(var i=0;i<10;i++) Check(observation.Sequence.Value==1,"Observation sequence should remain one.");
        for(var i=0;i<10;i++) Check(observation.Defects.All(defect=>defect.IsValid),"All generated defects should remain valid.");
        for(var i=0;i<10;i++) Check(observation.Defects.All(defect=>defect.Position.IsFinite),"All generated defect positions should remain finite.");
        for(var i=0;i<10;i++) Check(observation.Fingerprint.All(char=>Uri.IsHexDigit(char)),"Observation fingerprint should remain hexadecimal.");

        assert(round==100,$"Simulation observation integrity smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
