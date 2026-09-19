using Asun.Domain.Pcb;

public static class PcbComponentReferenceValidationHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var board=new PcbBoardDefinition(Guid.NewGuid(),"ComponentBoard",100,80,4);
        var component=new PcbComponentReference(
            PcbFeatureId.Create("C-001"),
            "C1",
            "100nF",
            "0402",
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(20,30),
            0.25);
        var invalid=component with {Designator=""};
        var invalidLayer=component with {LayerIndex=4};

        for(var i=0;i<10;i++) Check(component.IsValid(board),"Valid component should remain valid.");
        for(var i=0;i<10;i++) Check(PcbComponentReferenceValidationRuntime.IsValid(component,board),"Component validation should pass.");
        for(var i=0;i<10;i++) Check(!PcbComponentReferenceValidationRuntime.IsValid(invalid,board),"Blank designator should fail.");
        for(var i=0;i<10;i++) Check(!PcbComponentReferenceValidationRuntime.IsValid(invalidLayer,board),"Out-of-range layer should fail.");
        for(var i=0;i<10;i++) Check(component.Position.IsFinite,"Component position should remain finite.");
        for(var i=0;i<10;i++) Check(component.LayerIndex==0,"Component layer should remain zero.");
        for(var i=0;i<10;i++) Check(component.Side==PcbLayerSide.Top,"Component side should remain Top.");
        for(var i=0;i<10;i++) Check(component.PackageName=="0402","Package identity should remain stable.");
        for(var i=0;i<10;i++) Check(component.Designator=="C1","Designator should remain stable.");
        for(var i=0;i<10;i++) Check(component.Value=="100nF","Component value should remain stable.");

        assert(round==100,$"PCB component reference smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
