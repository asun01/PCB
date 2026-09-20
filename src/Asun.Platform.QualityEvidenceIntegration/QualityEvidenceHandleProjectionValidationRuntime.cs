using Asun.Domain.Quality;

namespace Asun.Platform.QualityEvidenceIntegration;

public static class QualityEvidenceHandleProjectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionRun run,
        QualityEvidenceHandleProjection projection)
    {
        ArgumentNullException.ThrowIfNull(run);
        ArgumentNullException.ThrowIfNull(projection);

        var errors=new List<string>();
        errors.AddRange(QualityEvidenceHandleBindingRuntime.Validate(run,projection.Bindings));

        if(projection.QualityRunId!=run.RunId)
            errors.Add("Quality evidence projection run id must match the source run.");

        if(projection.Fingerprint.Length!=64 ||
           !projection.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Quality evidence projection fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count>0)
            return errors;

        var expected=QualityEvidenceHandleProjectionRuntime.CreateFingerprint(
            run.RunId,
            projection.Bindings);
        if(expected!=projection.Fingerprint)
            errors.Add("Quality evidence projection fingerprint does not match its canonical content.");

        return errors;
    }

    public static bool IsValid(
        QualityInspectionRun run,
        QualityEvidenceHandleProjection projection)=>
        Validate(run,projection).Count==0;
}
