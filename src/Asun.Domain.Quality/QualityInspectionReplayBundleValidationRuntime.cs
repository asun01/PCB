namespace Asun.Domain.Quality;

public static class QualityInspectionReplayBundleValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionReplayBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(bundle);

        var errors = new List<string>();

        if (bundle.Current is null)
        {
            errors.Add("Replay bundle current projection cannot be null.");
            return errors;
        }

        if (bundle.Diff is null)
        {
            errors.Add("Replay bundle diff cannot be null.");
            return errors;
        }

        errors.AddRange(
            QualityInspectionReplayProjectionValidationRuntime.Validate(
                bundle.Current));

        if (bundle.Previous is not null)
        {
            errors.AddRange(
                QualityInspectionReplayProjectionValidationRuntime.Validate(
                    bundle.Previous));

            if (errors.Count == 0)
            {
                var expected = QualityInspectionReplayProjectionDiffRuntime.Diff(
                    bundle.Previous,
                    bundle.Current);

                if (!expected.Equals(bundle.Diff))
                    errors.Add(
                        "Replay bundle diff does not match its projection pair.");
            }
        }
        else if (!bundle.Diff.IsEmpty)
        {
            errors.Add(
                "A replay bundle without a previous projection must have an empty diff.");
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionReplayBundle bundle) =>
        Validate(bundle).Count == 0;
}
