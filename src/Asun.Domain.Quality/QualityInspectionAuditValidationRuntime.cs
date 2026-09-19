namespace Asun.Domain.Quality;

public static class QualityInspectionAuditValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionAuditRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        var errors = new List<string>();

        if (record.ResultId == Guid.Empty)
            errors.Add("Audit result id cannot be empty.");

        if (record.SnapshotId == Guid.Empty)
            errors.Add("Audit snapshot id cannot be empty.");

        if (record.Sequence < 0)
            errors.Add("Audit sequence cannot be negative.");

        if (record.FindingCount < 0)
            errors.Add("Audit finding count cannot be negative.");

        if (record.EvidenceLinkCount < 0)
            errors.Add("Audit evidence count cannot be negative.");

        errors.AddRange(
            QualityInspectionDeterminismValidationRuntime
                .ValidateFingerprint(record.ContentFingerprint));

        return errors;
    }

    public static bool IsValid(QualityInspectionAuditRecord record) =>
        Validate(record).Count == 0;
}
