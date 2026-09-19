using Asun.Domain.Pcb;

namespace Asun.Production.Runtime;

public static class BoardProductionSessionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        BoardProductionSessionDefinition definition,
        BoardProductionSessionReport report)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(report);

        var errors=new List<string>(
            PcbAssemblySnapshotValidationRuntime.Validate(
                definition.Assembly));

        errors.AddRange(
            ProductionSessionValidationRuntime.Validate(
                definition.Production,
                report.ProductionReport));

        if(report.AssemblyFingerprint!=definition.Assembly.Fingerprint)
            errors.Add("Board production report assembly fingerprint must match the source assembly.");

        if(report.Fingerprint.Length!=64 ||
           !report.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Board production report fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count==0)
        {
            var canonical=string.Join(
                "|",
                definition.Assembly.Fingerprint,
                report.ProductionReport.Fingerprint,
                report.ProductionReport.SessionId);

            var expected=Convert.ToHexString(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(canonical)))
                .ToLowerInvariant();

            if(expected!=report.Fingerprint)
                errors.Add("Board production report fingerprint does not match the source assembly and production report.");
        }

        return errors;
    }

    public static bool IsValid(
        BoardProductionSessionDefinition definition,
        BoardProductionSessionReport report)=>
        Validate(definition,report).Count==0;
}
