using Asun.Domain.Pcb;

public static class PcbComponentCollectionHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var board=new PcbBoardDefinition(Guid.NewGuid(),"CollectionBoard",200,100,6);
        var c2=new PcbComponentReference(PcbFeatureId.Create("R-002"),"R2","10k","0603",0,PcbLayerSide.Top,new PcbCoordinate(40,50),0);
        var c1=new PcbComponentReference(PcbFeatureId.Create("R-001"),"R1","1k","0603",1,PcbLayerSide.Bottom,new PcbCoordinate(20,30),1);
        var ordered=PcbComponentCollectionRuntime.Create(board,new[]{c2,c1});
        var duplicate=new[]{c1,c1};

        for(var i=0;i<10;i++) Check(ordered.Count==2,"Component collection should preserve two components.");
        for(var i=0;i<10;i++) Check(ordered[0].Designator=="R1","Collection should canonicalize R1 before R2.");
        for(var i=0;i<10;i++) Check(ordered[1].Designator=="R2","Collection should retain R2 after R1.");
        for(var i=0;i<10;i++) Check(PcbComponentCollectionRuntime.Create(board,ordered).SequenceEqual(ordered),"Collection creation should be deterministic.");
        for(var i=0;i<10;i++) Check(PcbComponentReferenceValidationRuntime.IsValid(ordered[0],board),"First component should validate.");
        for(var i=0;i<10;i++) Check(PcbComponentReferenceValidationRuntime.IsValid(ordered[1],board),"Second component should validate.");
        for(var i=0;i<10;i++) Check(ordered.Select(component=>component.Designator).Distinct(StringComparer.Ordinal).Count()==2,"Component designators should remain unique.");
        for(var i=0;i<10;i++) Check(c1.Side==PcbLayerSide.Bottom,"R1 side should remain Bottom.");
        for(var i=0;i<10;i++) Check(c2.Side==PcbLayerSide.Top,"R2 side should remain Top.");
        for(var i=0;i<10;i++) Check(ordered[0].LayerIndex==1 && ordered[1].LayerIndex==0,"Layer indexes should remain attached to components.");
        for(var i=0;i<10;i++)
        {
            var threw=false;
            try{PcbComponentCollectionRuntime.Create(board,duplicate);}
            catch(ArgumentException){threw=true;}
            Check(threw,"Duplicate designator should be rejected.");
        }

        assert(round==100,$"PCB component collection smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
