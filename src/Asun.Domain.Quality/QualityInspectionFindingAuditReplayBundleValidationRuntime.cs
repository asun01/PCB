namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditReplayBundleValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult currentResult,
        QualityInspectionFindingAuditReplayBundle bundle,
        QualityInspectionResult? previousResult = null)
    {
        ArgumentNullException.ThrowIfNull(currentResult);
        ArgumentNullException.ThrowIfNull(bundle);

        var errors=new List<string>();

        if(bundle.Current is null)
        {
            errors.Add("Finding audit replay bundle current projection cannot be null.");
            return errors;
        }

        if(bundle.Diff is null)
        {
            errors.Add("Finding audit replay bundle diff cannot be null.");
            return errors;
        }

        errors.AddRange(
            QualityInspectionFindingAuditProjectionValidationRuntime
                .Validate(currentResult,bundle.Current));

        if(previousResult is null)
        {
            if(bundle.Previous is not null)
                errors.Add("Initial finding audit replay bundle cannot contain a previous projection.");
            if(!bundle.Diff.IsEmpty)
                errors.Add("Initial finding audit replay bundle must have an empty diff.");
        }
        else
        {
            if(bundle.Previous is null)
            {
                errors.Add("Non-initial finding audit replay bundle requires a previous projection.");
            }
            else
            {
                errors.AddRange(
                    QualityInspectionFindingAuditProjectionValidationRuntime
                        .Validate(previousResult,bundle.Previous));

                var expected=
                    QualityInspectionFindingAuditProjectionDiffRuntime.Diff(
                        bundle.Previous,
                        bundle.Current);

                if(!expected.Equals(bundle.Diff))
                    errors.Add(
                        "Finding audit replay bundle diff does not match its projections.");
            }
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult currentResult,
        QualityInspectionFindingAuditReplayBundle bundle,
        QualityInspectionResult? previousResult = null) =>
        Validate(currentResult,bundle,previousResult).Count==0;
}
