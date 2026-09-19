namespace Asun.Domain.Quality;

public static class QualityInspectionAuditWindowValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        IEnumerable<QualityInspectionResult> results,
        QualityInspectionAuditWindow window)
    {
        ArgumentNullException.ThrowIfNull(results);
        ArgumentNullException.ThrowIfNull(window);

        var source=results.ToArray();
        var sourceIds=source.Select(result=>result.ResultId).ToHashSet();
        var actualIds=new HashSet<Guid>();
        var errors=new List<string>();

        foreach(var envelope in window.Envelopes)
        {
            if(!actualIds.Add(envelope.ResultId))
                errors.Add("Audit window result ids must be unique.");

            var result=source.FirstOrDefault(item=>item.ResultId==envelope.ResultId);
            if(result is null)
            {
                errors.Add("Audit window contains an unknown result id.");
                continue;
            }

            errors.AddRange(
                QualityInspectionAuditEnvelopeValidationRuntime
                    .Validate(result,envelope));
        }

        if(!actualIds.SetEquals(sourceIds))
            errors.Add("Audit window does not cover the supplied results.");

        return errors;
    }

    public static bool IsValid(
        IEnumerable<QualityInspectionResult> results,
        QualityInspectionAuditWindow window) =>
        Validate(results,window).Count==0;
}
