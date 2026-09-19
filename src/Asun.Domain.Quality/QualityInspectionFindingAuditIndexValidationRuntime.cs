namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditIndexValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionResult result,
        QualityInspectionFindingAuditIndex index)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(index);

        var errors=new List<string>();

        if(!QualityInspectionResultValidationRuntime.IsValid(result))
        {
            errors.Add("Inspection result is invalid.");
            return errors;
        }

        var expected=QualityInspectionFindingAuditIndexRuntime.Create(result);

        if(!index.FindingIds.SequenceEqual(expected.FindingIds))
            errors.Add("Finding audit index ids do not match the inspection result.");

        foreach(var findingId in expected.FindingIds)
        {
            var record=index.Find(findingId);
            if(record is null ||
               !QualityInspectionFindingAuditRecordValidationRuntime.IsValid(
                   result,
                   record))
            {
                errors.Add(
                    $"Finding audit index record {findingId.Value} is invalid.");
            }
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionResult result,
        QualityInspectionFindingAuditIndex index) =>
        Validate(result,index).Count==0;
}
