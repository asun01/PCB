namespace Asun.Domain.Quality;

public static class QualityInspectionFindingAuditWindowValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        IEnumerable<QualityInspectionResult> results,
        QualityInspectionFindingAuditWindow window)
    {
        ArgumentNullException.ThrowIfNull(results);
        ArgumentNullException.ThrowIfNull(window);

        var resultArray=results.ToArray();
        var errors=new List<string>();
        var expectedIds=resultArray
            .Select(result=>result.ResultId)
            .Distinct()
            .ToHashSet();

        var actualIds=new HashSet<Guid>();

        foreach(var envelope in window.Envelopes)
        {
            if(envelope is null)
            {
                errors.Add("Finding audit window cannot contain null envelopes.");
                continue;
            }

            if(!actualIds.Add(envelope.ResultId))
                errors.Add("Finding audit window result ids must be unique.");

            var result=resultArray.FirstOrDefault(item=>item.ResultId==envelope.ResultId);
            if(result is null)
            {
                errors.Add(
                    $"Finding audit window contains an unknown result {envelope.ResultId:D}.");
                continue;
            }

            errors.AddRange(
                QualityInspectionFindingAuditEnvelopeValidationRuntime
                    .Validate(result,envelope));
        }

        if(!actualIds.SetEquals(expectedIds))
            errors.Add("Finding audit window does not cover the supplied results.");

        return errors;
    }

    public static bool IsValid(
        IEnumerable<QualityInspectionResult> results,
        QualityInspectionFindingAuditWindow window) =>
        Validate(results,window).Count==0;
}
