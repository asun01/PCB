using Asun.Domain.Pcb;

public static class PcbAssemblySnapshotHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var board=new PcbBoardDefinition(Guid.NewGuid(),"AssemblyBoard",120,80,4);
        var c2=new PcbComponentReference(PcbFeatureId.Create("U-002"),"U2","MCU","QFN",0,PcbLayerSide.Top,new PcbCoordinate(80,40),0);
        var c1=new PcbComponentReference(PcbFeatureId.Create("R-001"),"R1","10k","0402",0,PcbLayerSide.Top,new PcbCoordinate(20,20),0);
        var snapshot=PcbAssemblySnapshotRuntime.Create(board,new[]{c2,c1});
        var invalid=snapshot with {Fingerprint=new string('a',64)};

        for(var i=0;i<10;i++) Check(snapshot.Components.Count==2,"Assembly snapshot should preserve two components.");
        for(var i=0;i<10;i++) Check(snapshot.Components[0].Designator=="R1","Assembly snapshot should canonicalize R1 first.");
        for(var i=0;i<10;i++) Check(snapshot.Statistics.ComponentCount==2,"Assembly statistics should preserve component count.");
        for(var i=0;i<10;i++) Check(snapshot.Statistics.TopCount==2,"Assembly statistics should preserve top count.");
        for(var i=0;i<10;i++) Check(snapshot.Fingerprint.Length==64,"Assembly fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(PcbAssemblySnapshotValidationRuntime.IsValid(snapshot),"Assembly snapshot should validate.");
        for(var i=0;i<10;i++) Check(!PcbAssemblySnapshotValidationRuntime.IsValid(invalid),"Tampered fingerprint should fail.");
        for(var i=0;i<10;i++) Check(PcbAssemblySnapshotRuntime.Create(board,new[]{c1,c2})==snapshot,"Assembly snapshot creation should be deterministic.");
        for(var i=0;i<10;i++) Check(snapshot.Board==board,"Assembly snapshot should retain board identity.");
        for(var i=0;i<10;i++) Check(snapshot.Statistics.BottomCount==0,"Assembly statistics should retain zero bottom components.");

        assert(round==100,$"PCB assembly snapshot smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
