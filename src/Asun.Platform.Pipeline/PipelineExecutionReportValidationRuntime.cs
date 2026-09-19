namespace Asun.Platform.Pipeline;

public static class PipelineExecutionReportValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PipelineExecutionReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        var errors=new List<string>();

        if(report.StageCount!=report.ExecutedStages.Count)
            errors.Add("Pipeline report stage count must match executed-stage count.");

        if(report.StageCount<0)
            errors.Add("Pipeline report stage count cannot be negative.");

        if(report.Fingerprint.Length!=64 ||
           !report.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Pipeline report fingerprint must be 64 lowercase hexadecimal characters.");
        }

        var expected=Convert.ToHexString(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(
                    string.Join("|",report.ExecutedStages))))
            .ToLowerInvariant();

        if(expected!=report.Fingerprint)
            errors.Add("Pipeline report fingerprint does not match the executed stage list.");

        return errors;
    }

    public static bool IsValid(
        PipelineExecutionReport report)=>
        Validate(report).Count==0;
}
