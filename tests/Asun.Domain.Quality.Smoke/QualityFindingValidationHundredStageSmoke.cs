using Asun.Domain.Quality;

public static class QualityFindingValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var finding = new QualityFinding(
            QualityFindingId.Create("F-001"),
            "SPI.VOLUME",
            QualityOutcome.Fail,
            QualitySeverity.Warning,
            "Paste volume is below the configured observation threshold.");

        var unknownOutcome = finding with { Outcome = QualityOutcome.Unknown };
        var blankRule = finding with { RuleCode = " " };
        var critical = finding with { Severity = QualitySeverity.Critical };
        var validation = QualityFindingValidationRuntime.Validate(finding);

        for (var i = 0; i < 10; i++)
            Check(finding.IsValid, $"finding validity round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(validation.Count == 0, $"finding validation round {i + 1} should pass.");

        for (var i = 0; i < 10; i++)
            Check(finding.Id.Value == "F-001", $"finding identity round {i + 1} should remain stable.");

        for (var i = 0; i < 10; i++)
            Check(finding.RuleCode == "SPI.VOLUME", $"rule code round {i + 1} should remain explicit.");

        for (var i = 0; i < 10; i++)
            Check(finding.Outcome == QualityOutcome.Fail && finding.Severity == QualitySeverity.Warning, $"finding disposition metadata round {i + 1} should remain typed.");

        for (var i = 0; i < 10; i++)
            Check(!unknownOutcome.IsValid && QualityFindingValidationRuntime.Validate(unknownOutcome).Count > 0, $"unknown outcome finding round {i + 1} should be rejected.");

        for (var i = 0; i < 10; i++)
            Check(!blankRule.IsValid && QualityFindingValidationRuntime.Validate(blankRule).Count > 0, $"blank rule finding round {i + 1} should be rejected.");

        for (var i = 0; i < 10; i++)
            Check(critical.IsValid && critical.Severity == QualitySeverity.Critical, $"critical severity finding round {i + 1} should remain valid.");

        for (var i = 0; i < 10; i++)
            Check(!string.IsNullOrWhiteSpace(finding.Message), $"finding message round {i + 1} should remain audit-readable.");

        for (var i = 0; i < 10; i++)
            Check(QualityOutcomeValidationRuntime.IsValid(finding.Outcome) && QualitySeverityValidationRuntime.IsValid(finding.Severity), $"supporting enum validation round {i + 1} should pass.");

        assert(round == 100, $"Quality finding smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
