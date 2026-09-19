using Asun.Validation;

public static class StageLedgerValidationHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var lines=Enumerable.Range(4501,100).Select(stage=>$"- [x] {stage}. stage").ToArray();
        var valid=string.Join(Environment.NewLine,lines);
        var duplicate=valid.Replace("- [x] 4550.","- [x] 4549.",StringComparison.Ordinal);
        var gap=valid.Replace("- [x] 4550.","- [x] 4600.",StringComparison.Ordinal);
        var shortLedger=string.Join(Environment.NewLine,lines.Take(99));

        var validReport=StageArtifactValidator.ValidateLedger(valid,4501,4600);
        var duplicateReport=StageArtifactValidator.ValidateLedger(duplicate,4501,4600);
        var gapReport=StageArtifactValidator.ValidateLedger(gap,4501,4600);
        var shortReport=StageArtifactValidator.ValidateLedger(shortLedger,4501,4600);

        for(var i=0;i<10;i++) Check(validReport.IsValid,$"valid ledger round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(validReport.Errors.Count==0,$"valid ledger errors round {i+1} should be empty.");
        for(var i=0;i<10;i++) Check(validReport.LoopGroups==0,$"ledger loop field round {i+1} should remain zero.");
        for(var i=0;i<10;i++) Check(!duplicateReport.IsValid,$"duplicate ledger round {i+1} should fail.");
        for(var i=0;i<10;i++) Check(duplicateReport.Errors.Any(error=>error.Contains("duplicate",StringComparison.OrdinalIgnoreCase)),$"duplicate diagnostics round {i+1} should be explicit.");
        for(var i=0;i<10;i++) Check(!gapReport.IsValid,$"gap ledger round {i+1} should fail.");
        for(var i=0;i<10;i++) Check(gapReport.Errors.Any(error=>error.Contains("expected stage",StringComparison.OrdinalIgnoreCase)),$"gap diagnostics round {i+1} should be explicit.");
        for(var i=0;i<10;i++) Check(!shortReport.IsValid,$"short ledger round {i+1} should fail.");
        for(var i=0;i<10;i++) Check(shortReport.Errors.Count>0,$"short ledger diagnostics round {i+1} should be non-empty.");
        for(var i=0;i<10;i++) Check(validReport.IsValid,$"final ledger state round {i+1} should remain valid.");

        assert(round==100,$"Stage ledger validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
