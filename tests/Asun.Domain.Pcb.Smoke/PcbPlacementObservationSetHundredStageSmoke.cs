using Asun.Domain.Pcb;
using Asun.Metrology.Core;

public static class PcbPlacementObservationSetHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        static PcbComponentReference Component(
            string id,
            string designator,
            double x,
            double y)=>
            new(
                PcbFeatureId.Create(id),
                designator,
                "10k",
                "0402",
                0,
                PcbLayerSide.Top,
                new PcbCoordinate(x,y),
                0);

        var first=Component("R-001","R1",10,10);
        var second=Component("R-002","R2",20,20);
        var observations=new[]{
            PcbPlacementObservationRuntime.Measure(
                first,
                new MetrologyPoint2D(10.1,10.1)),
            PcbPlacementObservationRuntime.Measure(
                second,
                new MetrologyPoint2D(19.8,20.1))
        };
        var set=PcbPlacementObservationSetRuntime.Create(
            new[]{observations[1],observations[0]});

        for(var i=0;i<10;i++) Check(set.Observations.Count==2,"Placement observation set should retain two observations.");
        for(var i=0;i<10;i++) Check(set.Observations[0].Designator=="R1","Observation set should canonicalize R1 first.");
        for(var i=0;i<10;i++) Check(set.Observations[1].Designator=="R2","Observation set should preserve R2 second.");
        for(var i=0;i<10;i++) Check(Math.Abs(set.Observations[0].ErrorDistance-Math.Sqrt(0.02))<1e-12,"R1 error should match geometry.");
        for(var i=0;i<10;i++) Check(Math.Abs(set.Observations[1].ErrorDistance-Math.Sqrt(0.05))<1e-12,"R2 error should match geometry.");
        for(var i=0;i<10;i++) Check(Math.Abs(set.MaximumError-Math.Sqrt(0.05))<1e-12,"Maximum error should match the largest observation.");
        for(var i=0;i<10;i++) Check(set.RootMeanSquareError>0,"RMS placement error should be positive.");
        for(var i=0;i<10;i++) Check(set.Observations.All(PcbPlacementObservationValidationRuntime.IsValid),"Every placement observation should validate.");
        for(var i=0;i<10;i++) Check(PcbPlacementObservationSetRuntime.Create(set.Observations).Equals(set),"Observation set creation should be deterministic.");
        for(var i=0;i<10;i++) Check(set.Observations.Select(item=>item.ComponentId).Distinct().Count()==2,"Placement component identities should remain unique.");

        assert(round==100,$"PCB placement observation set smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
