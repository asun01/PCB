namespace Asun.Domain.Quality;

public static class QualityInspectionRunFingerprintValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        QualityInspectionRun run,
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(run);

        var errors=new List<string>(
            QualityInspectionRunValidationRuntime.Validate(run));

        if(fingerprint is null ||
           fingerprint.Length!=64 ||
           !fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Inspection run fingerprint must be 64 lowercase hexadecimal characters.");
        }
        else if(QualityInspectionRunFingerprintRuntime.CreateFingerprint(run)!=fingerprint)
        {
            errors.Add("Inspection run fingerprint does not match the run.");
        }

        return errors;
    }

    public static bool IsValid(
        QualityInspectionRun run,
        string fingerprint)=>
        Validate(run,fingerprint).Count==0;
}
