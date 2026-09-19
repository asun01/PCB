namespace Asun.Domain.Quality;

public static class QualityInspectionAuditReplayBundleValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult currentResult,
        QualityInspectionAuditReplayBundle bundle,
        QualityInspectionResult? previousResult = null)
    {
        ArgumentNullException.ThrowIfNull(currentResult);
        ArgumentNullException.ThrowIfNull(bundle);

        var errors=new List<string>();

        if(bundle.Current is null)
        {
            errors.Add("Audit replay bundle current projection cannot be null.");
            return errors;
        }

        if(bundle.Diff is null)
        {
            errors.Add("Audit replay bundle diff cannot be null.");
            return errors;
        }

        errors.AddRange(
            QualityInspectionAuditProjectionValidationRuntime
                .Validate(currentResult,bundle.Current));

        if(previousResult is null)
        {
            if(bundle.Previous is not null)
                errors.Add("Initial audit replay bundle cannot contain a previous projection.");

            if(!bundle.Diff.IsEmpty)
                errors.Add("Initial audit replay bundle must have an empty diff.");
        }
        else if(bundle.Previous is null)
        {
            errors.Add("Non-initial audit replay bundle requires a previous projection.");
        }
        else
        {
            errors.AddRange(
                QualityInspectionAuditProjectionValidationRuntime
                    .Validate(previousResult,bundle.Previous));

            var expected=QualityInspectionAuditWindowDiffRuntime.Diff(
                new QualityInspectionAuditWindow(new[]{
                    QualityInspectionAuditEnvelopeRuntime.Create(previousResult)}),
                new QualityInspectionAuditWindow(new[]{
                    QualityInspectionAuditEnvelopeRuntime.Create(currentResult)}));

            if(!expected.Equals(bundle.Diff))
                errors.Add("Audit replay bundle diff does not match the supplied results.");
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult currentResult,
        QualityInspectionAuditReplayBundle bundle,
        QualityInspectionResult? previousResult = null) =>
        Validate(currentResult,bundle,previousResult).Count==0;
}
