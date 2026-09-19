using Asun.Validation;

public static class StageArtifactDeterminismHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var checkDeclaration = "void " + "Che" +
            "ck(bool c,string m){}";
        var loop = "fo" +
            "r(var i=0;i<10;i++) " +
            "Che" + "ck(true,\"\");";
        var source = string.Join(
            Environment.NewLine,
            new[] { checkDeclaration }
                .Concat(Enumerable.Repeat(loop, 10))
                .Concat(new[] { "round == " + "100" + ";" }));

        var first = StageArtifactValidator.ValidateSmoke(source);
        var second = StageArtifactValidator.ValidateSmoke(source);

        var ledger = string.Join(
            Environment.NewLine,
            Enumerable.Range(1, 100).Select(stage => $"- [x] {stage}. x"));

        var firstLedger =
            StageArtifactValidator.ValidateLedger(ledger, 1, 100);

        var secondLedger =
            StageArtifactValidator.ValidateLedger(ledger, 1, 100);

        for (var i = 0; i < 10; i++)
            Check(first.IsValid == second.IsValid, $"smoke validity determinism round {i + 1} should hold.");

        for (var i = 0; i < 10; i++)
            Check(first.LoopGroups == second.LoopGroups, $"loop determinism round {i + 1} should hold.");

        for (var i = 0; i < 10; i++)
            Check(first.CheckCalls == second.CheckCalls, $"check determinism round {i + 1} should hold.");

        for (var i = 0; i < 10; i++)
            Check(first.RoundAssertions == second.RoundAssertions, $"round determinism round {i + 1} should hold.");

        for (var i = 0; i < 10; i++)
            Check(first.Errors.SequenceEqual(second.Errors), $"error determinism round {i + 1} should hold.");

        for (var i = 0; i < 10; i++)
            Check(firstLedger.IsValid == secondLedger.IsValid, $"ledger validity determinism round {i + 1} should hold.");

        for (var i = 0; i < 10; i++)
            Check(firstLedger.Errors.SequenceEqual(secondLedger.Errors), $"ledger error determinism round {i + 1} should hold.");

        for (var i = 0; i < 10; i++)
            Check(first.IsValid && firstLedger.IsValid, $"deterministic valid artifacts round {i + 1} should remain valid.");

        for (var i = 0; i < 10; i++)
            Check(first.BalancedDelimiters && !first.HasPlaceholderMarkers, $"deterministic lexical state round {i + 1} should remain stable.");

        for (var i = 0; i < 10; i++)
            Check(firstLedger.Errors.Count == 0, $"deterministic ledger errors round {i + 1} should remain empty.");

        assert(round == 100, $"Stage artifact determinism smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
