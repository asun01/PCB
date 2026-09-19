using Asun.Domain.Pcb;
using Asun.Simulation.Core;

public static class SimulationSessionHundredStageSmoke
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
        var observations=SimulationSessionRuntime.Run(
            SimulationScenarioRuntime.Create(assembly,7),
            5);
        var invalid=new List<SimulationObservation>(observations)
        {
            observations[4] with {Sequence=FrameSequence.Create(7)}
        };

        for(var i=0;i<10;i++) Check(observations.Count==5,"Simulation session should contain five observations.");
        for(var i=0;i<10;i++) Check(observations[0].Sequence.Value==1 && observations[4].Sequence.Value==5,"Simulation session should cover a contiguous sequence.");
        for(var i=0;i<10;i++) Check(SimulationSessionValidationRuntime.IsValid(observations),"Simulation session should validate.");
        for(var i=0;i<10;i++) Check(!SimulationSessionValidationRuntime.IsValid(invalid),"Non-contiguous sequence should be rejected.");
        for(var i=0;i<10;i++) Check(observations.Select(item=>item.Fingerprint).Distinct().Count()==5,"Each simulation sequence should produce a distinct fingerprint.");
        for(var i=0;i<10;i++) Check(observations.All(item=>item.Sequence.IsValid),"Every observation sequence should be valid.");
        for(var i=0;i<10;i++) Check(observations.All(item=>item.Defects.All(defect=>defect.IsValid)),"Every simulated defect should validate.");
        for(var i=0;i<10;i++) Check(observations.All(item=>item.BoardOrigin.IsFinite),"Every observation origin should be finite.");
        for(var i=0;i<10;i++) Check(SimulationSessionRuntime.Run(SimulationScenarioRuntime.Create(assembly,7),5).SequenceEqual(observations),"Same scenario should replay deterministically.");
        for(var i=0;i<10;i++) Check(SimulationSessionRuntime.Run(SimulationScenarioRuntime.Create(assembly,8),5).Select(item=>item.Fingerprint).SequenceEqual(observations.Select(item=>item.Fingerprint))==false,"Different seed should alter the simulation session.");

        assert(round==100,$"Simulation session smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
