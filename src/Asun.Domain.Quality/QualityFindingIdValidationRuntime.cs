namespace Asun.Domain.Quality;

public static class QualityFindingIdValidationRuntime
{
    public static bool IsValid(QualityFindingId id) =>
        id.IsValid;

    public static IReadOnlyList<string> Validate(QualityFindingId id) =>
        IsValid(id)
            ? Array.Empty<string>()
            : new[] { "Quality finding ids must be non-blank." };
}
