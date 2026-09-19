namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditRecordValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult result,
        QualityInspectionFindingAuditRecord record)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(record);

        var errors=new List<string>();

        if(!record.FindingId.IsValid)
            errors.Add("Finding audit id must be valid.");

        if(string.IsNullOrWhiteSpace(record.RuleCode))
            errors.Add("Finding audit RuleCode cannot be blank.");

        if(record.Outcome==QualityOutcome.Unknown)
            errors.Add("Finding audit outcome cannot be Unknown.");

        if(record.Severity is < QualitySeverity.None or > QualitySeverity.Critical)
            errors.Add("Finding audit severity is invalid.");

        if(string.IsNullOrWhiteSpace(record.Message))
            errors.Add("Finding audit message cannot be blank.");

        var finding=result.Findings.Find(record.FindingId);
        if(finding is null)
        {
            errors.Add("Finding audit record points to an unknown finding.");
            return errors;
        }

        if(record.RuleCode!=finding.RuleCode ||
           record.Outcome!=finding.Outcome ||
           record.Severity!=finding.Severity ||
           record.Message!=finding.Message)
        {
            errors.Add("Finding audit record content does not match the source finding.");
        }

        var expectedEvidence=result.Evidence
            .ForFinding(record.FindingId)
            .OrderBy(key=>key.Value,StringComparer.Ordinal)
            .ToArray();

        if(!record.EvidenceKeys.SequenceEqual(expectedEvidence))
            errors.Add("Finding audit evidence keys do not match the source finding.");

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult result,
        QualityInspectionFindingAuditRecord record) =>
        Validate(result,record).Count==0;
}
