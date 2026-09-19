using Asun.Domain.Quality;

public static class QualityFindingSetValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var first = new QualityFinding(
            QualityFindingId.Create("F-001"),
            "AOI.CLEARANCE",
            QualityOutcome.Pass,
            QualitySeverity.None,
            "Observed clearance is within the configured rule envelope.");

        var second = new QualityFinding(
            QualityFindingId.Create("F-002"),
            "SPI.VOLUME",
            QualityOutcome.Fail,
            QualitySeverity.Warning,
            "Observed paste volume requires review.");

        var set = new QualityFindingSet(new[] { first, second });
        var duplicateSet = new QualityFindingSet(new[] { first, first });
        var valid = QualityFindingSetValidationRuntime.IsValid(set);
        var duplicateValid = QualityFindingSetValidationRuntime.IsValid(duplicateSet);

        var found = set.Find(second.Id);

        for (var i = 0; i < 10; i++)
            Check(set.Count == 2, $"finding set count round {i + 1} should remain two.");

        for (var i = 0; i < 10; i++)
            Check(valid, $"finding set validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(!duplicateValid && QualityFindingSetValidationRuntime.Validate(duplicateSet).Count > 0, $"duplicate finding-id round {i + 1} should be rejected.");

        for (var i = 0; i < 10; i++)
            Check(found == second, $"finding lookup round {i + 1} should return the requested finding.");

        for (var i = 0; i < 10; i++)
            Check(set.Find(QualityFindingId.Create("MISSING")) is null, $"missing lookup round {i + 1} should be empty.");

        for (var i = 0; i < 10; i++)
            Check(set.Find(first.Id)?.Outcome == QualityOutcome.Pass, $"pass finding round {i + 1} should remain retrievable.");

        for (var i = 0; i < 10; i++)
            Check(set.Find(second.Id)?.Outcome == QualityOutcome.Fail, $"fail finding round {i + 1} should remain retrievable.");

        for (var i = 0; i < 10; i++)
            Check(set.Find(second.Id)?.Severity == QualitySeverity.Warning, $"severity lookup round {i + 1} should remain typed.");

        for (var i = 0; i < 10; i++)
            Check(set.Find(second.Id)?.Message.Contains("review", StringComparison.Ordinal) == true, $"finding message lookup round {i + 1} should remain deterministic.");

        for (var i = 0; i < 10; i++)
            Check(set.Findings.Select(item => item.Id).Distinct().Count() == set.Count, $"unique id snapshot round {i + 1} should remain stable.");

        assert(round == 100, $"Quality finding-set smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
