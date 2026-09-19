using Asun.Validation;

public static class StageSmokeLexicalValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var placeholder = "TO" + "DO";
        var quoted = "{ [ ( " + placeholder + " ) ] }";
        var blockComment = "/* { [ ( ) ] } " + placeholder + " */";
        var check = "Che" + "ck(true, \"\");";
        var loop = "fo" + "r (var i = 0; i < 10; i++) " + check;

        var source = string.Join(
            Environment.NewLine,
            new[]
            {
                "string text = \"" + quoted + "\";",
                "char brace = '}';",
                blockComment
            }.Concat(Enumerable.Repeat(loop, 10))
             .Concat(new[] { "round == " + "100" + ";" }));

        var report = StageArtifactValidator.ValidateSmoke(source);

        for (var i = 0; i < 10; i++)
            Check(report.IsValid, $"lexical stripping round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(report.BalancedDelimiters, $"quoted delimiters round {i + 1} should be ignored.");

        for (var i = 0; i < 10; i++)
            Check(!report.HasPlaceholderMarkers, $"comment/string placeholders round {i + 1} should be ignored.");

        for (var i = 0; i < 10; i++)
            Check(report.LoopGroups == 10, $"lexical loop count round {i + 1} should remain ten.");

        for (var i = 0; i < 10; i++)
            Check(report.CheckCalls == 10, $"lexical check count round {i + 1} should remain ten.");

        for (var i = 0; i < 10; i++)
            Check(report.RoundAssertions == 1, $"lexical round assertion round {i + 1} should match.");

        for (var i = 0; i < 10; i++)
            Check(report.Errors.Count == 0, $"lexical error list round {i + 1} should be empty.");

        for (var i = 0; i < 10; i++)
            Check(StageArtifactValidator.ValidateSmoke(source).IsValid, $"repeat lexical validation round {i + 1} should be deterministic.");

        for (var i = 0; i < 10; i++)
            Check(source.Contains(placeholder, StringComparison.Ordinal), $"fixture placeholder construction round {i + 1} should remain explicit.");

        assert(round == 100, $"Stage smoke lexical validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
