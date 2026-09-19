using Asun.Domain.Pcb;
using Asun.Metrology.Core;

public static class PcbPlacementObservationHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var component=new PcbComponentReference(
            PcbFeatureId.Create("R-001"),
            "R1",
            "10k",
            "0402",
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(20,30),
            0);
        var observation=PcbPlacementObservationRuntime.Measure(
            component,
            new MetrologyPoint2D(20.3,29.6));
        var invalid=observation with
        {
            ErrorDistance=0
        };

        for(var i=0;i<10;i++) Check(observation.ComponentId==component.Id,"Observation should retain component identity.");
        for(var i=0;i<10;i++) Check(observation.Designator=="R1","Observation should retain designator.");
        for(var i=0;i<10;i++) Check(observation.ExpectedPosition==new MetrologyPoint2D(20,30),"Expected position should reflect component placement.");
        for(var i=0;i<10;i++) Check(observation.MeasuredPosition==new MetrologyPoint2D(20.3,29.6),"Measured position should remain explicit.");
        for(var i=0;i<10;i++) Check(observation.Delta==new MetrologyPoint2D(0.3,-0.4),"Placement delta should be measured minus expected.");
        for(var i=0;i<10;i++) Check(Math.Abs(observation.ErrorDistance-0.5)<1e-12,"Placement error distance should be 0.5.");
        for(var i=0;i<10;i++) Check(PcbPlacementObservationValidationRuntime.IsValid(observation),"Placement observation should validate.");
        for(var i=0;i<10;i++) Check(!PcbPlacementObservationValidationRuntime.IsValid(invalid),"Tampered error distance should be rejected.");
        for(var i=0;i<10;i++) Check(observation.ExpectedPosition.IsFinite && observation.MeasuredPosition.IsFinite,"Placement coordinates should remain finite.");
        for(var i=0;i<10;i++) Check(component.IsValid(),"Source component should remain valid.");

        assert(round==100,$"PCB placement observation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
