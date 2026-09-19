using Asun.Domain.Quality;

public static class QualityFindingEvidenceIndexValidationHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){ round++; assert(condition,$"Round {round}: {message}"); }

        var f1=QualityFindingId.Create("F-001");
        var f2=QualityFindingId.Create("F-002");
        var e1=QualityEvidenceKey.Create("frame://001");
        var e2=QualityEvidenceKey.Create("frame://002");
        var set=new QualityFindingEvidenceSet(new[]{
            new QualityFindingEvidenceLink(f1,e1),
            new QualityFindingEvidenceLink(f1,e2),
            new QualityFindingEvidenceLink(f2,e1)
        });
        var index=new QualityFindingEvidenceIndex(set);

        for(var i=0;i<10;i++) Check(index.EvidenceFor(f1).Count==2,$"finding index round {i+1} should expose two evidence keys.");
        for(var i=0;i<10;i++) Check(index.EvidenceFor(f1).Contains(e1) && index.EvidenceFor(f1).Contains(e2),$"finding index membership round {i+1} should preserve both keys.");
        for(var i=0;i<10;i++) Check(index.EvidenceFor(f2).Single()==e1,$"second finding index round {i+1} should expose one key.");
        for(var i=0;i<10;i++) Check(index.EvidenceFor(QualityFindingId.Create("MISSING")).Count==0,$"missing finding index round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(index.FindingsFor(e1).Count==2,$"shared evidence index round {i+1} should link two findings.");
        for(var i=0;i<10;i++) Check(index.FindingsFor(e1).Contains(f1) && index.FindingsFor(e1).Contains(f2),$"shared evidence membership round {i+1} should retain both findings.");
        for(var i=0;i<10;i++) Check(index.FindingsFor(e2).Single()==f1,$"unique evidence index round {i+1} should resolve to one finding.");
        for(var i=0;i<10;i++) Check(index.FindingsFor(QualityEvidenceKey.Create("MISSING")).Count==0,$"missing evidence index round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(QualityFindingEvidenceValidationRuntime.IsValid(set),$"indexed set validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(index.EvidenceFor(f1).Distinct().Count()==2 && index.FindingsFor(e1).Distinct().Count()==2,$"index deduplication round {i+1} should remain deterministic.");

        assert(round==100,$"Quality finding-evidence index smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
