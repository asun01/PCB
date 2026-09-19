namespace Asun.Domain.Quality;

public static class QualityOutcomeValidationRuntime
{
    public static bool IsValid(QualityOutcome outcome) =>
        outcome is >= QualityOutcome.Pass and <= QualityOutcome.Informational;

    public static IReadOnlyList<string> Validate(QualityOutcome outcome) =>
        IsValid(outcome)
            ? Array.Empty<string>()
            : new[] { "Quality outcome is not a supported concrete state." };
}
