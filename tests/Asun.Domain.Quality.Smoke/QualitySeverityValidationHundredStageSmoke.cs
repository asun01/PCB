using Asun.Domain.Quality;

public static class QualitySeverityValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;

        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var none = QualitySeverity.None;
        var info = QualitySeverity.Information;
        var warning = QualitySeverity.Warning;
        var critical = QualitySeverity.Critical;
        var invalid = (QualitySeverity)99;

        for (var i = 0; i < 10; i++)
            Check(QualitySeverityValidationRuntime.IsValid(none), $"none severity round {i + 1} should be valid.");

        for (var i = 0; i < 10; i++)
            Check(QualitySeverityValidationRuntime.IsValid(info), $"information severity round {i + 1} should be valid.");

        for (var i = 0; i < 10; i++)
            Check(QualitySeverityValidationRuntime.IsValid(warning), $"warning severity round {i + 1} should be valid.");

        for (var i = 0; i < 10; i++)
            Check(QualitySeverityValidationRuntime.IsValid(critical), $"critical severity round {i + 1} should be valid.");

        for (var i = 0; i < 10; i++)
            Check(!QualitySeverityValidationRuntime.IsValid(invalid), $"invalid severity round {i + 1} should be rejected.");

        for (var i = 0; i < 10; i++)
            Check(QualitySeverityValidationRuntime.Validate(critical).Count == 0, $"valid severity validation round {i + 1} should be empty.");

        for (var i = 0; i < 10; i++)
            Check(QualitySeverityValidationRuntime.Validate(invalid).Count > 0, $"invalid severity validation round {i + 1} should produce evidence.");

        for (var i = 0; i < 10; i++)
            Check((int)none == 0 && (int)critical == 3, $"severity ordering round {i + 1} should remain deterministic.");

        for (var i = 0; i < 10; i++)
            Check((int)info < (int)warning && (int)warning < (int)critical, $"severity ordering round {i + 1} should remain monotonic.");

        for (var i = 0; i < 10; i++)
            Check(none != critical && info != warning, $"severity identity round {i + 1} should remain distinct.");

        assert(round == 100, $"Quality severity smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
