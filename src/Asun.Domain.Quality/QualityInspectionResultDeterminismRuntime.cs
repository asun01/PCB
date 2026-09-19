namespace Asun.Domain.Quality;

public static class QualityInspectionResultDeterminismRuntime
{
    public static string CreateContentFingerprint(
        QualityInspectionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (!QualityInspectionResultValidationRuntime.IsValid(result))
            throw new ArgumentException(
                "Inspection result is invalid.",
                nameof(result));

        return QualityInspectionDeterminismRuntime
            .CreateContentFingerprint(result.Snapshot);
    }

    public static bool AreContentEquivalent(
        QualityInspectionResult left,
        QualityInspectionResult right) =>
        string.Equals(
            CreateContentFingerprint(left),
            CreateContentFingerprint(right),
            StringComparison.Ordinal);
}
