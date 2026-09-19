using Asun.Domain.Quality;

public static class QualityFindingEvidenceSetValidationHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){ round++; assert(condition,$"Round {round}: {message}"); }

        var firstFinding=QualityFindingId.Create("F-001");
        var secondFinding=QualityFindingId.Create("F-002");
        var first= new QualityFindingEvidenceLink(firstFinding,QualityEvidenceKey.Create("frame://001"));
        var second= new QualityFindingEvidenceLink(secondFinding,QualityEvidenceKey.Create("frame://002"));
        var shared= new QualityFindingEvidenceLink(firstFinding,QualityEvidenceKey.Create("trace://001"));
        var set=new QualityFindingEvidenceSet(new[]{first,second,shared});
        var duplicate=new QualityFindingEvidenceSet(new[]{first,first});

        for(var i=0;i<10;i++) Check(set.Count==3,$"set count round {i+1} should remain three.");
        for(var i=0;i<10;i++) Check(QualityFindingEvidenceValidationRuntime.IsValid(set),$"set validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(!QualityFindingEvidenceValidationRuntime.IsValid(duplicate),$"duplicate link set round {i+1} should fail.");
        for(var i=0;i<10;i++) Check(set.ForFinding(firstFinding).Count==2,$"finding evidence count round {i+1} should be two.");
        for(var i=0;i<10;i++) Check(set.ForFinding(secondFinding).Single()==second.EvidenceKey,$"second finding evidence round {i+1} should resolve exactly.");
        for(var i=0;i<10;i++) Check(set.ForFinding(QualityFindingId.Create("MISSING")).Count==0,$"missing finding evidence round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(set.Links.Select(link=>link.EvidenceKey).Distinct().Count()==3,$"evidence key uniqueness round {i+1} should remain deterministic.");
        for(var i=0;i<10;i++) Check(set.Links.All(link=>link.IsValid),$"member link state round {i+1} should remain valid.");
        for(var i=0;i<10;i++) Check(set.Links is IReadOnlyList<QualityFindingEvidenceLink>,$"immutable exposure round {i+1} should remain read-only.");
        for(var i=0;i<10;i++) Check(set.ForFinding(firstFinding).Contains(first.EvidenceKey) && set.ForFinding(firstFinding).Contains(shared.EvidenceKey),$"multi-evidence lookup round {i+1} should retain both links.");

        assert(round==100,$"Quality finding-evidence set smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
