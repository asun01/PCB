using Asun.Validation;

public static class StageSmokeFailureValidationHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var missingRound="""
void Check(bool c,string m){}
for (var i=0;i<10;i++) Check(true, "");
for (var i=0;i<10;i++) Check(true, "");
for (var i=0;i<10;i++) Check(true, "");
for (var i=0;i<10;i++) Check(true, "");
for (var i=0;i<10;i++) Check(true, "");
for (var i=0;i<10;i++) Check(true, "");
for (var i=0;i<10;i++) Check(true, "");
for (var i=0;i<10;i++) Check(true, "");
for (var i=0;i<10;i++) Check(true, "");
for (var i=0;i<10;i++) Check(true, "");
""";
        var wrongLoops=missingRound+"""
round == 90;
""";
        var missingReport=StageArtifactValidator.ValidateSmoke(missingRound);
        var wrongReport=StageArtifactValidator.ValidateSmoke(wrongLoops);

        for(var i=0;i<10;i++) Check(!missingReport.IsValid,$"missing round failure round {i+1} should be detected.");
        for(var i=0;i<10;i++) Check(missingReport.RoundAssertions==0,$"missing round assertion count round {i+1} should be zero.");
        for(var i=0;i<10;i++) Check(missingReport.Errors.Count>0,$"missing round diagnostics round {i+1} should be non-empty.");
        for(var i=0;i<10;i++) Check(wrongReport.RoundAssertions==0,$"wrong round token round {i+1} should not match expected 100.");
        for(var i=0;i<10;i++) Check(!wrongReport.IsValid,$"wrong round validation round {i+1} should fail.");
        for(var i=0;i<10;i++) Check(wrongReport.LoopGroups==10,$"loop preservation round {i+1} should remain ten.");
        for(var i=0;i<10;i++) Check(wrongReport.CheckCalls==10,$"check-site preservation round {i+1} should remain ten.");
        for(var i=0;i<10;i++) Check(!wrongReport.BalancedDelimiters?false:true,$"balanced source round {i+1} should remain balanced.");
        for(var i=0;i<10;i++) Check(!wrongReport.HasPlaceholderMarkers,$"placeholder state round {i+1} should remain clean.");
        for(var i=0;i<10;i++) Check(wrongReport.Errors.Any(error=>error.Contains("round == 100",StringComparison.Ordinal)),$"round mismatch diagnostic round {i+1} should be explicit.");

        assert(round==100,$"Stage smoke failure validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
