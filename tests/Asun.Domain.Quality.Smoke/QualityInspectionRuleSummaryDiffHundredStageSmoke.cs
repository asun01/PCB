using Asun.Domain.Quality;

public static class QualityInspectionRuleSummaryDiffHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var previous=new QualityInspectionRuleSummary(new[]{
            new QualityInspectionRuleSummaryEntry("RULE.A",2,1),
            new QualityInspectionRuleSummaryEntry("RULE.B",1,1)
        });
        var current=new QualityInspectionRuleSummary(new[]{
            new QualityInspectionRuleSummaryEntry("RULE.A",3,2),
            new QualityInspectionRuleSummaryEntry("RULE.C",1,1)
        });

        var same=QualityInspectionRuleSummaryDiffRuntime.Diff(previous,previous);
        var diff=QualityInspectionRuleSummaryDiffRuntime.Diff(previous,current);
        var invalid=new QualityInspectionRuleSummaryDiff(
            new[]{"RULE.A"},
            new[]{"RULE.A"},
            Array.Empty<string>());

        for(var i=0;i<10;i++) Check(same.IsEmpty,$"rule self diff round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(diff.AddedRuleCodes.Single()=="RULE.C",$"rule added code round {i+1} should be RULE.C.");
        for(var i=0;i<10;i++) Check(diff.RemovedRuleCodes.Single()=="RULE.B",$"rule removed code round {i+1} should be RULE.B.");
        for(var i=0;i<10;i++) Check(diff.ChangedRuleCodes.Single()=="RULE.A",$"rule changed code round {i+1} should be RULE.A.");
        for(var i=0;i<10;i++) Check(QualityInspectionRuleSummaryDiffValidationRuntime.IsValid(diff),$"rule diff validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(!QualityInspectionRuleSummaryDiffValidationRuntime.IsValid(invalid),$"rule diff overlap round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(diff.AddedRuleCodes.SequenceEqual(diff.AddedRuleCodes.OrderBy(code=>code,StringComparer.Ordinal)),$"rule added ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(diff.RemovedRuleCodes.SequenceEqual(diff.RemovedRuleCodes.OrderBy(code=>code,StringComparer.Ordinal)),$"rule removed ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(diff.ChangedRuleCodes.SequenceEqual(diff.ChangedRuleCodes.OrderBy(code=>code,StringComparer.Ordinal)),$"rule changed ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(QualityInspectionRuleSummaryDiffRuntime.Diff(previous,current).Equals(diff),$"rule diff determinism round {i+1} should remain stable.");

        assert(round==100,$"Quality inspection rule summary diff smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
