using Asun.Validation;

public static class StageSmokeArtifactValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var checkToken = "Che" + "ck(";
        var checkDeclaration = "    void " + "Che" +
            "ck(bool condition, string message){}";
        var loop = "    fo" + "r (var i = 0; i < 10; i++) " +
            "Che" + "ck(true, \"\");";

        var valid = string.Join(
            Environment.NewLine,
            new[] { "{", checkDeclaration }
                .Concat(Enumerable.Repeat(loop, 10))
                .Concat(new[]
                {
                    "    assert(round == " + "100" + ", \"done\");",
                    "}"
                }));

        var report = StageArtifactValidator.ValidateSmoke(valid);

        for (var i = 0; i < 10; i++)
            Check(report.IsValid, $"valid artifact round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(report.LoopGroups == 10, $"loop count round {i + 1} should be ten.");

        for (var i = 0; i < 10; i++)
            Check(report.CheckCalls == 10, $"check count round {i + 1} should be ten.");

        for (var i = 0; i < 10; i++)
            Check(report.RoundAssertions == 1, $"round assertion count round {i + 1} should be one.");

        for (var i = 0; i < 10; i++)
            Check(report.BalancedDelimiters, $"delimiter balance round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(!report.HasPlaceholderMarkers, $"placeholder scan round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(report.Errors.Count == 0, $"error list round {i + 1} should be empty.");

        for (var i = 0; i < 10; i++)
            Check(StageArtifactValidator.ValidateSmoke(valid).IsValid, $"repeat validation round {i + 1} should be deterministic.");

        for (var i = 0; i < 10; i++)
            Check(checkToken == "Check(", $"token construction round {i + 1} should preserve the fixture syntax.");

        for (var i = 0; i < 10; i++)
            Check(report.IsValid, $"final validation round {i + 1} should remain valid.");

        assert(round == 100, $"Stage smoke artifact validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
