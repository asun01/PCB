namespace Asun.Production.Runtime;

public static class ProductionSessionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ProductionSessionDefinition definition,
        ProductionSessionReport report)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(report);

        var errors=new List<string>();

        if(definition.SessionId==Guid.Empty)
            errors.Add("Production session id cannot be empty.");

        if(report.SessionId!=definition.SessionId)
            errors.Add("Production report session id must match the definition.");

        if(report.FrameCount!=report.Frames.Count ||
           report.FrameCount!=definition.FrameCount)
        {
            errors.Add("Production report frame count must match definition and frame collection.");
        }

        if(report.ProgramFingerprint!=definition.ProgramPlan.Fingerprint)
            errors.Add("Production report program fingerprint must match the definition.");

        var sequences=report.Frames.Select(frame=>frame.Sequence.Value).ToArray();

        if(!sequences.SequenceEqual(
            Enumerable.Range(1,report.FrameCount).Select(value=>(long)value)))
        {
            errors.Add("Production report frame sequences must be contiguous from one.");
        }

        if(report.Frames.Any(frame=>string.IsNullOrWhiteSpace(frame.InputFingerprint) ||
                                    frame.InputFingerprint.Length!=64 ||
                                    frame.PipelineReport is null))
        {
            errors.Add("Production report contains an invalid frame execution.");
        }

        if(report.Fingerprint.Length!=64 ||
           !report.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Production report fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(!errors.Any())
        {
            var expectedFingerprint=ProductionSessionFingerprintRuntime.CreateFingerprint(
                definition,
                report.Frames);

            if(expectedFingerprint!=report.Fingerprint)
                errors.Add("Production report fingerprint does not match the report.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionSessionDefinition definition,
        ProductionSessionReport report)=>
        Validate(definition,report).Count==0;
}
