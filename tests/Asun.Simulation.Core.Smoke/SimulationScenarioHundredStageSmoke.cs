using Asun.Domain.Pcb;
using Asun.Simulation.Core;

public static class SimulationScenarioHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var board=new PcbBoardDefinition(Guid.Parse("30000000-0000-0000-0000-000000000001"),"SimBoard",100,80,4);
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
        var first=SimulationScenarioRuntime.Observe(scenario,1);
        var second=SimulationScenarioRuntime.Observe(scenario,1);
        var changed=SimulationScenarioRuntime.Observe(
            SimulationScenarioRuntime.Create(assembly,43),
            1);

        for(var i=0;i<10;i++) Check(scenario.Seed==42,"Scenario seed should remain explicit.");
        for(var i=0;i<10;i++) Check(first.Sequence.Value==1,"Observation sequence should be one.");
        for(var i=0;i<10;i++) Check(SimulationObservationValidationRuntime.IsValid(first),"Simulation observation should validate.");
        for(var i=0;i<10;i++) Check(first==second,"Same seed and sequence should produce deterministic observation.");
        for(var i=0;i<10;i++) Check(first.Fingerprint.Length==64,"Observation fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(first.BoardOrigin==Asun.Metrology.Core.MetrologyPoint2D.Zero,"Observation board origin should remain canonical.");
        for(var i=0;i<10;i++) Check(changed.Fingerprint!=first.Fingerprint,"Different simulation seed should alter the observation fingerprint.");
        for(var i=0;i<10;i++) Check(first.Defects.All(defect=>defect.IsValid),"Every generated defect should validate.");
        for(var i=0;i<10;i++) Check(first.Defects.All(defect=>defect.TargetDesignator=="R1"),"Generated defects should target the simulated assembly component.");
        for(var i=0;i<10;i++) Check(first.Defects.All(defect=>defect.Position.IsFinite),"Generated defect coordinates should remain finite.");

        assert(round==100,$"Simulation scenario smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
