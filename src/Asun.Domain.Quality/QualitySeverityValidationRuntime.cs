namespace Asun.Domain.Quality;

public static class QualitySeverityValidationRuntime
{
    public static bool IsValid(QualitySeverity severity) =>
        severity is >= QualitySeverity.None and <= QualitySeverity.Critical;

    public static IReadOnlyList<string> Validate(QualitySeverity severity) =>
        IsValid(severity)
            ? Array.Empty<string>()
            : new[] { "Quality severity is outside the supported range." };
}
