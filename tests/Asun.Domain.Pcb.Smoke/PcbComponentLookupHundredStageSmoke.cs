using Asun.Domain.Pcb;

public static class PcbComponentLookupHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var c1=new PcbComponentReference(PcbFeatureId.Create("C-001"),"C1","100nF","0402",0,PcbLayerSide.Top,new PcbCoordinate(10,20),0);
        var c2=new PcbComponentReference(PcbFeatureId.Create("C-002"),"C2","1uF","0603",0,PcbLayerSide.Top,new PcbCoordinate(30,40),0);
        var r1=new PcbComponentReference(PcbFeatureId.Create("R-001"),"R1","10k","0402",1,PcbLayerSide.Bottom,new PcbCoordinate(50,60),0);
        var components=new[]{r1,c2,c1};

        for(var i=0;i<10;i++) Check(PcbComponentPlacementLookupRuntime.FindByDesignator(components,"C1")==c1,"Designator lookup should find C1.");
        for(var i=0;i<10;i++) Check(PcbComponentPlacementLookupRuntime.FindByDesignator(components,"missing") is null,"Missing designator should return null.");
        for(var i=0;i<10;i++) Check(PcbComponentPlacementLookupRuntime.FindByDesignator(components," C2 ")==c2,"Lookup should trim the designator.");
        for(var i=0;i<10;i++) Check(PcbComponentPlacementLookupRuntime.FindByPackage(components,"0402").Count==2,"Package lookup should return two 0402 components.");
        for(var i=0;i<10;i++) Check(PcbComponentPlacementLookupRuntime.FindByPackage(components,"0402")[0].Designator=="C1","Package results should be canonically ordered.");
        for(var i=0;i<10;i++) Check(PcbComponentPlacementLookupRuntime.FindByPackage(components,"0402")[1].Designator=="R1","Package results should retain R1 second.");
        for(var i=0;i<10;i++) Check(PcbComponentPlacementLookupRuntime.FindByPackage(components,"0603").Single()==c2,"0603 lookup should return C2.");
        for(var i=0;i<10;i++) Check(PcbComponentPlacementLookupRuntime.FindByPackage(components,"missing").Count==0,"Unknown package should return empty.");
        for(var i=0;i<10;i++) Check(PcbComponentPlacementLookupRuntime.FindByDesignator(components,"R1")==r1,"Designator lookup should find R1.");
        for(var i=0;i<10;i++) Check(components.All(component=>component.IsValid()),"Lookup source components should remain valid.");

        assert(round==100,$"PCB component lookup smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
